﻿using Bogus;
using CeciRMQ.Domain.DTO.Email;

namespace CeciRMQ.Test.Fakers.Email
{
    public static class EmailFaker
    {
        public static Faker<EmailRequestDTO> EmailRequestDTO()
        {
            return new Faker<EmailRequestDTO>()
                .CustomInstantiator(p => new EmailRequestDTO
                {
                    Body = p.Lorem.Random.Words(3),
                    Subject = p.Random.Words(1),
                    ToEmail = p.Person.Email
                });
        }
    }
}
