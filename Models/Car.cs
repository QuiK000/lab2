namespace WebApplication2.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Year { get; set; }
        public string BodyType { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    }
}
