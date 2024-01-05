using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Infra.Data.Context;
using System.Diagnostics.CodeAnalysis;

namespace CeciRMQ.Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
    }
}
