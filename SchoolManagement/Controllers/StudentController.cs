using SchoolManagement.Data;
using SchoolManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Class)
                .ToListAsync();
            return View(students);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Classes = _context.Classes.ToList();
                return View();
            }
            catch (Exception ex)
            {
                // Log error
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student student)
        {
            // Kiểm tra mã sinh viên đã tồn tại chưa
            if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
            {
                ModelState.AddModelError("StudentCode", "Mã sinh viên này đã tồn tại");
                ViewBag.Classes = await _context.Classes.ToListAsync();
                return View(student);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    student.StudentSubjects = new List<StudentSubject>();
                    student.Attendances = new List<Attendance>();
                    
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm sinh viên.");
                }
            }
            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View(student);
        }

        public async Task<IActionResult> Attendance(int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            var students = await _context.StudentSubjects
                .Where(ss => ss.SubjectId == session.SubjectId)
                .Include(ss => ss.Student)
                .Select(ss => ss.Student)
                .ToListAsync();

            ViewBag.Session = session;
            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int sessionId, int studentId, bool isPresent)
        {
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.StudentId == studentId);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    SessionId = sessionId,
                    StudentId = studentId,
                    IsPresent = isPresent,
                    AttendanceTime = DateTime.Now
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                attendance.IsPresent = isPresent;
                attendance.AttendanceTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Manage()
        {
            var students = await _context.Students
                .Include(s => s.Class)
                .ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }
            
            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View(student);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudent = await _context.Students
                        .Include(s => s.StudentSubjects)
                        .Include(s => s.Attendances)
                        .FirstOrDefaultAsync(s => s.Id == student.Id);

                    if (existingStudent == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mã sinh viên đã tồn tại chưa (trừ chính nó)
                    if (await _context.Students.AnyAsync(s => 
                        s.StudentCode == student.StudentCode && 
                        s.Id != student.Id))
                    {
                        ModelState.AddModelError("StudentCode", "Mã sinh viên này đã tồn tại");
                        ViewBag.Classes = await _context.Classes.ToListAsync();
                        return View(student);
                    }

                    // Cập nhật các thuộc tính
                    existingStudent.StudentCode = student.StudentCode;
                    existingStudent.FullName = student.FullName;
                    existingStudent.DateOfBirth = student.DateOfBirth;
                    existingStudent.Email = student.Email;
                    existingStudent.Phone = student.Phone;
                    existingStudent.ClassId = student.ClassId;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View(student);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.StudentSubjects)
                    .Include(s => s.Attendances)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                // Xóa các bản ghi liên quan
                _context.StudentSubjects.RemoveRange(student.StudentSubjects);
                _context.Attendances.RemoveRange(student.Attendances);
                _context.Students.Remove(student);
                
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa sinh viên thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa sinh viên";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
} 