using System.Collections.Generic;
using Microservice.Uploader.Domain.ValueObjects;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class SearchPersonalFilesRequestDto
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchText { get; set; }

        public IEnumerable<FileType> FilterByType { get; set; }

        public IEnumerable<string> FilterByExtensions { get; set; }

        public SortBy SortBy { get; set; }

        public SortDirection SortDirection { get; set; }
    }
}
