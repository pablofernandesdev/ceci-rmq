﻿using CeciRMQ.Domain.DTO.Email;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Extensions;
using CeciRMQ.Service.Services;
using CeciRMQ.Test.Fakers.Email;
using CeciRMQ.Test.Fakers.User;
using CeciRMQ.Test.Fakers.ValidationCode;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CeciRMQ.Test.Services
{
    public class ValidationCodeServiceTest
    {
        private readonly string _claimNameIdentifier = "1";
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IMessageService<EmailRequestDTO>> _mockMessageService;

        public ValidationCodeServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockMessageService = new Mock<IMessageService<EmailRequestDTO>>();

            //http context configuration
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, _claimNameIdentifier),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(user);
        }

        [Fact]
        public async Task Send_new_code_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate();
            var emailRequestDTOFaker = EmailFaker.EmailRequestDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.User
                .GetUserByIdAsync(Convert.ToInt32(userId)))
                .ReturnsAsync(userEntityFaker);

            _mockUnitOfWork.Setup(x => x.ValidationCode
                .AddAsync(validationCodeEntityFaker))
                .ReturnsAsync(validationCodeEntityFaker);

            _mockUnitOfWork.Setup(x => x.User
                 .Update(userEntityFaker));

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.SendAsync();

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Send_new_code_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.User
                .GetUserByIdAsync(Convert.ToInt32(userId)))
                .ThrowsAsync(new Exception());

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.SendAsync();

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Validate_code_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();

            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate();
            validationCodeEntityFaker.Code = PasswordExtension.EncryptPassword(validateCodeValidateDTOFaker.Code);

            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.ValidationCode
                .GetFirstOrDefaultNoTrackingAsync(x => x.UserId.Equals(Convert.ToInt32(userId))))
                .ReturnsAsync(validationCodeEntityFaker);

            _mockUnitOfWork.Setup(x => x.User
                .GetUserByIdAsync(Convert.ToInt32(userId)))
                .ReturnsAsync(userEntityFaker);

            _mockUnitOfWork.Setup(x => x.User
                 .Update(userEntityFaker));

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Validate_code_invalid_code()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate();
            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.ValidationCode
                .GetFirstOrDefaultNoTrackingAsync(x => x.UserId.Equals(Convert.ToInt32(userId))))
                .ReturnsAsync(validationCodeEntityFaker);

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Validate_code_not_found()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.ValidationCode
                .GetFirstOrDefaultNoTrackingAsync(x => x.UserId.Equals(Convert.ToInt32(userId))))
                .ReturnsAsync(value: null);

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Validate_code_exception()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUnitOfWork.Setup(x => x.ValidationCode
                .GetFirstOrDefaultNoTrackingAsync(x => x.UserId.Equals(Convert.ToInt32(userId))))
                .ThrowsAsync(new Exception());

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        private ValidationCodeService ValidationCodeServiceConstrutor()
        {
            return new ValidationCodeService(
                _mockUnitOfWork.Object,
                _mockHttpContextAccessor.Object,
                _mockMessageService.Object);
        }
    }
}
