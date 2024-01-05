using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetByFilterAsync(UserFilterDTO filter);
        Task<int> GetTotalByFilterAsync(UserFilterDTO filter);
    }
}
