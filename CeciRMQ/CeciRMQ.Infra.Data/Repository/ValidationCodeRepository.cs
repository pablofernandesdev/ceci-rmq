using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Infra.Data.Context;
using System.Diagnostics.CodeAnalysis;

namespace CeciRMQ.Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class ValidationCodeRepository : BaseRepository<ValidationCode>, IValidationCodeRepository
    {
        public ValidationCodeRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
    }
}
