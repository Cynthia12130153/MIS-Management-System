using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS期末.Data;
using MIS期末.Models;

namespace MIS期末.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CustomersController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Customers.Include(c => c.Level).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Phone.Contains(search));

            ViewBag.Search = search;
            return View(await query.OrderByDescending(c => c.TotalSpent).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _db.Customers
                .Include(c => c.Level)
                .Include(c => c.Orders).ThenInclude(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(c => c.PointRecords)
                .FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer == null) return NotFound();

            var nextLevel = await _db.MembershipLevels
                .Where(l => l.MinPoints > customer.Points)
                .OrderBy(l => l.MinPoints)
                .FirstOrDefaultAsync();

            ViewBag.NextLevel = nextLevel;
            return View(customer);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Levels = await _db.MembershipLevels.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (await _db.Customers.AnyAsync(c => c.Phone == customer.Phone))
            {
                ModelState.AddModelError("Phone", "此手機號碼已被使用！");
            }

            if (ModelState.IsValid)
            {
                customer.JoinDate = DateTime.Now;
                customer.LevelID = 1;
                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
                TempData["Success"] = "會員新增成功！";
                return RedirectToAction(nameof(Details), new { id = customer.CustomerID });
            }
            ViewBag.Levels = await _db.MembershipLevels.ToListAsync();
            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            ViewBag.Levels = await _db.MembershipLevels.ToListAsync();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerID) return NotFound();

            if (await _db.Customers.AnyAsync(c => c.Phone == customer.Phone && c.CustomerID != id))
            {
                ModelState.AddModelError("Phone", "此手機號碼已被使用！");
            }

            if (ModelState.IsValid)
            {
                var existing = await _db.Customers.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = customer.Name;
                existing.Phone = customer.Phone;
                existing.Email = customer.Email;
                existing.Address = customer.Address;

                await _db.SaveChangesAsync();
                TempData["Success"] = "會員資料更新成功！";
                return RedirectToAction(nameof(Details), new { id });
            }
            ViewBag.Levels = await _db.MembershipLevels.ToListAsync();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AdjustPoints(int customerId, int points, string description)
        {
            var customer = await _db.Customers.FindAsync(customerId);
            if (customer == null) return NotFound();

            customer.Points += points;
            if (customer.Points < 0) customer.Points = 0;

            _db.PointRecords.Add(new PointRecord
            {
                CustomerID = customerId,
                Points = points,
                Type = points > 0 ? "Earn" : "Redeem",
                Description = description,
                CreatedAt = DateTime.Now
            });

            // Level check
            var newLevel = await _db.MembershipLevels
                .Where(l => l.MinPoints <= customer.Points)
                .OrderByDescending(l => l.MinPoints)
                .FirstOrDefaultAsync();
            if (newLevel != null)
                customer.LevelID = newLevel.LevelID;

            await _db.SaveChangesAsync();
            TempData["Success"] = "點數調整成功！";
            return RedirectToAction(nameof(Details), new { id = customerId });
        }
    }
}
