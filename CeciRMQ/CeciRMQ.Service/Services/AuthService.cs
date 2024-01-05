using AutoMapper;
using CeciRMQ.Domain.DTO.Auth;
using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Email;
using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Extensions;
using CeciRMQ.Infra.CrossCutting.Helper;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CeciRMQ.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMessageService<EmailRequestDTO> _messageService;

        public AuthService(ITokenService tokenService,
            IUnitOfWork uow,
            IMapper mapper,
            IMessageService<EmailRequestDTO> messageService)
        {
            _tokenService = tokenService;
            _uow = uow;
            _mapper = mapper;
            _messageService = messageService;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns>Auth result</returns>
        public async Task<ResultResponse<AuthResultDTO>> AuthenticateAsync(LoginDTO model, string ipAddress)
        {
            var result = new ResultResponse<AuthResultDTO>();

            try
            {
                var user = await _uow.User.GetFirstOrDefaultNoTrackingAsync(x => x.Email.Equals(model.Username));

                if (user != null)
                {
                    if (!PasswordExtension.DecryptPassword(user.Password).Equals(model.Password))
                    {
                        result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                        result.Message = "Password incorret.";
                        return result;
                    };

                    var userValid = await _uow.User.GetUserByIdAsync(user.Id);

                    //authentication successful so generate jwt and refresh tokens
                    var jwtToken = _tokenService.GenerateToken(_mapper.Map<UserResultDTO>(userValid));

                    //generate and save refresh token
                    var refreshToken = GenerateRefreshToken(ipAddress, userValid.Id);
                    await _uow.RefreshToken.AddAsync(refreshToken);
                    await _uow.CommitAsync();

                    result.Data = _mapper.Map<AuthResultDTO>(userValid);
                    result.Data.RefreshToken = refreshToken.Token;
                    result.Data.Token = jwtToken;
                }
                else
                {
                    result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    result.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                result.Message = "Unable to authenticate.";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// Refresh token user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<ResultResponse<AuthResultDTO>> RefreshTokenAsync(string token, string ipAddress)
        {
            var result = new ResultResponse<AuthResultDTO>();

            try
            {
                var atualToken = await _uow.RefreshToken.GetFirstOrDefaultAsync(x => x.Token.Equals(token));
                
                if (atualToken != null && atualToken.IsActive)
                {
                    // generate new refresh token
                    var newRefreshToken = GenerateRefreshToken(ipAddress, atualToken.UserId);

                    // replace old refresh token
                    atualToken.Revoked = DateTime.UtcNow;
                    atualToken.RevokedByIp = ipAddress;
                    atualToken.ReplacedByToken = newRefreshToken.Token;
                    _uow.RefreshToken.Update(atualToken);

                    //save new refresh token
                    await _uow.RefreshToken.AddAsync(newRefreshToken);
                    await _uow.CommitAsync();

                    // generate new jwt token
                    var jwtToken = _tokenService.GenerateToken(_mapper.Map<UserResultDTO>(atualToken.User));

                    result.Data = _mapper.Map<AuthResultDTO>(atualToken.User);
                    result.Data.RefreshToken = newRefreshToken.Token;
                    result.Data.Token = jwtToken;
                }
                else
                {
                    result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    result.Message = "Token not found or expired.";
                }
            }
            catch (Exception ex)
            {
                result.Message = "Unable to refresh token.";
                result.Exception = ex;
            }

            return result;

        }

        /// <summary>
        /// Revoke user token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<ResultResponse> RevokeTokenAsync(string token, string ipAddress)
        {
            var result = new ResultResponse();

            try
            {
                var atualToken = await _uow.RefreshToken.GetFirstOrDefaultAsync(x => x.Token.Equals(token) && x.IsActive);

                if (atualToken != null)
                {
                    atualToken.Revoked = DateTime.UtcNow;
                    atualToken.RevokedByIp = ipAddress;
                    _uow.RefreshToken.Update(atualToken);

                    await _uow.CommitAsync();

                    result.Message = "Token successfully revoked.";
                }
                else
                {
                    result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    result.Message = "Token not found or expired.";
                }
            }
            catch (Exception ex)
            {
                result.Message = "Unable to revoke token.";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// Generates a new password and sends it to the user's email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultResponse> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            var result = new ResultResponse();

            try
            {
                var user = await _uow.User.GetFirstOrDefaultAsync(x => x.Email.Equals(model.Email));

                var newPassword = PasswordExtension.GeneratePassword(2, 2, 2, 2);

                user.Password = PasswordExtension.EncryptPassword(StringHelper.Base64Encode(newPassword));

                _uow.User.Update(user);

                await _uow.CommitAsync();

                //rabbit
                _messageService.AddQueueItem(new EmailRequestDTO
                {
                    Body = "A password change request has been requested for your user. Use the password <b>" + newPassword + "</b> in your next application access.",
                    Subject = user.Name,
                    ToEmail = user.Email
                });

                result.Message = "Request made successfully";

            }
            catch (Exception ex)
            {
                result.Message = "Unable request made.";
                result.Exception = ex;
            }

            return result;
        }

        // helper methods
        private RefreshToken GenerateRefreshToken(string ipAddress, int userId)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    RegistrationDate = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    UserId = userId
                };
            }
        }
    }
}
