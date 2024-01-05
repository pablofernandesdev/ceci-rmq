﻿using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Service.Validators.User;
using CeciRMQ.Test.Fakers.Role;
using CeciRMQ.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciRMQ.Test.Validators.User
{
    public class UserAddValidatorTest
    {
        private readonly UserAddValidator _validator;
        private readonly Moq.Mock<IUnitOfWork> _mockUnitOfWork;

        public UserAddValidatorTest()
        {
            _mockUnitOfWork = new Moq.Mock<IUnitOfWork>();
            _validator = new UserAddValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new UserAddDTO { Email = "asdf"};

            _mockUnitOfWork.Setup(x => x.Role.GetFirstOrDefaultAsync(c => c.Id.Equals(model.RoleId)))
                .ReturnsAsync(value: null);

            _mockUnitOfWork.Setup(x => x.User.GetFirstOrDefaultAsync(c => c.Email.Equals(model.Email)))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.RoleId);
            result.ShouldHaveValidationErrorFor(user => user.Name);
            result.ShouldHaveValidationErrorFor(user => user.Email);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = UserFaker.UserAddDTO().Generate();

            _mockUnitOfWork.Setup(x => x.Role.GetFirstOrDefaultAsync(c => c.Id.Equals(model.RoleId)))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            _mockUnitOfWork.Setup(x => x.User.GetFirstOrDefaultAsync(c => c.Email.Equals(model.Email)))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            //result.ShouldNotHaveValidationErrorFor(user => user.RoleId);
            result.ShouldNotHaveValidationErrorFor(user => user.Name);
            //result.ShouldNotHaveValidationErrorFor(user => user.Email);
            result.ShouldNotHaveValidationErrorFor(user => user.Password);
        }
    }
}
