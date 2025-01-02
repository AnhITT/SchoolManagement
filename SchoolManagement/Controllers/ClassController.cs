using SchoolManagement.Data;
using SchoolManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ClassController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Students)
                .ToListAsync();
            return View(classes);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Class model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Classes.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm lớp học.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var class_ = await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (class_ == null)
            {
                return NotFound();
            }

            return View(class_);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var class_ = await _context.Classes.FindAsync(id);
            if (class_ == null)
            {
                return NotFound();
            }
            return View(class_);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Class model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var class_ = await _context.Classes
                    .Include(c => c.Students)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (class_ == null)
                {
                    return NotFound();
                }

                if (class_.Students.Any())
                {
                    TempData["Error"] = "Không thể xóa lớp học đang có sinh viên";
                    return RedirectToAction(nameof(Index));
                }

                _context.Classes.Remove(class_);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa lớp học thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa lớp học";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
} 