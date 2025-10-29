using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        [Required(ErrorMessage = "Оберіть оцінку")]
        [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5")]
        [Display(Name = "Оцінка")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Коментар не може перевищувати 500 символів")]
        [Display(Name = "Коментар")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}