using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Models;

namespace Microservice.WebinarVideoConverter.Application.Services.Abstractions
{
    public interface IUploaderService
    {
        public Task<UploadResultModel> UploadMeetingRecordAsync(string internalMeetingId);
    }
}
