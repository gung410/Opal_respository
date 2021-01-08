using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class SearchAccessRightRequest
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
