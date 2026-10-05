using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models;

namespace MIS期末.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public InventoryController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? filter)
        {
            var query = _db.Products.Include(p => p.Category).Where(p => p.IsActive);

            if (filter == "low")
                query = query.Where(p => p.Stock <= p.LowStockThreshold);

            ViewBag.Filter = filter;
            var products = await query.OrderBy(p => p.Stock).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Restock(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();
            ViewBag.Product = product;
            return View(new StockRecord { ProductID = id, Type = "In" });
        }

        [HttpPost]
        public async Task<IActionResult> Restock(StockRecord record)
        {
            if (record.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "數量必須大於0");
            }

            if (ModelState.IsValid)
            {
                var product = await _db.Products.FindAsync(record.ProductID);
                if (product == null) return NotFound();

                if (record.Type == "In")
                    product.Stock += record.Quantity;
                else if (record.Type == "Out")
                {
                    if (product.Stock < record.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "庫存不足！");
                        ViewBag.Product = product;
                        return View(record);
                    }
                    product.Stock -= record.Quantity;
                }
                else if (record.Type == "Adjustment")
                    product.Stock = record.Quantity;

                record.CreatedAt = DateTime.Now;
                _db.StockRecords.Add(record);
                await _db.SaveChangesAsync();

                TempData["Success"] = "庫存更新成功！";
                return RedirectToAction(nameof(Index));
            }

            var p = await _db.Products.FindAsync(record.ProductID);
            ViewBag.Product = p;
            return View(record);
        }

        public async Task<IActionResult> History(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var records = await _db.StockRecords
                .Where(r => r.ProductID == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Product = product;
            return View(records);
        }
    }
}
