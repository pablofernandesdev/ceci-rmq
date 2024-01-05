using Bogus;
using CeciRMQ.Test.Fakers.User;

namespace CeciRMQ.Test.Fakers.RegistrationToken
{
    public class RegistrationTokenFaker
    {
        public static Faker<CeciRMQ.Domain.Entities.RegistrationToken> RegistrationTokenEntity()
        {
            return new Faker<CeciRMQ.Domain.Entities.RegistrationToken>()
                .CustomInstantiator(p => new CeciRMQ.Domain.Entities.RegistrationToken
                {
                    Active = true,
                    UserId = p.Random.Int(),
                    Id = p.Random.Int(),
                    RegistrationDate = p.Date.Recent(),
                    Token = p.Random.String2(30),
                    User = UserFaker.UserEntity().Generate()
                });
        }
    }
}
