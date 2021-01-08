using System.Threading.Tasks;
using Amazon.ECS.Model;

namespace Microservice.WebinarVideoConverter.Application.Services.Abstractions
{
    public interface IAwsFargateTaskRunnerService
    {
        Task<RunTaskResponse> RunConvertPlaybackTaskAsync(string playbackUrl);
    }
}
