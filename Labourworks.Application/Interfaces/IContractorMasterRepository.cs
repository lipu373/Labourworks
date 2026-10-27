
using Labourworks.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labourworks.Application.Interfaces
{
    public interface IContractorMasterRepository
    {
        Task<List<ContractorMasterDto>> GetAllAsync();

        Task<ContractorMasterDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(ContractorMasterDto model);

        Task<bool> UpdateAsync(ContractorMasterDto model);

        Task<bool> DeleteAsync(int id);
    }
}
