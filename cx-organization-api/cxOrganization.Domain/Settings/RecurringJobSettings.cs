using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Settings
{
    public class RecurringJobSettings : Dictionary<string, RecurringJobSetting>
    {
        public RecurringJobSettings() : base(StringComparer.CurrentCultureIgnoreCase)
        {

        }

    }

    public class RecurringJobSetting
    {
        public bool Enable { get; set; }
        public int OwnerId { get; set; }
        public int CustomerId { get; set; }
        public string LanguageCode { get; set; }
        public string CronExpression { get; set; }
        public string Queue { get; set; }

    }
}
