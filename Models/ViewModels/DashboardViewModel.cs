namespace MIS期末.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public decimal MonthRevenue { get; set; }
        public int MonthOrders { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int TotalCustomers { get; set; }
        public List<TopProductItem> TopProducts { get; set; } = new();
        public List<RecentOrderItem> RecentOrders { get; set; } = new();
        public List<DailySalesItem> WeeklySales { get; set; } = new();
    }

    public class TopProductItem
    {
        public string ProductName { get; set; } = "";
        public int TotalQty { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RecentOrderItem
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = "";
    }

    public class DailySalesItem
    {
        public string Date { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }
}
