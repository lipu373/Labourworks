
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Infrastructure.Repositories
{
    public class DepartmentMasterRepository : IDepartmentMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentMaster>> GetAllAsync()
        {
            return await _context.DepartmentMaster
                .AsNoTracking()
                .Where(d => d.DeletedDate == null)
                .OrderByDescending(d => d.DepartmentId)
                .ToListAsync();
        }

        public async Task<DepartmentMaster?> GetByIdAsync(int id)
        {
            return await _context.DepartmentMaster
                .FirstOrDefaultAsync(d =>
                    d.DepartmentId == id &&
                    d.DeletedDate == null);
        }

        public async Task<bool> DepartmentNameExistsAsync(
            string departmentName,
            int? excludeId = null)
        {
            var normalizedName = departmentName.Trim().ToLower();

            var query = _context.DepartmentMaster
                .Where(d =>
                    d.DeletedDate == null &&
                    d.DepartmentName.ToLower() == normalizedName);

            if (excludeId.HasValue)
            {
                query = query.Where(d =>
                    d.DepartmentId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(DepartmentMaster department)
        {
            await _context.DepartmentMaster.AddAsync(department);
        }

        public Task UpdateAsync(DepartmentMaster department)
        {
            _context.DepartmentMaster.Update(department);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.DepartmentMaster
                .FirstOrDefaultAsync(d =>
                    d.DepartmentId == id &&
                    d.DeletedDate == null);

            if (department == null)
            {
                return false;
            }

            department.DeletedDate = DateTime.Now;
            department.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
