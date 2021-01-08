using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Events
{
    public class NotifyInvitationPayload
    {
        private string title;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime? untilDate;
        private bool isAllDay;
        private RepeatFrequency repeatFrequency;

        public NotifyInvitationPayload(
            string title,
            DateTime startDate,
            DateTime endDate,
            DateTime? untilDate,
            bool isAllDay = false,
            RepeatFrequency repeatFrequency = RepeatFrequency.None)
        {
            this.title = title;
            this.startDate = startDate;
            this.endDate = endDate;
            this.untilDate = untilDate;
            this.isAllDay = isAllDay;
            this.repeatFrequency = repeatFrequency;
        }

        public string SubjectContent => BuildContent();

        public string BodyContent => BuildContent();

        public string NotificationContent => BuildContent();

        private string BuildContent()
        {
            if (repeatFrequency == RepeatFrequency.Daily && isAllDay)
            {
                // [Event title] repeat from [Start date] until [End date] (All day)
                return $"{this.title} repeat from {this.startDate:dd/MM/yyyy} until {this.untilDate:dd/MM/yyyy} (All day)";
            }

            if (repeatFrequency == RepeatFrequency.Daily && !isAllDay)
            {
                // [Event title] repeat from [Start date] until [End date] at [Start Time] - [End Time]
                return $"{this.title} repeat from {this.startDate:dd/MM/yyyy} until {this.untilDate:dd/MM/yyyy} at {this.startDate:hh:mm tt} - {this.endDate:hh:mm tt}";
            }

            var isOneDay = string.Equals(this.startDate.ToString("dd/MM/yyyy"), this.endDate.ToString("dd/MM/yyyy"));
            if (repeatFrequency == RepeatFrequency.None && isAllDay && isOneDay)
            {
                // [Event title] on [Start date] (All day)
                return $"{this.title} on {this.startDate:dd/MM/yyyy} (All day)";
            }

            if (repeatFrequency == RepeatFrequency.None && isAllDay && !isOneDay)
            {
                // [Event title] from [Start date] to [End date] (All day)
                return $"{this.title} from {this.startDate:dd/MM/yyyy} to {this.endDate:dd/MM/yyyy} (All day)";
            }

            if (isOneDay)
            {
                // [Event title] on [Start date] at [Start Time] - [End Time]
                return $"{this.title} on {this.startDate:dd/MM/yyyy} at {this.startDate:hh:mm tt} - {this.endDate:hh:mm tt}";
            }

            // [Event title] from [Start date] to [End date] at [Start Time] - [End Time]
            return $"{this.title} from {this.startDate:dd/MM/yyyy} to {this.endDate:dd/MM/yyyy} at {this.startDate:hh:mm tt} - {this.endDate:hh:mm tt}";
        }
    }
}
