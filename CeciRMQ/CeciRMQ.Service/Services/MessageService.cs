using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace CeciRMQ.Service.Services
{
    public class MessageService<T> : IMessageService<T> where T : class
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ConnectionFactory _connectionFactory;

        public MessageService(IOptions<RabbitMqSettings> option)
        {
            _rabbitMqSettings = option.Value;

            _connectionFactory = new ConnectionFactory { 
                HostName = _rabbitMqSettings.Host
            };
        }

        public bool AddQueueItem (T dto)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var chanel = connection.CreateModel())
                    {
                        chanel.QueueDeclare(
                            queue: _rabbitMqSettings.Queue,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var stringFieldMessage = JsonConvert.SerializeObject(dto);
                        var bytesMessage = Encoding.UTF8.GetBytes(stringFieldMessage);

                        chanel.BasicPublish(
                            exchange: "",
                            routingKey: _rabbitMqSettings.Queue,
                            basicProperties: null,
                            body: bytesMessage
                            );
                    }
                }
            }
            catch (System.Exception)
            {
                //add notification
                return false;
            }

            return true;
        }
    }
}
