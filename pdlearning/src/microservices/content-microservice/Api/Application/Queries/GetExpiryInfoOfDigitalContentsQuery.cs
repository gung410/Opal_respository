using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetExpiryInfoOfDigitalContentsQuery : BaseThunderQuery<DigitalContentExpiryInfoModel[]>
    {
        public Guid[] ListDigitalContentId { get; set; }

        public Guid UserId { get; set; }
    }
}
