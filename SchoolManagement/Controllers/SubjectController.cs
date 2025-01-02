using SchoolManagement.Data;
using SchoolManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.ViewModels;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.StudentSubjects)
                .ToListAsync();
            return View(subjects);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm môn học.");
                }
            }
            return View(subject);
        }

        public async Task<IActionResult> Detail(int id, int page = 1)
        {
            var subject = await _context.Subjects
                .Include(s => s.Sessions.OrderBy(s => s.Date).ThenBy(s => s.StartTime))
                    .ThenInclude(s => s.Attendances.Where(a => a.IsPresent))
                .Include(s => s.StudentSubjects)
                    .ThenInclude(ss => ss.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            // Tính toán trước các thông số
            var today = DateTime.Now.Date;
            var allSessions = subject.Sessions
                .Select(s => new SessionViewModel
                {
                    Session = s,
                    IsUpcoming = s.Date.Date > today,
                    IsPast = s.Date.Date < today,
                    IsToday = s.Date.Date == today,
                    TotalStudents = subject.StudentSubjects.Count,
                    PresentCount = s.Attendances.Count
                })
                .OrderBy(s => s.Session.Date)
                .ThenBy(s => s.Session.StartTime)
                .ToList();

            // Phân trang
            const int pageSize = 10;
            var paginatedSessions = new PaginatedList<SessionViewModel>(
                allSessions.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                allSessions.Count,
                page,
                pageSize
            );

            ViewBag.PaginatedSessions = paginatedSessions;
            return View(subject);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Subject subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(subject);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var subject = await _context.Subjects
                    .Include(s => s.StudentSubjects)
                    .Include(s => s.Sessions)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (subject == null)
                {
                    return NotFound();
                }

                if (subject.StudentSubjects.Any() || subject.Sessions.Any())
                {
                    TempData["Error"] = "Không thể xóa môn học đang có sinh viên đăng ký hoặc có buổi học";
                    return RedirectToAction(nameof(Index));
                }

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa môn học thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa môn học";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddStudent(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            var registeredStudentIds = subject.StudentSubjects.Select(ss => ss.StudentId).ToList();

            var students = await _context.Students
                .Include(s => s.Class)
                .Select(s => new StudentInSubjectViewModel
                {
                    StudentId = s.Id,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    ClassName = s.Class.ClassName,
                    IsRegistered = registeredStudentIds.Contains(s.Id)
                })
                .OrderBy(s => s.ClassName)
                .ThenBy(s => s.FullName)
                .ToListAsync();

            var viewModel = new AddStudentToSubjectViewModel
            {
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Students = students
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(AddStudentToSubjectViewModel model)
        {
            var subject = await _context.Subjects
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == model.SubjectId);

            if (subject == null)
            {
                return NotFound();
            }

            try
            {
                // Lấy danh sách sinh viên đã chọn
                var selectedStudents = model.Students.Where(s => s.IsSelected).ToList();
                var currentStudentIds = subject.StudentSubjects.Select(ss => ss.StudentId).ToList();

                // Thêm sinh viên mới
                foreach (var student in selectedStudents)
                {
                    if (!currentStudentIds.Contains(student.StudentId))
                    {
                        subject.StudentSubjects.Add(new StudentSubject
                        {
                            SubjectId = subject.Id,
                            StudentId = student.StudentId
                        });
                    }
                }

                // Xóa sinh viên bị bỏ chọn
                var studentsToRemove = subject.StudentSubjects
                    .Where(ss => !selectedStudents.Any(s => s.StudentId == ss.StudentId))
                    .ToList();

                foreach (var studentSubject in studentsToRemove)
                {
                    subject.StudentSubjects.Remove(studentSubject);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật danh sách sinh viên thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật danh sách sinh viên";
            }

            return RedirectToAction(nameof(Detail), new { id = model.SubjectId });
        }
    }
} 