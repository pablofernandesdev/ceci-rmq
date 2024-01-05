using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class RegistrationTokenRepository : BaseRepository<RegistrationToken>, IRegistrationTokenRepository
    {
        public RegistrationTokenRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
    }
}
