using cxOrganization.Domain.Entities;
using System;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities.BroadcastMessage;
using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.BroadcastMessage
{
    public class BroadcastMessageDto
    {
        public static BroadcastMessageDto MapToBroadcastMessageDto(BroadcastMessageEntity broadcastMessage)
        {
            if (broadcastMessage is null)
            {
                throw new ArgumentNullException(nameof(broadcastMessage));
            }
            return new BroadcastMessageDto
            {
                BroadcastMessageId = broadcastMessage.BroadcastMessageId,
                BroadcastContent = broadcastMessage.BroadcastContent,
                OwnerId = broadcastMessage.OwnerId,
                Recipients = new Recipient(
                broadcastMessage.Recipient.DepartmentIds,
                broadcastMessage.Recipient.RoleIds,
                broadcastMessage.Recipient.UserIds,
                broadcastMessage.Recipient.GroupIds),
                Status = broadcastMessage.Status,
                Title = broadcastMessage.Title,
                ValidFromDate = broadcastMessage.ValidFromDate,
                ValidToDate = broadcastMessage.ValidToDate,
                CreatedDate = broadcastMessage.CreatedDate,
                SendMode = broadcastMessage.SendMode,
                TargetUserType = broadcastMessage.TargetUserType,
                NumberOfRecurrence = broadcastMessage.NumberOfRecurrence,
                RecurrenceType = broadcastMessage.RecurrenceType,
                DayRepetitions = broadcastMessage.DayRepetitions,
                MonthRepetition = broadcastMessage.MonthRepetition,
                LastUpdatedBy = broadcastMessage.LastUpdatedBy,
                LastUpdated = broadcastMessage.LastUpdated
            };
        }

        public int? BroadcastMessageId { get; set; }
        public string Title { get; set; }
        public string BroadcastContent { get; set; }
        public Recipient Recipients { get; set; }
        public Guid OwnerId { get; set; }
        public SendMode SendMode { get; set; }
        public TargetUserType TargetUserType { get; set; }
        public int? NumberOfRecurrence { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }
        public List<DayRepetition> DayRepetitions { get; set; }
        public MonthRepetition MonthRepetition { get; set; }
        public BroadcastMessageStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastUpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
