namespace MIS期末.Models.ViewModels
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesItem> DailySales { get; set; } = new();
        public List<TopProductItem> TopProducts { get; set; } = new();
        public List<PaymentMethodItem> PaymentMethods { get; set; } = new();
    }

    public class PaymentMethodItem
    {
        public string Method { get; set; } = "";
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}
