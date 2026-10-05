using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS期末.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "請輸入商品名稱")]
        [StringLength(100)]
        [Display(Name = "商品名稱")]
        public string Name { get; set; } = "";

        [StringLength(500)]
        [Display(Name = "商品描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "請輸入售價")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999, ErrorMessage = "售價必須大於0")]
        [Display(Name = "售價")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "請輸入成本")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999)]
        [Display(Name = "成本")]
        public decimal Cost { get; set; }

        [Display(Name = "庫存數量")]
        public int Stock { get; set; } = 0;

        [Display(Name = "庫存警戒值")]
        public int LowStockThreshold { get; set; } = 5;

        [Display(Name = "商品圖片")]
        public string? ImageUrl { get; set; }

        [Display(Name = "是否上架")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "分類")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<StockRecord> StockRecords { get; set; } = new List<StockRecord>();
    }
}
