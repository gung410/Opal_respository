using System;
using System.Collections.Generic;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Domain.ValueObjects;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Queries
{
    public class SearchPersonalFilesQuery : BaseThunderQuery<PagedResultDto<PersonalFileModel>>
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchText { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<FileType> FilterByType { get; set; }

        public IEnumerable<string> FilterByExtensions { get; set; }

        public SortBy SortBy { get; set; }

        public SortDirection SortDirection { get; set; }
    }
}
