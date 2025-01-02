using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class SessionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int subjectId)
        {
            var subject = await _context.Subjects
                .Include(s => s.Sessions)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                return NotFound();
            }

            ViewBag.Subject = subject;
            return View(subject.Sessions.OrderBy(s => s.Date).ThenBy(s => s.StartTime));
        }

        public async Task<IActionResult> Create(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                return NotFound();
            }

            ViewBag.Subject = subject;
            return View(new Session { SubjectId = subjectId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Session session)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra môn học tồn tại
                    var subject = await _context.Subjects.FindAsync(session.SubjectId);
                    if (subject == null)
                    {
                        TempData["Error"] = "Không tìm thấy môn học";
                        return RedirectToAction("Index", "Subject");
                    }

                    // Tạo danh sách điểm danh cho tất cả sinh viên trong môn học
                    var students = await _context.StudentSubjects
                        .Where(ss => ss.SubjectId == session.SubjectId)
                        .Select(ss => ss.Student)
                        .ToListAsync();

                    foreach (var student in students)
                    {
                        session.Attendances.Add(new Attendance
                        {
                            StudentId = student.Id,
                            IsPresent = false
                        });
                    }

                    _context.Sessions.Add(session);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm buổi học thành công";
                    return RedirectToAction("Detail", "Subject", new { id = session.SubjectId });
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Dữ liệu không hợp lệ: {errors}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi thêm buổi học: {ex.Message}";
            }

            return RedirectToAction("Detail", "Subject", new { id = session.SubjectId });
        }

        public async Task<IActionResult> Attendance(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Subject)
                    .ThenInclude(s => s.StudentSubjects)
                        .ThenInclude(ss => ss.Student)
                            .ThenInclude(s => s.Class)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            // Lấy danh sách sinh viên từ StudentSubjects
            var studentsInSubject = session.Subject.StudentSubjects
                .Select(ss => ss.Student)
                .OrderBy(s => s.StudentCode)
                .ToList();

            // Tạo hoặc cập nhật bản ghi điểm danh cho mỗi sinh viên
            foreach (var student in studentsInSubject)
            {
                var attendance = session.Attendances
                    .FirstOrDefault(a => a.StudentId == student.Id);

                if (attendance == null)
                {
                    // Nếu chưa có bản ghi điểm danh, tạo mới với IsPresent = false
                    attendance = new Attendance
                    {
                        SessionId = session.Id,
                        StudentId = student.Id,
                        Student = student,
                        IsPresent = false,
                        AttendanceTime = DateTime.Now,
                        Note = ""
                    };
                    session.Attendances.Add(attendance);
                    _context.Attendances.Add(attendance);
                }
            }

            await _context.SaveChangesAsync();

            // Sắp xếp lại danh sách điểm danh theo mã sinh viên
            session.Attendances = session.Attendances
                .OrderBy(a => a.Student.StudentCode)
                .ToList();

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAttendance(int sessionId, List<AttendanceViewModel> attendances)
        {
            var session = await _context.Sessions
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            try
            {
                foreach (var attendance in attendances)
                {
                    var existingAttendance = session.Attendances
                        .FirstOrDefault(a => a.StudentId == attendance.StudentId);

                    if (existingAttendance != null)
                    {
                        existingAttendance.IsPresent = attendance.IsPresent;
                        existingAttendance.Note = attendance.Note;
                        existingAttendance.AttendanceTime = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật điểm danh thành công";
                return RedirectToAction("Detail", "Subject", new { id = session.SubjectId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật điểm danh";
                return RedirectToAction("Detail", "Subject", new { id = session.SubjectId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMultiple(CreateSessionsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var subject = await _context.Subjects
                        .Include(s => s.StudentSubjects)
                        .FirstOrDefaultAsync(s => s.Id == model.SubjectId);

                    if (subject == null)
                    {
                        TempData["Error"] = "Không tìm thấy môn học";
                        return RedirectToAction("Index", "Subject");
                    }

                    // Lấy danh sách sinh viên đã đăng ký môn học
                    var studentIds = subject.StudentSubjects.Select(ss => ss.StudentId).ToList();

                    var currentDate = model.StartDate;
                    var sessionCount = 1;
                    var sessions = new List<Session>();

                    while (currentDate <= model.EndDate)
                    {
                        if (model.WeekDays.Contains(currentDate.DayOfWeek))
                        {
                            var session = new Session
                            {
                                SubjectId = model.SubjectId,
                                SessionName = $"Buổi {sessionCount++}",
                                Date = currentDate,
                                StartTime = model.StartTime,
                                EndTime = model.EndTime,
                                Description = $"Buổi học thứ {GetVietnameseDayOfWeek(currentDate.DayOfWeek)}",
                                Attendances = new List<Attendance>()
                            };

                            // Tạo bản ghi điểm danh cho từng sinh viên
                            foreach (var studentId in studentIds)
                            {
                                session.Attendances.Add(new Attendance
                                {
                                    StudentId = studentId,
                                    IsPresent = false,
                                    AttendanceTime = DateTime.Now
                                });
                            }

                            sessions.Add(session);
                        }
                        currentDate = currentDate.AddDays(1);
                    }

                    _context.Sessions.AddRange(sessions);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Đã tạo {sessions.Count} buổi học thành công";
                    return RedirectToAction("Detail", "Subject", new { id = model.SubjectId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("Detail", "Subject", new { id = model.SubjectId });
        }

        private string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Hai",
                DayOfWeek.Tuesday => "Ba",
                DayOfWeek.Wednesday => "Tư",
                DayOfWeek.Thursday => "Năm",
                DayOfWeek.Friday => "Sáu",
                DayOfWeek.Saturday => "Bảy",
                DayOfWeek.Sunday => "Chủ nhật",
                _ => dayOfWeek.ToString()
            };
        }
    }
} 