using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models.ViewModels;

namespace MIS期末.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var vm = new DashboardViewModel
            {
                TodayRevenue = await _db.Orders
                    .Where(o => o.OrderDate.Date == today && o.Status == "完成")
                    .SumAsync(o => (decimal?)o.FinalAmount) ?? 0,

                TodayOrders = await _db.Orders
                    .Where(o => o.OrderDate.Date == today && o.Status == "完成")
                    .CountAsync(),

                MonthRevenue = await _db.Orders
                    .Where(o => o.OrderDate >= monthStart && o.Status == "完成")
                    .SumAsync(o => (decimal?)o.FinalAmount) ?? 0,

                MonthOrders = await _db.Orders
                    .Where(o => o.OrderDate >= monthStart && o.Status == "完成")
                    .CountAsync(),

                TotalProducts = await _db.Products.Where(p => p.IsActive).CountAsync(),

                LowStockCount = await _db.Products
                    .Where(p => p.IsActive && p.Stock <= p.LowStockThreshold)
                    .CountAsync(),

                TotalCustomers = await _db.Customers.CountAsync(),

                TopProducts = await _db.OrderDetails
                    .Where(d => d.Order!.Status == "完成" && d.Order.OrderDate >= monthStart)
                    .GroupBy(d => d.Product!.Name)
                    .Select(g => new TopProductItem
                    {
                        ProductName = g.Key,
                        TotalQty = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.SubTotal)
                    })
                    .OrderByDescending(x => x.TotalQty)
                    .Take(5)
                    .ToListAsync(),

                RecentOrders = await _db.Orders
                    .Include(o => o.Customer)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .Select(o => new RecentOrderItem
                    {
                        OrderID = o.OrderID,
                        OrderDate = o.OrderDate,
                        CustomerName = o.Customer != null ? o.Customer.Name : "散客",
                        FinalAmount = o.FinalAmount,
                        Status = o.Status
                    })
                    .ToListAsync(),

                WeeklySales = (await _db.Orders
                    .Where(o => o.OrderDate >= today.AddDays(-6) && o.Status == "完成")
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new { Date = g.Key, Revenue = g.Sum(x => x.FinalAmount), Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToListAsync())
                    .Select(x => new DailySalesItem { Date = x.Date.ToString("MM/dd"), Revenue = x.Revenue, Orders = x.Count })
                    .ToList()
            };

            return View(vm);
        }
    }
}
