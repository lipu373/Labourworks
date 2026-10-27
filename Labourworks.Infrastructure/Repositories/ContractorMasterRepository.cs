
using Labourworks.Application.DTOs;
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Infrastructure.Repositories
{
    public class ContractorMasterRepository : IContractorMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractorMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get All

        public async Task<List<ContractorMasterDto>> GetAllAsync()
        {
            return await _context.ContractorMaster
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.ContractorId)
                .Select(x => new ContractorMasterDto
                {
                    ContractorId = x.ContractorId,
                    ContractorCode = x.ContractorCode,
                    ContractorName = x.ContractorName,
                    ContactPerson = x.ContactPerson,
                    MobileNumber = x.MobileNumber,
                    Email = x.Email,
                    GSTNumber = x.GSTNumber,
                    PANNumber = x.PANNumber,
                    LicenceNumber = x.LicenceNumber,
                    LicenceValidFrom = x.LicenceValidFrom,
                    LicenceValidTo = x.LicenceValidTo,
                    LicenceCapacity = x.LicenceCapacity,
                    DeployedCount = x.DeployedCount,
                    PFEstablishmentCode = x.PFEstablishmentCode,
                    ESICEmployerCode = x.ESICEmployerCode,
                    Address = x.Address,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    DeletedBy = x.DeletedBy,
                    DeletedDate = x.DeletedDate,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        #endregion

        #region Get By Id

        public async Task<ContractorMasterDto?> GetByIdAsync(int id)
        {
            return await _context.ContractorMaster
                .AsNoTracking()
                .Where(x => x.ContractorId == id && x.IsActive)
                .Select(x => new ContractorMasterDto
                {
                    ContractorId = x.ContractorId,
                    ContractorCode = x.ContractorCode,
                    ContractorName = x.ContractorName,
                    ContactPerson = x.ContactPerson,
                    MobileNumber = x.MobileNumber,
                    Email = x.Email,
                    GSTNumber = x.GSTNumber,
                    PANNumber = x.PANNumber,
                    LicenceNumber = x.LicenceNumber,
                    LicenceValidFrom = x.LicenceValidFrom,
                    LicenceValidTo = x.LicenceValidTo,
                    LicenceCapacity = x.LicenceCapacity,
                    DeployedCount = x.DeployedCount,
                    PFEstablishmentCode = x.PFEstablishmentCode,
                    ESICEmployerCode = x.ESICEmployerCode,
                    Address = x.Address,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    DeletedBy = x.DeletedBy,
                    DeletedDate = x.DeletedDate,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Add

        public async Task<bool> AddAsync(ContractorMasterDto model)
        {
            if (!model.LicenceValidFrom.HasValue ||
                !model.LicenceValidTo.HasValue)
            {
                return false;
            }

            var entity = new ContractorMaster
            {
                ContractorCode = model.ContractorCode,
                ContractorName = model.ContractorName,
                ContactPerson = model.ContactPerson,
                MobileNumber = model.MobileNumber,
                Email = model.Email,
                GSTNumber = model.GSTNumber,
                PANNumber = model.PANNumber,
                LicenceNumber = model.LicenceNumber,
                LicenceValidFrom = model.LicenceValidFrom.Value,
                LicenceValidTo = model.LicenceValidTo.Value,
                LicenceCapacity = model.LicenceCapacity,
                DeployedCount = model.DeployedCount ?? 0,
                PFEstablishmentCode = model.PFEstablishmentCode,
                ESICEmployerCode = model.ESICEmployerCode,
                Address = model.Address,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _context.ContractorMaster.AddAsync(entity);

            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Update

        public async Task<bool> UpdateAsync(ContractorMasterDto model)
        {
            if (!model.LicenceValidFrom.HasValue ||
                !model.LicenceValidTo.HasValue)
            {
                return false;
            }

            var entity = await _context.ContractorMaster
                .FirstOrDefaultAsync(x =>
                    x.ContractorId == model.ContractorId &&
                    x.IsActive);

            if (entity == null)
                return false;

            entity.ContractorName = model.ContractorName;
            entity.ContactPerson = model.ContactPerson;
            entity.MobileNumber = model.MobileNumber;
            entity.Email = model.Email;
            entity.GSTNumber = model.GSTNumber;
            entity.PANNumber = model.PANNumber;
            entity.LicenceNumber = model.LicenceNumber;
            entity.LicenceValidFrom = model.LicenceValidFrom.Value;
            entity.LicenceValidTo = model.LicenceValidTo.Value;
            entity.LicenceCapacity = model.LicenceCapacity;
            entity.DeployedCount = model.DeployedCount ?? 0;
            entity.PFEstablishmentCode = model.PFEstablishmentCode;
            entity.ESICEmployerCode = model.ESICEmployerCode;
            entity.Address = model.Address;
            entity.Status = model.Status;
            entity.UpdatedBy = model.UpdatedBy;
            entity.UpdatedDate = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Delete (Soft Delete)

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ContractorMaster
                .FirstOrDefaultAsync(x =>
                    x.ContractorId == id &&
                    x.IsActive);

            if (entity == null)
                return false;

            entity.IsActive = false;
            entity.DeletedDate = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
