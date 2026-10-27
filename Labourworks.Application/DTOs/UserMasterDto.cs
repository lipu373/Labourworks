
using System.ComponentModel.DataAnnotations;
using Labourworks.Domain.Entities;

namespace Labourworks.Application.DTOs
{
    public class UserMasterDto
    {
        public int UserMasterId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [StringLength(100)]
        public string UserID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid role")]
        public int RoleId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid plant")]
        public int PlantId { get; set; }

        public int? ContractorId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Enter a valid mobile number")]
        [StringLength(15)]
        public string? MobileNumber { get; set; }

        [Required]
        [RegularExpression("^(Active|Inactive|Blocked)$",
            ErrorMessage = "Invalid status")]
        public string Status { get; set; } = "Active";

        public bool IsActive { get; set; } = true;

        // Used only when creating or changing a password.
        public string? Password { get; set; }

        // User list displayed on the same page.
        public List<UserMaster> Users { get; set; } = new();

        // Active roles displayed in the Role dropdown.
        public List<RoleMaster> Roles { get; set; } = new();

        public bool IsEditMode => UserMasterId > 0;
    }
}
