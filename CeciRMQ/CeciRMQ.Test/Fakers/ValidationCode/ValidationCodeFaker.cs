using Bogus;
using CeciRMQ.Domain.DTO.ValidationCode;
using CeciRMQ.Infra.CrossCutting.Extensions;
using CeciRMQ.Test.Fakers.User;

namespace CeciRMQ.Test.Fakers.ValidationCode
{
    public class ValidationCodeFaker
    {
        public static Faker<CeciRMQ.Domain.Entities.ValidationCode> ValidationCodeEntity()
        {
            return new Faker<CeciRMQ.Domain.Entities.ValidationCode>()
                .CustomInstantiator(p => new CeciRMQ.Domain.Entities.ValidationCode
                {
                    UserId = p.Random.Int(),
                    Active = true,
                    Code = PasswordExtension.EncryptPassword(p.Random.Word()),
                    Expires = p.Date.Future(),
                    Id = p.Random.Int(),
                    RegistrationDate = p.Date.Recent(),
                    User = UserFaker.UserEntity().Generate()               
                });
        }

        public static Faker<ValidationCodeValidateDTO> ValidationCodeValidateDTO()
        {
            return new Faker<ValidationCodeValidateDTO>()
                .CustomInstantiator(p => new ValidationCodeValidateDTO
                {
                    Code = p.Random.Word()
                });
        }
    }
}
