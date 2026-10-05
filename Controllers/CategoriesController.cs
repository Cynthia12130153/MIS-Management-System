using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models;

namespace MIS期末.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoriesController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var cats = await _db.Categories.Include(c => c.Products).ToListAsync();
            return View(cats);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "分類新增成功！";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryID) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "分類更新成功！";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == id);
            if (cat == null) return NotFound();
            if (cat.Products.Any())
            {
                TempData["Error"] = "此分類下有商品，無法刪除！";
                return RedirectToAction(nameof(Index));
            }
            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            TempData["Success"] = "分類刪除成功！";
            return RedirectToAction(nameof(Index));
        }
    }
}
