using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS期末.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "請輸入會員姓名")]
        [StringLength(50)]
        [Display(Name = "姓名")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "請輸入手機號碼")]
        [StringLength(20)]
        [Phone]
        [Display(Name = "手機")]
        public string Phone { get; set; } = "";

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "地址")]
        public string? Address { get; set; }

        [Display(Name = "累積點數")]
        public int Points { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "累積消費金額")]
        public decimal TotalSpent { get; set; } = 0;

        [Display(Name = "加入日期")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Display(Name = "會員等級")]
        public int LevelID { get; set; } = 1;
        public MembershipLevel? Level { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<PointRecord> PointRecords { get; set; } = new List<PointRecord>();
    }
}
