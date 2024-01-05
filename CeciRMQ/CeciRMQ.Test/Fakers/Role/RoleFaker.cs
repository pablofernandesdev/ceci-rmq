using Bogus;
using CeciRMQ.Domain.DTO.Role;
using CeciRMQ.Test.Fakers.User;
using System.Collections.Generic;

namespace CeciRMQ.Test.Fakers.Role
{
    public static class RoleFaker
    {
        public static Faker<CeciRMQ.Domain.Entities.Role> RoleEntity()
        {
            return new Faker<CeciRMQ.Domain.Entities.Role>()
                .CustomInstantiator(p => new CeciRMQ.Domain.Entities.Role
                {
                    Active = true,
                    Id = p.Random.Int(1, 2),
                    Name = p.Random.Word(),
                    RegistrationDate = p.Date.Recent(),
                    User = new List<CeciRMQ.Domain.Entities.User>()                   
                });
        }

        public static Faker<RoleAddDTO> RoleAddDTO()
        {
            return new Faker<RoleAddDTO>()
                .CustomInstantiator(p => new RoleAddDTO
                {
                    Name = p.Random.Word(),
                });
        }

        public static Faker<RoleUpdateDTO> RoleUpdateDTO()
        {
            return new Faker<RoleUpdateDTO>()
                .CustomInstantiator(p => new RoleUpdateDTO
                {
                    Name = p.Random.Word(),
                    RoleId = p.Random.Int()
                });
        }

        public static Faker<RoleResultDTO> RoleResultDTO()
        {
            return new Faker<RoleResultDTO>()
                .CustomInstantiator(p => new RoleResultDTO
                {
                    Name = p.Random.Word(),
                    RoleId = p.Random.Int()
                });
        }
    }
}
