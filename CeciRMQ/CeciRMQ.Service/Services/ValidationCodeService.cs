using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Email;
using CeciRMQ.Domain.DTO.ValidationCode;
using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Extensions;
using CeciRMQ.Infra.CrossCutting.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CeciRMQ.Service.Services
{
    public class ValidationCodeService : IValidationCodeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageService<EmailRequestDTO> _messageService;

        public ValidationCodeService(IUnitOfWork uow,
            IHttpContextAccessor httpContextAccessor,
            IMessageService<EmailRequestDTO> messageService)
        {
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _messageService = messageService;
        }

        public async Task<ResultResponse> SendAsync()
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _uow.User
                    .GetUserByIdAsync(Convert.ToInt32(userId));

                var code = PasswordExtension.GeneratePassword(0, 0, 6, 0);
                
                await _uow.ValidationCode.AddAsync(new ValidationCode {
                    Code = PasswordExtension.EncryptPassword(StringHelper.Base64Encode(code)),
                    UserId = user.Id,
                    Expires = System.DateTime.UtcNow.AddMinutes(10)
                });
                await _uow.CommitAsync();

                user.Validated = false;
                _uow.User.Update(user);
                await _uow.CommitAsync();

                response.Message = "Code sent successfully.";

                //rabbit
                _messageService.AddQueueItem(new EmailRequestDTO
                {
                    Body = "A new validation code was requested. Use the code <b>" + code + "</b> to complete validation.",
                    Subject = user.Name,
                    ToEmail = user.Email
                });
            }
            catch (Exception ex)
            {
                response.Message = "Could not send code.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> ValidateCodeAsync(ValidationCodeValidateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var validationCode = await _uow.ValidationCode
                    .GetFirstOrDefaultNoTrackingAsync(x => x.UserId.Equals(Convert.ToInt32(userId)));

                if (validationCode == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = "Invalid or expired validation code.";
                    return response;
                }

                if (!PasswordExtension.DecryptPassword(validationCode.Code).Equals(obj.Code)
                    || validationCode.IsExpired)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = "Invalid or expired validation code.";
                    return response;
                }

                var user = await _uow.User
                    .GetUserByIdAsync(Convert.ToInt32(userId));

                user.Validated = true;
                _uow.User.Update(user);
                await _uow.CommitAsync();

                response.Message = "Code validated successfully.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not validate code.";
                response.Exception = ex;
            }

            return response;
        }
    }
}
