using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть назву послуги")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва має містити від 2 до 50 символів")]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть опис")]
        [StringLength(500, ErrorMessage = "Опис не може перевищувати 500 символів")]
        [Display(Name = "Опис")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть ціну за км")]
        [Range(1, 1000, ErrorMessage = "Ціна має бути від 1 до 1000 грн")]
        [Display(Name = "Ціна за км")]
        public decimal PricePerKm { get; set; }

        [Required(ErrorMessage = "Введіть базову ціну")]
        [Range(1, 5000, ErrorMessage = "Базова ціна має бути від 1 до 5000 грн")]
        [Display(Name = "Базова ціна")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Введіть тип автомобіля")]
        [StringLength(50, ErrorMessage = "Тип автомобіля не може перевищувати 50 символів")]
        [Display(Name = "Тип автомобіля")]
        public string CarType { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
