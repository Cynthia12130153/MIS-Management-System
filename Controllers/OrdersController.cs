using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;

namespace MIS期末.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrdersController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string? search)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var query = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDate.Date >= start && o.OrderDate.Date <= end);

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int orderId))
                    query = query.Where(o => o.OrderID == orderId);
                else
                    query = query.Where(o => o.Customer != null && o.Customer.Name.Contains(search));
            }

            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");
            ViewBag.Search = search;

            return View(await query.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Customer).ThenInclude(c => c!.Level)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _db.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderID == id);
            if (order == null) return NotFound();

            if (order.Status == "完成")
            {
                order.Status = "已取消";

                // Restore stock
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _db.Products.FindAsync(detail.ProductID);
                    if (product != null) product.Stock += detail.Quantity;
                }

                // Restore customer points
                if (order.CustomerID.HasValue)
                {
                    var customer = await _db.Customers.FindAsync(order.CustomerID);
                    if (customer != null)
                    {
                        customer.Points = customer.Points - order.PointsEarned + order.PointsRedeemed;
                        customer.TotalSpent -= order.FinalAmount;
                    }
                }

                await _db.SaveChangesAsync();
                TempData["Success"] = "訂單已取消！";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
