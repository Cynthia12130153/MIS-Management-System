using System.ComponentModel.DataAnnotations;

namespace MIS期末.Models
{
    public class PointRecord
    {
        [Key]
        public int RecordID { get; set; }

        public int CustomerID { get; set; }
        public Customer? Customer { get; set; }

        [Display(Name = "點數變動")]
        public int Points { get; set; }

        [StringLength(20)]
        [Display(Name = "類型")]
        public string Type { get; set; } = "Earn"; // Earn, Redeem

        [StringLength(200)]
        [Display(Name = "說明")]
        public string? Description { get; set; }

        public int? OrderID { get; set; }

        [Display(Name = "時間")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
