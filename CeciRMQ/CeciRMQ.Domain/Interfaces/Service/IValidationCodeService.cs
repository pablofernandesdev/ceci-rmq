using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.ValidationCode;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IValidationCodeService
    {
        Task<ResultResponse> SendAsync();
        Task<ResultResponse> ValidateCodeAsync(ValidationCodeValidateDTO obj);
    }
}
