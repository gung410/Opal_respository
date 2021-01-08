using System;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class SourceDto
    {
        /// <summary>
        /// We use dynamic because it comes from many different sources maybe string or integer.
        /// </summary>
        public dynamic Id { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
