
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labourworks.Domain.Entities
{
    [Table("RoleMaster", Schema = "dbo")]
    public class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
