namespace MIS期末.Models.ViewModels
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    }

    public class CheckoutViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount => TotalAmount - DiscountAmount;
        public int? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public int CustomerPoints { get; set; }
        public int PointsToRedeem { get; set; }
        public string PaymentMethod { get; set; } = "現金";
        public string? Notes { get; set; }
        public decimal DiscountRate { get; set; }
    }
}
