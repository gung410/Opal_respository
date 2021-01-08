using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Settings
{
    public class ChangeUserStatusSettings : Dictionary<string, ChangeStatusPolicy>
    {
        public ChangeUserStatusSettings() : base(StringComparer.CurrentCultureIgnoreCase)
        {

        }

    }

    public class ChangeStatusPolicy
    {
        public int LimitAbsenceHours { get; set; }
    }
}
