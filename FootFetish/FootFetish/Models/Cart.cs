using System.ComponentModel.DataAnnotations;

namespace FootFetish.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> Items { get; set; }
    }
}
