using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.NewsFeed.Application.Models
{
    public class FeedResultModel : PagedResultDto<object>
    {
        public FeedResultModel(object[] items, int totalCount)
        {
            // Note: items should be object type because by default System.Text.Json can't serialize
            // properties of derived class.
            // Ref: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-properties-of-derived-classes
            Items = items;
            TotalCount = totalCount;
        }
    }
}
