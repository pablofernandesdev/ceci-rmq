using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface ITokenService
    {
        public string GenerateToken(UserResultDTO model);
    }
}
