using cxOrganization.Domain.Entities;
using System;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities.BroadcastMessage;

namespace cxOrganization.Domain.Dtos.BroadcastMessage
{
    public class BroadcastMessageChangeStatusDto
    {
        public int BroadcastMessageId { get; set; }
        public BroadcastMessageStatus Status { get; set; }
        public Guid? LastUpdatedBy { get; set; }
    }
}
