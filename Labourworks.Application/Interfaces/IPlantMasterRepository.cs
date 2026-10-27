using Labourworks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labourworks.Application.Interfaces
{
    public interface IPlantMasterRepository
    {
        Task<List<PlantMaster>> GetAllAsync();

        Task<PlantMaster?> GetByIdAsync(int id);

        Task<bool> AddAsync(PlantMaster model);

        Task<bool> UpdateAsync(PlantMaster model);

        Task<bool> DeleteAsync(int id);
    }
}
