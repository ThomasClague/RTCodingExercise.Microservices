using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models
{
    public class CreatePlateViewModel
    {
        [Required(ErrorMessage = "Registration is required")]
        [Display(Name = "Registration")]
        public string Registration { get; set; }

        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than zero")]
        [Display(Name = "Purchase Price")]
        [DataType(DataType.Currency)]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "Sale price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be greater than zero")]
        [Display(Name = "Sale Price")]
        [DataType(DataType.Currency)]
        public decimal SalePrice { get; set; }

        public List<string> Errors { get; set; } = new();
    }
} 