using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities.BroadcastMessage;
using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.BroadcastMessage
{
    public class BroadcastMessageCreationDto
    {
        public BroadcastMessageCreationDto(
            string title,
            string broadcastContent,
            Recipient recipients,
            TargetUserType targetUserType,
            SendMode sendMode,
            RecurrenceType recurrenceType,
            int? numberOfRecurrence,
            DateTime validFromDate,
            DateTime validToDate,
            List<DayRepetition> dayRepetitions,
            MonthRepetition monthRepetition,
            BroadcastMessageStatus status
           )
        {
            Title = title;
            BroadcastContent = broadcastContent;
            Recipients = recipients;
            TargetUserType = targetUserType;
            SendMode = sendMode;
            RecurrenceType = recurrenceType;
            NumberOfRecurrence = numberOfRecurrence;
            DayRepetitions = dayRepetitions;
            MonthRepetition = monthRepetition;
            ValidFromDate = validFromDate;
            ValidToDate = validToDate;
            Status = status;
        }

        public SendMode SendMode { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public TargetUserType TargetUserType { get; set; }
        public int? NumberOfRecurrence { get; set; }
        public string Title { get; set; }
        public string BroadcastContent { get; set; }
        public Recipient Recipients { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }
        public List<DayRepetition> DayRepetitions { get; set; }
        public MonthRepetition MonthRepetition { get; set; }
        public BroadcastMessageStatus Status { get; set; }
    }
}