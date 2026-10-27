
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(100)]
        public string UserID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
