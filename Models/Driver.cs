using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть ім'я водія")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 100 символів")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть телефон")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер ліцензії")]
        [StringLength(50, ErrorMessage = "Номер ліцензії не може перевищувати 50 символів")]
        [Display(Name = "Номер ліцензії")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть досвід")]
        [Range(0, 50, ErrorMessage = "Досвід має бути від 0 до 50 років")]
        [Display(Name = "Досвід (років)")]
        public int Experience { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг має бути від 0 до 5")]
        [Display(Name = "Рейтинг")]
        public decimal Rating { get; set; } = 5.0m;

        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}