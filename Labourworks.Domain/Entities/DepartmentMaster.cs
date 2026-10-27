
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labourworks.Domain.Entities
{
    [Table("DepartmentMaster", Schema = "dbo")]
    public class DepartmentMaster
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(200, ErrorMessage = "Department name cannot exceed 200 characters")]
        public string DepartmentName { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
