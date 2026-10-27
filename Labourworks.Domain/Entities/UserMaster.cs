
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labourworks.Domain.Entities
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Key]
        public int UserMasterId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserID { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public int PlantId { get; set; }

        public int? ContractorId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? MobileNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(255)]
        public string? Password { get; set; }
    }
}
