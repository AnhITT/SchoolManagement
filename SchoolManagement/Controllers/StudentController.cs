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
        public async Task<IActionResult> Create()
        {
            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                    CheckTime = DateTime.Now
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                attendance.IsPresent = isPresent;
                attendance.CheckTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
} 