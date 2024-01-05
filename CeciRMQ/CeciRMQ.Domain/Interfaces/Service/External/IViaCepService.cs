using CeciRMQ.Domain.DTO.Address;
using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.ViaCep;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service.External
{
    public interface IViaCepService
    {
        Task<ResultResponse<ViaCepAddressResponseDTO>> GetAddressByZipCodeAsync(string zipCode);
    }
}
