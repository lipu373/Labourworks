
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Application.DTOs;

public class WorkOrderMasterDto
{
    public int WorkOrderId { get; set; }

    [Required(ErrorMessage = "Work Order Number is required.")]
    [StringLength(30, ErrorMessage = "Work Order Number cannot exceed 30 characters.")]
    public string WorkOrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a Contractor.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Contractor.")]
    public int ContractorId { get; set; }

    [Required(ErrorMessage = "Please select a Plant.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Plant.")]
    public int PlantId { get; set; }

    [Required(ErrorMessage = "Please select a Department.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Department.")]
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Description of Work is required.")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string DescriptionOfWork { get; set; } = string.Empty;

    [Required(ErrorMessage = "Valid From date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidFrom { get; set; }

    [Required(ErrorMessage = "Valid To date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidTo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Approved Capacity cannot be negative.")]
    public int ApprovedCapacity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Deployed Count cannot be negative.")]
    public int? DeployedCount { get; set; } = 0;

    [Range(typeof(decimal), "0", "9999999999999999.99",
        ErrorMessage = "Rate Per Day cannot be negative.")]
    public decimal RatePerDay { get; set; }

    [Range(typeof(decimal), "0", "999.99",
        ErrorMessage = "Service Charge must be between 0 and 999.99.")]
    public decimal? ServiceCharge { get; set; }

    [Required(ErrorMessage = "Please select a Status.")]
    [RegularExpression("^(Active|Expired|Closed)$",
        ErrorMessage = "Status must be Active, Expired, or Closed.")]
    public string Status { get; set; } = "Active";
}
