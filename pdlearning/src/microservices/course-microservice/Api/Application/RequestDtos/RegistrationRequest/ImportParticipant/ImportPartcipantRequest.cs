using System;
using Microsoft.AspNetCore.Http;

namespace Microservice.Course.Application.RequestDtos
{
    public class ImportPartcipantRequest
    {
        public Guid CourseId { get; set; }

        public IFormFile File { get; set; }
    }
}
