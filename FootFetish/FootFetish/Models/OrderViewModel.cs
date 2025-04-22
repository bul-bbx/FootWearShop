namespace FootFetish.Models
{
    public class OrderViewModel
    {
        public string PaymentMethod { get; set; }
        public List<CartItem> CartItems { get; set; }
        public string Address { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
