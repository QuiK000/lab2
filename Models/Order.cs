namespace WebApplication2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get;set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string DestinationAddress { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Нове";
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
