using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface INotificationService
    {
        Task<ResultResponse> SendAsync(NotificationSendDTO obj);
    }
}
