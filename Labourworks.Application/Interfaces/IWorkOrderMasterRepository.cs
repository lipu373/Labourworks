
using Labourworks.Domain.Entities;

namespace Labourworks.Application.Interfaces;

public interface IWorkOrderMasterRepository
{
    Task<List<WorkOrderMaster>> GetAllAsync();

    Task<WorkOrderMaster?> GetByIdAsync(int id);

    Task<bool> WorkOrderNumberExistsAsync(
        string workOrderNumber,
        int? excludeId = null);

    Task AddAsync(WorkOrderMaster workOrder);

    Task UpdateAsync(WorkOrderMaster workOrder);

    Task<bool> DeleteAsync(int id);

    Task SaveChangesAsync();
}
