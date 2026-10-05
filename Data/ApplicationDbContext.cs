using Microsoft.EntityFrameworkCore;
using MIS期末.Models;

namespace MIS期末.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MembershipLevel> MembershipLevels { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PointRecord> PointRecords { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<StockRecord> StockRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed MembershipLevels
            modelBuilder.Entity<MembershipLevel>().HasData(
                new MembershipLevel { LevelID = 1, Name = "一般會員", MinPoints = 0, DiscountRate = 0, PointsMultiplier = 1, Description = "消費每百元獲得1點" },
                new MembershipLevel { LevelID = 2, Name = "銀卡會員", MinPoints = 500, DiscountRate = 5, PointsMultiplier = 2, Description = "享95折優惠，消費每百元獲得2點" },
                new MembershipLevel { LevelID = 3, Name = "金卡會員", MinPoints = 2000, DiscountRate = 10, PointsMultiplier = 3, Description = "享9折優惠，消費每百元獲得3點" }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, Name = "飲料", Description = "各式飲品" },
                new Category { CategoryID = 2, Name = "食品", Description = "零食小吃" },
                new Category { CategoryID = 3, Name = "日用品", Description = "生活用品" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductID = 1, Name = "礦泉水", Price = 20, Cost = 10, Stock = 100, LowStockThreshold = 10, CategoryID = 1, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Product { ProductID = 2, Name = "綠茶", Price = 25, Cost = 12, Stock = 80, LowStockThreshold = 10, CategoryID = 1, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Product { ProductID = 3, Name = "可樂", Price = 30, Cost = 15, Stock = 60, LowStockThreshold = 10, CategoryID = 1, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Product { ProductID = 4, Name = "洋芋片", Price = 35, Cost = 18, Stock = 50, LowStockThreshold = 5, CategoryID = 2, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Product { ProductID = 5, Name = "巧克力", Price = 45, Cost = 22, Stock = 40, LowStockThreshold = 5, CategoryID = 2, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Product { ProductID = 6, Name = "面紙", Price = 50, Cost = 25, Stock = 30, LowStockThreshold = 5, CategoryID = 3, IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }
    }
}
