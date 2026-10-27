
using System;
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Application.DTOs
{
    public class ContractorMasterDto
    {
        public int ContractorId { get; set; }

        public string? ContractorCode { get; set; }

        [Required(ErrorMessage = "Contractor name is required.")]
        [StringLength(200)]
        public string ContractorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required.")]
        [StringLength(150)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "GST number is required.")]
        [StringLength(20)]
        public string GSTNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "PAN number is required.")]
        [StringLength(10)]
        public string PANNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Licence number is required.")]
        [StringLength(50)]
        public string LicenceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Licence valid from date is required.")]
        public DateTime? LicenceValidFrom { get; set; }

        [Required(ErrorMessage = "Licence valid to date is required.")]
        public DateTime? LicenceValidTo { get; set; }

        [Required(ErrorMessage = "Licence capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Licence capacity must be greater than zero.")]
        public int LicenceCapacity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Deployed count cannot be negative.")]
        public int? DeployedCount { get; set; } = 0;

        [Required(ErrorMessage = "PF establishment code is required.")]
        [StringLength(50)]
        public string PFEstablishmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "ESIC employer code is required.")]
        [StringLength(50)]
        public string ESICEmployerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
