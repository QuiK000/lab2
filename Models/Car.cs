using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть марку")]
        [StringLength(50, ErrorMessage = "Марка не може перевищувати 50 символів")]
        [Display(Name = "Марка")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть модель")]
        [StringLength(50, ErrorMessage = "Модель не може перевищувати 50 символів")]
        [Display(Name = "Модель")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номерний знак")]
        [RegularExpression(@"^[A-Z]{2}\d{4}[A-Z]{2}$", ErrorMessage = "Невірний формат (AA1234BB)")]
        [Display(Name = "Номерний знак")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть колір")]
        [StringLength(30, ErrorMessage = "Колір не може перевищувати 30 символів")]
        [Display(Name = "Колір")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть рік")]
        [Range(1990, 2025, ErrorMessage = "Рік має бути від 1990 до 2025")]
        [Display(Name = "Рік випуску")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Оберіть тип кузова")]
        [Display(Name = "Тип кузова")]
        public string BodyType { get; set; } = string.Empty;

        [Display(Name = "Доступний")]
        public bool IsAvailable { get; set; } = true;

        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    }
}
