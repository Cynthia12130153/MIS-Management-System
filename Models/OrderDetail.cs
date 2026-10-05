using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS期末.Models
{
    public class OrderDetail
    {
        [Key]
        public int DetailID { get; set; }

        public int OrderID { get; set; }
        public Order? Order { get; set; }

        public int ProductID { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "數量")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "單價")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "小計")]
        public decimal SubTotal { get; set; }
    }
}
