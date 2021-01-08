using System;
using System.Threading.Tasks;
using Microservice.Content.Domain.Entities;

namespace Microservice.Content.Application.Services
{
    /// <summary>
    /// To extract URL(s) from a learning content.
    /// </summary>
    public interface IContentUrlExtractor
    {
        Task ExtractAll();

        Task ExtractContentUrl(LearningContent learningContent);

        Task DeleteExtractedUrls(Guid learningContentId);
    }
}
