
using Labourworks.Domain.Entities;

namespace Labourworks.Application.Interfaces
{
    public interface IUserMasterRepository
    {
        Task<List<UserMaster>> GetAllAsync();

        Task<UserMaster?> GetByIdAsync(int id);

        Task<bool> UserIdExistsAsync(
            string userId,
            int? excludeId = null);

        Task AddAsync(UserMaster user);

        Task UpdateAsync(UserMaster user);

        Task<bool> DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}
