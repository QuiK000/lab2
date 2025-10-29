using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть ім'я")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 100 символів")]
        [Display(Name = "Ім'я клієнта")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть телефон")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть адресу подачі")]
        [StringLength(200, ErrorMessage = "Адреса не може перевищувати 200 символів")]
        [Display(Name = "Адреса подачі")]
        public string PickupAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть адресу призначення")]
        [StringLength(200, ErrorMessage = "Адреса не може перевищувати 200 символів")]
        [Display(Name = "Адреса призначення")]
        public string DestinationAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть відстань")]
        [Range(0.1, 1000, ErrorMessage = "Відстань має бути від 0.1 до 1000 км")]
        [Display(Name = "Відстань (км)")]
        public decimal Distance { get; set; }

        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Нове";

        [Required(ErrorMessage = "Оберіть послугу")]
        [Display(Name = "Послуга")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? AssignedDriverId { get; set; }
        public Driver? AssignedDriver { get; set; }

        public int? AssignedCarId { get; set; }
        public Car? AssignedCar { get; set; }
    }
}