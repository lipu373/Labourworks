
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Infrastructure.Repositories;

public class WorkOrderMasterRepository : IWorkOrderMasterRepository
{
    private readonly ApplicationDbContext _context;

    public WorkOrderMasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkOrderMaster>> GetAllAsync()
    {
        return await _context.WorkOrderMaster
            .AsNoTracking()
            .Where(x => x.DeletedDate == null)
            .OrderByDescending(x => x.WorkOrderId)
            .ToListAsync();
    }

    public async Task<WorkOrderMaster?> GetByIdAsync(int id)
    {
        return await _context.WorkOrderMaster
            .FirstOrDefaultAsync(x =>
                x.WorkOrderId == id &&
                x.DeletedDate == null);
    }

    public async Task<bool> WorkOrderNumberExistsAsync(
        string workOrderNumber,
        int? excludeId = null)
    {
        return await _context.WorkOrderMaster
            .AnyAsync(x =>
                x.WorkOrderNumber == workOrderNumber &&
                x.DeletedDate == null &&
                (!excludeId.HasValue || x.WorkOrderId != excludeId.Value));
    }

    public async Task AddAsync(WorkOrderMaster workOrder)
    {
        await _context.WorkOrderMaster.AddAsync(workOrder);
    }

    public Task UpdateAsync(WorkOrderMaster workOrder)
    {
        _context.WorkOrderMaster.Update(workOrder);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workOrder = await _context.WorkOrderMaster
            .FirstOrDefaultAsync(x =>
                x.WorkOrderId == id &&
                x.DeletedDate == null);

        if (workOrder == null)
            return false;

        workOrder.DeletedDate = DateTime.Now;
        workOrder.IsActive = false;
        workOrder.Status = "Closed";

        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
