using AutoMapper;
using CeciRMQ.Domain.DTO.Address;
using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Email;
using CeciRMQ.Domain.DTO.Register;
using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeciRMQ.Service.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageService<EmailRequestDTO> _messageService;

        public RegisterService(IUnitOfWork uow,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IMessageService<EmailRequestDTO> messageService)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _messageService = messageService;
        }

        public async Task<ResultResponse<UserResultDTO>> GetLoggedInUserAsync()
        {
            var response = new ResultResponse<UserResultDTO>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                response.Data = _mapper.Map<UserResultDTO>(await _uow.User.GetUserByIdAsync(Convert.ToInt32(userId)));
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> SelfRegistrationAsync(UserSelfRegistrationDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var basicProfile = await _uow.Role.GetBasicProfile();

                obj.Password = PasswordExtension.EncryptPassword(obj.Password);

                var newUser = _mapper.Map<User>(obj);
                newUser.RoleId = basicProfile.Id;

                await _uow.User.AddAsync(newUser);
                await _uow.CommitAsync();

                response.Message = "User successfully added.";

                //rabbit
                _messageService.AddQueueItem(new EmailRequestDTO
                {
                    Body = "User successfully added.",
                    Subject = obj.Name,
                    ToEmail = obj.Email
                });

            }
            catch (Exception ex)
            {
                response.Message = "Could not add user.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> UpdateLoggedUserAsync(UserLoggedUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var emailRegistered = await _uow.User
                    .GetFirstOrDefaultNoTrackingAsync(c => c.Email == obj.Email && c.Id != Convert.ToInt32(userId));

                if (emailRegistered != null)
                {
                    return new ResultResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Message = "E-mail already registered"
                    };
                }

                var user = await _uow.User.GetFirstOrDefaultAsync(c => c.Id == Convert.ToInt32(userId));

                user = _mapper.Map(obj, user);

                _uow.User.Update(user);
                await _uow.CommitAsync();

                response.Message = "User successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated user.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> RedefinePasswordAsync(UserRedefinePasswordDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _uow.User
                    .GetFirstOrDefaultAsync(c => c.Id.Equals(Convert.ToInt32(userId)));

                if (!PasswordExtension.DecryptPassword(user.Password).Equals(obj.CurrentPassword))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    response.Message = "Password incorret.";
                    return response;
                };

                user.Password = PasswordExtension.EncryptPassword(obj.NewPassword);

                _uow.User.Update(user);
                await _uow.CommitAsync();

                response.Message = "Password user successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated password user.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> AddLoggedUserAddressAsync(AddressLoggedUserAddDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var addressEntity = _mapper.Map<Address>(obj);
                addressEntity.UserId = Convert.ToInt32(userId);

                await _uow.Address.AddAsync(addressEntity);
                await _uow.CommitAsync();

                response.Message = "Address successfully added.";

            }
            catch (Exception ex)
            {
                response.Message = "Could not add address.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> UpdateLoggedUserAddressAsync(AddressLoggedUserUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var address = await _uow.Address.GetFirstOrDefaultAsync(x => x.UserId == Convert.ToInt32(userId)
                    && x.Id == obj.AddressId);

                if (address == null)
                {
                    return new ResultResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Message = "Address not found."
                    };
                }

                address = _mapper.Map(obj, address);

                _uow.Address.Update(address);
                await _uow.CommitAsync();

                response.Message = "Address successfully updated.";

            }
            catch (Exception ex)
            {
                response.Message = "Could not updated address.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse> InactivateLoggedUserAddressAsync(AddressDeleteDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var address = await _uow.Address.GetFirstOrDefaultAsync(x => x.UserId == Convert.ToInt32(userId)
                    && x.Id == obj.AddressId);

                if (address == null)
                {
                    return new ResultResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Message = "Address not found."
                    };
                }

                address.Active = false;

                _uow.Address.Update(address);
                await _uow.CommitAsync();

                response.Message = "Address successfully deactivated.";

            }
            catch (Exception ex)
            {
                response.Message = "Could not deactivated address.";
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultDataResponse<IEnumerable<AddressResultDTO>>> GetLoggedUserAddressesAsync(AddressFilterDTO filter)
        {
            var response = new ResultDataResponse<IEnumerable<AddressResultDTO>>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                response.Data = _mapper.Map<IEnumerable<AddressResultDTO>>(await _uow.Address.GetLoggedUserAddressesAsync(Convert.ToInt32(userId), filter));
                response.TotalItems = await _uow.Address.GetTotalLoggedUserAddressesAsync(Convert.ToInt32(userId), filter);
                response.TotalPages = (int)Math.Ceiling((double)response.TotalItems / filter.PerPage);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        public async Task<ResultResponse<AddressResultDTO>> GetLoggedUserAddressAsync(AddressIdentifierDTO obj)
        {
            var response = new ResultResponse<AddressResultDTO>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var address = await _uow.Address.GetFirstOrDefaultNoTrackingAsync(x => x.UserId == Convert.ToInt32(userId)
                        && x.Id == obj.AddressId);

                if (address == null)
                {
                    return new ResultResponse<AddressResultDTO>()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                }

                response.Data = _mapper.Map<AddressResultDTO>(address);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }
}
