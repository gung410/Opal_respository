using System;
using System.Collections.Generic;

namespace Microservice.Content.Application.Models
{
    public enum CourseReferencedContentType
    {
        Form,
        Content
    }

    public class CheckHasReferenceToResourceModel
    {
        public List<Guid> ObjectIds { get; set; }

        public CourseReferencedContentType ContentType { get; set; }
    }
}