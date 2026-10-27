using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labourworks.Infrastructure.Repositories
{
    public class PlantMasterRepository : IPlantMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public PlantMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get All

        public async Task<List<PlantMaster>> GetAllAsync()
        {
            try
            {
                return await _context.PlantMaster
                    .Where(x => x.IsActive == true)
                    .OrderByDescending(x => x.PlantId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Get By Id

        public async Task<PlantMaster?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.PlantMaster
                    .FirstOrDefaultAsync(x => x.PlantId == id && x.IsActive == true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Add

        public async Task<bool> AddAsync(PlantMaster model)
        {
            try
            {

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;

                _context.PlantMaster.Add(model);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Update

        public async Task<bool> UpdateAsync(PlantMaster model)
        {
            try
            {
                var data = await _context.PlantMaster
                    .FirstOrDefaultAsync(x => x.PlantId == model.PlantId);

                if (data == null)
                    return false;


                data.PlantName = model.PlantName;


                data.UpdatedBy = model.UpdatedBy;
                data.UpdatedDate = DateTime.Now;

                _context.PlantMaster.Update(data);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Delete (Soft Delete)

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var data = await _context.PlantMaster.FirstOrDefaultAsync(x => x.PlantId == id);

                if (data == null)
                    return false;
                data.DeletedDate = DateTime.Now;
                data.IsActive = false;

                _context.PlantMaster.Update(data);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
