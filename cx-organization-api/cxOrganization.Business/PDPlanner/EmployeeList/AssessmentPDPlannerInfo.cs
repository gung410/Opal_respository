using System;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class AssessmentPDPlannerInfo
    {
        public IdentityDto Identity { get; set; }
        public StatusTypeInfo StatusInfo { get; set; } 
        public DateTime? DueDate { get; set; }
        public short CompletionRate { get; set; }
    }
}
