using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS期末.Models
{
    public class MembershipLevel
    {
        [Key]
        public int LevelID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "等級名稱")]
        public string Name { get; set; } = "";

        [Display(Name = "所需點數")]
        public int MinPoints { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "折扣率 (%)")]
        public decimal DiscountRate { get; set; } = 0;

        [Display(Name = "點數倍率")]
        public int PointsMultiplier { get; set; } = 1;

        [StringLength(200)]
        [Display(Name = "說明")]
        public string? Description { get; set; }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
