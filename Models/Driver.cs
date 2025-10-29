namespace WebApplication2.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int Experience { get; set; }
        public decimal Rating { get; set; } = 5.0m;
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
