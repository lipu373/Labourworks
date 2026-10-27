
using Labourworks.Domain.Entities;

namespace Labourworks.Application.Interfaces
{
    public interface IDepartmentMasterRepository
    {
        Task<List<DepartmentMaster>> GetAllAsync();

        Task<DepartmentMaster?> GetByIdAsync(int id);

        Task<bool> DepartmentNameExistsAsync(
            string departmentName,
            int? excludeId = null);

        Task AddAsync(DepartmentMaster department);

        Task UpdateAsync(DepartmentMaster department);

        Task<bool> DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}
