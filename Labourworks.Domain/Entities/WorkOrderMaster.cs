
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Domain.Entities;

public class WorkOrderMaster
{
    [Key]
    public int WorkOrderId { get; set; }

    public string WorkOrderNumber { get; set; } = string.Empty;

    public int ContractorId { get; set; }

    public int PlantId { get; set; }

    public int DepartmentId { get; set; }

    public string DescriptionOfWork { get; set; } = string.Empty;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public int ApprovedCapacity { get; set; }

    public int? DeployedCount { get; set; } = 0;

    public decimal RatePerDay { get; set; }

    public decimal? ServiceCharge { get; set; }

    public string Status { get; set; } = "Active";

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ContractorMaster? Contractor { get; set; }

    public PlantMaster? Plant { get; set; }

    public DepartmentMaster? Department { get; set; }
}
