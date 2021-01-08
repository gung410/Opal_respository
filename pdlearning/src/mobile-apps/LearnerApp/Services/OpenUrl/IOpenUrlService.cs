using System;
using System.Threading.Tasks;

namespace LearnerApp.Services.OpenUrl
{
    public interface IOpenUrlService
    {
        Task OpenUrl(string url);

        Task OpenUrl(Uri url);
    }
}
