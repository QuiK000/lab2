using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public int? DriverId { get; set; }
        public virtual Driver? Driver { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}