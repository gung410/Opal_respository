using Microsoft.AspNetCore.Http;
using System;

namespace cxOrganization.Domain.RequestDtos.FileRequest
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; }
        public Guid CurrentUserExId { get; set; }
    }
}
