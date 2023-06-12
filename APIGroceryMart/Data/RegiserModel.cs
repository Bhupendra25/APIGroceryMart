using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Data
{
    public class RegiserModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string? ConfirmPassword { get; set; }

    }
}
