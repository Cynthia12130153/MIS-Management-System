using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models.ViewModels;

namespace MIS期末.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReportsController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-29);
            var end = (endDate ?? DateTime.Today).AddDays(1).AddSeconds(-1);

            var orders = await _db.Orders
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Where(o => o.OrderDate >= start && o.OrderDate <= end && o.Status == "完成")
                .ToListAsync();

            var vm = new ReportViewModel
            {
                StartDate = start,
                EndDate = end.Date,
                TotalRevenue = orders.Sum(o => o.FinalAmount),
                TotalOrders = orders.Count,
                AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.FinalAmount) : 0,

                DailySales = orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new DailySalesItem
                    {
                        Date = g.Key.ToString("MM/dd"),
                        Revenue = g.Sum(x => x.FinalAmount),
                        Orders = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList(),

                TopProducts = orders
                    .SelectMany(o => o.OrderDetails)
                    .GroupBy(d => d.Product!.Name)
                    .Select(g => new TopProductItem
                    {
                        ProductName = g.Key,
                        TotalQty = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.SubTotal)
                    })
                    .OrderByDescending(x => x.TotalQty)
                    .Take(10)
                    .ToList(),

                PaymentMethods = orders
                    .GroupBy(o => o.PaymentMethod)
                    .Select(g => new PaymentMethodItem
                    {
                        Method = g.Key,
                        Count = g.Count(),
                        Amount = g.Sum(x => x.FinalAmount)
                    })
                    .OrderByDescending(x => x.Amount)
                    .ToList()
            };

            return View(vm);
        }
    }
}
