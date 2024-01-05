using CeciRMQ.Domain.DTO.Address;
using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Register;
using CeciRMQ.Domain.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IRegisterService
    {
        Task<ResultResponse<UserResultDTO>> GetLoggedInUserAsync();
        Task<ResultResponse> SelfRegistrationAsync(UserSelfRegistrationDTO obj);
        Task<ResultResponse> UpdateLoggedUserAsync(UserLoggedUpdateDTO obj);
        Task<ResultResponse> RedefinePasswordAsync(UserRedefinePasswordDTO obj);
        Task<ResultResponse> AddLoggedUserAddressAsync(AddressLoggedUserAddDTO obj);
        Task<ResultResponse> UpdateLoggedUserAddressAsync(AddressLoggedUserUpdateDTO obj);
        Task<ResultResponse> InactivateLoggedUserAddressAsync(AddressDeleteDTO obj);
        Task<ResultDataResponse<IEnumerable<AddressResultDTO>>> GetLoggedUserAddressesAsync(AddressFilterDTO filter);
        Task<ResultResponse<AddressResultDTO>> GetLoggedUserAddressAsync(AddressIdentifierDTO obj);
    }
}
