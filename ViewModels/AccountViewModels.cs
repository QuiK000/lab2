using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я")]
        [Display(Name = "Повне ім'я")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [Display(Name = "Номер телефону")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, ErrorMessage = "Пароль повинен містити мінімум {2} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}