
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Infrastructure.Repositories
{
    public class UserMasterRepository : IUserMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public UserMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all users that have not been deleted.
        public async Task<List<UserMaster>> GetAllAsync()
        {
            return await _context.UserMaster
                .AsNoTracking()
                .Where(u => u.DeletedDate == null)
                .OrderByDescending(u => u.UserMasterId)
                .ToListAsync();
        }

        // Get one user by ID, excluding deleted users.
        public async Task<UserMaster?> GetByIdAsync(int id)
        {
            return await _context.UserMaster
                .FirstOrDefaultAsync(u =>
                    u.UserMasterId == id &&
                    u.DeletedDate == null);
        }

        // Check duplicate User ID.
        public async Task<bool> UserIdExistsAsync(
            string userId,
            int? excludeId = null)
        {
            var query = _context.UserMaster
                .Where(u =>
                    u.UserID == userId &&
                    u.DeletedDate == null);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    u => u.UserMasterId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // Insert user.
        public async Task AddAsync(UserMaster user)
        {
            await _context.UserMaster.AddAsync(user);
        }

        // Update user.
        public Task UpdateAsync(UserMaster user)
        {
            _context.UserMaster.Update(user);
            return Task.CompletedTask;
        }

        // Soft-delete user.
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.UserMaster
                .FirstOrDefaultAsync(u =>
                    u.UserMasterId == id &&
                    u.DeletedDate == null);

            if (user == null)
                return false;

            user.DeletedDate = DateTime.Now;
            user.IsActive = false;
            user.Status = "Inactive";

            await _context.SaveChangesAsync();

            return true;
        }

        // Save changes.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
