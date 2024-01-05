using CeciRMQ.Domain.DTO.Auth;
using CeciRMQ.Domain.DTO.Commons;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IAuthService
    {
        Task<ResultResponse<AuthResultDTO>> AuthenticateAsync(LoginDTO model, string ipAddress);
        Task<ResultResponse<AuthResultDTO>> RefreshTokenAsync(string token, string ipAddress);
        Task<ResultResponse> RevokeTokenAsync(string token, string ipAddress);
        Task<ResultResponse> ForgotPasswordAsync(ForgotPasswordDTO model);
    }
}
