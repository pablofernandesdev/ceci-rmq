using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IEmailService
    {
        Task<ResultResponse> SendEmailAsync(EmailRequestDTO emailRequest);
    }
}
