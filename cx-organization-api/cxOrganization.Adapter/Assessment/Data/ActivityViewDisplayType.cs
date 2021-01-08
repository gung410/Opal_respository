using System;

namespace cxOrganization.Adapter.Assessment.Data
{
    [Flags]
    public enum ActivityViewDisplayType
    {
        None = 0,
        ShowInRegistration = 1,
        ShowInChartView = 1 << 1,
        ShowInExtenalSystem = 1 << 2,
        ShowInChartReport = 1 << 3,
        ShowInDashboard = 1 << 4,
        All = ~None,
        Whatever = -1
    }
}
