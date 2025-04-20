using FootFetish.Models;
using System.ComponentModel.DataAnnotations;

namespace FootFetish.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}