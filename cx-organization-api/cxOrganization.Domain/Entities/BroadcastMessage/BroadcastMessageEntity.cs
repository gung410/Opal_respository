using System;
using System.Collections.Generic;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.BroadcastMessage;
using cxPlatform.Core.Entities;

namespace cxOrganization.Domain.Entities.BroadcastMessage
{
    public class BroadcastMessageEntity : ISoftDeleteEntity
    {
        public static BroadcastMessageEntity GenerateRecurringMessage(BroadcastMessageEntity originalBroadcastMessage, DateTime newDisplayDateTime)
        {
            if (originalBroadcastMessage is null)
            {
                throw new ArgumentNullException(nameof(originalBroadcastMessage));
            }

            var recurringMessage = originalBroadcastMessage;
            recurringMessage.ValidFromDate = newDisplayDateTime;
            recurringMessage.ValidToDate = newDisplayDateTime + (originalBroadcastMessage.ValidToDate - originalBroadcastMessage.ValidFromDate);
            recurringMessage.BroadcastMessageId = (new Random()).Next(10000, 100000);

            return recurringMessage;
        }

        public static BroadcastMessageEntity MapToBroadcastMessageEntity(BroadcastMessageCreationDto broadcastMessageCreationDto, Guid ownerId)
        {
            if (broadcastMessageCreationDto is null)
            {
                throw new ArgumentNullException(nameof(broadcastMessageCreationDto));
            }
            return new BroadcastMessageEntity
            {
                Recipient = new Recipient(
                    broadcastMessageCreationDto.Recipients.DepartmentIds,
                    broadcastMessageCreationDto.Recipients.RoleIds,
                    broadcastMessageCreationDto.Recipients.UserIds,
                    broadcastMessageCreationDto.Recipients.GroupIds),
                Status = broadcastMessageCreationDto.Status,
                BroadcastContent = broadcastMessageCreationDto.BroadcastContent,
                ValidFromDate = broadcastMessageCreationDto.ValidFromDate,
                ValidToDate = broadcastMessageCreationDto.ValidToDate,
                OwnerId = ownerId,
                Title = broadcastMessageCreationDto.Title,
                TargetUserType = broadcastMessageCreationDto.TargetUserType,
                NumberOfRecurrence = broadcastMessageCreationDto.NumberOfRecurrence,
                RecurrenceType = broadcastMessageCreationDto.RecurrenceType,
                SendMode = broadcastMessageCreationDto.SendMode,
                MonthRepetition = broadcastMessageCreationDto.MonthRepetition,
                DayRepetitions = broadcastMessageCreationDto.DayRepetitions,
                LastUpdatedBy = ownerId
            };
        }

        public int? BroadcastMessageId { get; set; }

        public string Title { get; set; }

        public string BroadcastContent { get; set; }

        public Recipient Recipient { get; set; }

        public DateTime ValidFromDate { get; set; }

        public DateTime ValidToDate { get; set; }

        public BroadcastMessageStatus Status { get; set; }

        public TargetUserType TargetUserType { get; set; }
        
        public SendMode SendMode { get; set; }

        public RecurrenceType RecurrenceType { get; set; }

        public int? NumberOfRecurrence { get; set; } // Repetition

        public List<DayRepetition> DayRepetitions { get; set; }  

        public MonthRepetition MonthRepetition { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? Deleted { get; set; }

        public Guid OwnerId { get; set; }

        public Guid? LastUpdatedBy { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
 }
