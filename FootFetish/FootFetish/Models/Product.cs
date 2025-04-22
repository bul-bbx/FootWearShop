using System.ComponentModel.DataAnnotations;

namespace FootFetish.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        public decimal Price { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}