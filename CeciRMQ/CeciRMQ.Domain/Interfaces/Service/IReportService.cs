using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IReportService
    {
        Task<ResultResponse<byte[]>> GenerateUsersReport(UserFilterDTO filter);
    }
}
