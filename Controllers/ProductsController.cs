using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models;

namespace MIS期末.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductsController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? search, int? categoryId, string? status)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId);

            if (status == "active")
                query = query.Where(p => p.IsActive);
            else if (status == "inactive")
                query = query.Where(p => !p.IsActive);
            else if (status == "lowstock")
                query = query.Where(p => p.Stock <= p.LowStockThreshold);

            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name");
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Status = status;

            return View(await query.OrderBy(p => p.CategoryID).ThenBy(p => p.Name).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(p => p.Category)
                .Include(p => p.StockRecords)
                .FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = "商品新增成功！";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name");
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductID) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = "商品更新成功！";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "商品已下架！";
            return RedirectToAction(nameof(Index));
        }
    }
}
