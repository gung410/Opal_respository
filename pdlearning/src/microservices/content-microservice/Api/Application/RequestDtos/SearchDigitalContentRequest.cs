using System.Collections.Generic;
using Microservice.Content.Domain.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.RequestDtos
{
    public class SearchDigitalContentRequest
    {
        public string SearchText { get; set; }

        public bool? IncludeContentForImportToCourse { get; set; }

        public IEnumerable<DigitalContentStatus> FilterByStatus { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }

        public bool? WithinCopyrightDuration { get; set; }

        public DigitalContentQueryMode? QueryMode { get; set; }

        public string SortField { get; set; } = "ChangedDate";

        public SortDirection? SortDirection { get; set; }

        public IEnumerable<string> FilterByExtensions { get; set; }

        public bool? WithinDownloadableContent { get; set; }
    }
}
