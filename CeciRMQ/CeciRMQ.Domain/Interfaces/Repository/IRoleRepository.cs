using CeciRMQ.Domain.Entities;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Repository
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetBasicProfile();
    }
}
