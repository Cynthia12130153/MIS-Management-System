using System.ComponentModel.DataAnnotations;

namespace MIS期末.Models
{
    public class StockRecord
    {
        [Key]
        public int RecordID { get; set; }

        public int ProductID { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "數量")]
        public int Quantity { get; set; }

        [StringLength(20)]
        [Display(Name = "類型")]
        public string Type { get; set; } = "In"; // In=進貨, Out=出貨, Adjustment=調整

        [StringLength(300)]
        [Display(Name = "備註")]
        public string? Notes { get; set; }

        [Display(Name = "時間")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
