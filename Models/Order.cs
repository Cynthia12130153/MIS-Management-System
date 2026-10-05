using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS期末.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Display(Name = "訂單日期")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "小計")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "折扣金額")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "實付金額")]
        public decimal FinalAmount { get; set; }

        [Display(Name = "會員")]
        public int? CustomerID { get; set; }
        public Customer? Customer { get; set; }

        [Display(Name = "獲得點數")]
        public int PointsEarned { get; set; } = 0;

        [Display(Name = "折抵點數")]
        public int PointsRedeemed { get; set; } = 0;

        [StringLength(20)]
        [Display(Name = "付款方式")]
        public string PaymentMethod { get; set; } = "現金";

        [StringLength(20)]
        [Display(Name = "訂單狀態")]
        public string Status { get; set; } = "完成";

        [StringLength(300)]
        [Display(Name = "備註")]
        public string? Notes { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
