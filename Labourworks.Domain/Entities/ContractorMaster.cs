
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labourworks.Domain.Entities
{
    [Table("ContractorMaster", Schema = "dbo")]
    public class ContractorMaster
    {
        [Key]
        public int ContractorId { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? ContractorCode { get; set; }

        [Required]
        [StringLength(200)]
        public string ContractorName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string MobileNumber { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string GSTNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string PANNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LicenceNumber { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime LicenceValidFrom { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime LicenceValidTo { get; set; }

        [Required]
        public int LicenceCapacity { get; set; }

        public int? DeployedCount { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string PFEstablishmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ESICEmployerCode { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
