using System.ComponentModel.DataAnnotations;

namespace MIS期末.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "請輸入分類名稱")]
        [StringLength(50)]
        [Display(Name = "分類名稱")]
        public string Name { get; set; } = "";

        [StringLength(200)]
        [Display(Name = "說明")]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
