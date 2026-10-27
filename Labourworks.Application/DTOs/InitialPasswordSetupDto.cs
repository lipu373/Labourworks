
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Application.DTOs
{
    public class InitialPasswordSetupDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(100)]
        public string UserID { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 12,
            ErrorMessage = "Password must be at least 12 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(NewPassword),
            ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
