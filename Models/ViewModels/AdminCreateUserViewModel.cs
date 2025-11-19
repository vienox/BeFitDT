using System.ComponentModel.DataAnnotations;

namespace BeFit.Models.ViewModels
{
    public class AdminCreateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Adres email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są takie same.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Przyznaj rolę administratora")]
        public bool IsAdmin { get; set; }
    }
}
