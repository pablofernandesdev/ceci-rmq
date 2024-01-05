using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Import;
using System.Threading.Tasks;

namespace CeciRMQ.Domain.Interfaces.Service
{
    public interface IImportService
    {
        Task<ResultResponse> ImportUsersAsync(FileUploadDTO model);
    }
}
