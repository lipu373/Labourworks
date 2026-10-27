
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Application.DTOs
{
    public class DepartmentMasterDto
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(200, ErrorMessage = "Department name cannot exceed 200 characters")]
        public string DepartmentName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsEditMode => DepartmentId > 0;
    }
}
