using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models;
using MIS期末.Models.ViewModels;
using System.Text.Json;

namespace MIS期末.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SalesController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _db.Products.Include(p => p.Category)
                .Where(p => p.IsActive && p.Stock > 0);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId);

            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "CategoryID", "Name");
            ViewBag.Search = search;

            return View(await query.OrderBy(p => p.CategoryID).ThenBy(p => p.Name).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomerInfo(int customerId)
        {
            var customer = await _db.Customers.Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);
            if (customer == null) return Json(new { success = false });

            return Json(new
            {
                success     = true,
                name        = customer.Name,
                points      = customer.Points,
                discountRate = customer.Level?.DiscountRate ?? 0,
                levelName   = customer.Level?.Name ?? "一般會員"
            });
        }

        // 接收前端 JS POST 過來的購物車 JSON
        [HttpPost]
        public async Task<IActionResult> Checkout(string cartJson)
        {
            List<CartItem> cart;
            try
            {
                cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch { return RedirectToAction(nameof(Index)); }

            if (!cart.Any()) return RedirectToAction(nameof(Index));

            var vm = new CheckoutViewModel
            {
                Items       = cart,
                TotalAmount = cart.Sum(c => c.SubTotal)
            };

            ViewBag.Customers = new SelectList(await _db.Customers.ToListAsync(), "CustomerID", "Name");
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel vm)
        {
            var cart = vm.Items;
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "購物車是空的！";
                return RedirectToAction(nameof(Index));
            }

            foreach (var item in cart)
            {
                var product = await _db.Products.FindAsync(item.ProductID);
                if (product == null || product.Stock < item.Quantity)
                {
                    TempData["Error"] = $"商品 {item.ProductName} 庫存不足！";
                    return RedirectToAction(nameof(Index));
                }
            }

            var totalAmount   = cart.Sum(c => c.SubTotal);
            decimal discount  = 0;
            int pointsEarned  = 0;

            Customer? customer = null;
            if (vm.CustomerID.HasValue)
                customer = await _db.Customers.Include(c => c.Level)
                    .FirstOrDefaultAsync(c => c.CustomerID == vm.CustomerID);

            if (customer?.Level != null && customer.Level.DiscountRate > 0)
                discount += totalAmount * customer.Level.DiscountRate / 100;

            if (vm.PointsToRedeem > 0 && customer != null && customer.Points >= vm.PointsToRedeem)
                discount += vm.PointsToRedeem / 10m;

            var finalAmount = Math.Max(0, totalAmount - discount);

            var order = new Order
            {
                OrderDate      = DateTime.Now,
                TotalAmount    = totalAmount,
                DiscountAmount = discount,
                FinalAmount    = finalAmount,
                CustomerID     = vm.CustomerID,
                PointsRedeemed = vm.PointsToRedeem,
                PaymentMethod  = vm.PaymentMethod ?? "現金",
                Status         = "完成",
                Notes          = vm.Notes
            };

            if (customer?.Level != null)
            {
                pointsEarned    = (int)(finalAmount / 100) * customer.Level.PointsMultiplier;
                order.PointsEarned = pointsEarned;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in cart)
            {
                _db.OrderDetails.Add(new OrderDetail
                {
                    OrderID   = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity  = item.Quantity,
                    UnitPrice = item.Price,
                    SubTotal  = item.SubTotal
                });

                var product = await _db.Products.FindAsync(item.ProductID);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _db.StockRecords.Add(new StockRecord
                    {
                        ProductID = item.ProductID,
                        Quantity  = item.Quantity,
                        Type      = "Out",
                        Notes     = $"訂單 #{order.OrderID}",
                        CreatedAt = DateTime.Now
                    });
                }
            }

            if (customer != null)
            {
                customer.Points     = customer.Points - vm.PointsToRedeem + pointsEarned;
                customer.TotalSpent += finalAmount;

                if (vm.PointsToRedeem > 0)
                    _db.PointRecords.Add(new PointRecord
                    {
                        CustomerID  = customer.CustomerID,
                        Points      = -vm.PointsToRedeem,
                        Type        = "Redeem",
                        Description = $"訂單 #{order.OrderID} 點數折抵",
                        OrderID     = order.OrderID,
                        CreatedAt   = DateTime.Now
                    });

                if (pointsEarned > 0)
                    _db.PointRecords.Add(new PointRecord
                    {
                        CustomerID  = customer.CustomerID,
                        Points      = pointsEarned,
                        Type        = "Earn",
                        Description = $"訂單 #{order.OrderID} 消費獲點",
                        OrderID     = order.OrderID,
                        CreatedAt   = DateTime.Now
                    });

                var newLevel = await _db.MembershipLevels
                    .Where(l => l.MinPoints <= customer.Points)
                    .OrderByDescending(l => l.MinPoints)
                    .FirstOrDefaultAsync();
                if (newLevel != null)
                    customer.LevelID = newLevel.LevelID;
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = $"結帳完成！訂單編號 #{order.OrderID}";
            return RedirectToAction("Details", "Orders", new { id = order.OrderID });
        }
    }
}
