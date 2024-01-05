using CeciRMQ.Domain.DTO.Address;
using CeciRMQ.Domain.DTO.Commons;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IAddressService
    {
        Task<ResultResponse<AddressResultDTO>> GetAddressByZipCodeAsync(AddressZipCodeDTO obj);
        Task<ResultResponse> AddAsync(AddressAddDTO obj);
        Task<ResultResponse> UpdateAsync(AddressUpdateDTO obj);
        Task<ResultResponse> DeleteAsync(int id);
        Task<ResultDataResponse<IEnumerable<AddressResultDTO>>> GetAsync(AddressFilterDTO filter);
        Task<ResultResponse<AddressResultDTO>> GetByIdAsync(int id);
    }
}
