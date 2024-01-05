namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IMessageService<T> where T : class
    {
        bool AddQueueItem(T dto);
    }
}
