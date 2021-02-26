using System;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    /// <summary>
    /// Sort field staff list table
    /// </summary>
    public enum IdpEmployeeListSortField
    {
        FirstName = 0,
        LastName = 1,
        FullName = 2,
        EntityStatus = 3,
        Department = 4,
        [Obsolete("This will be not supported")]
        Group = 5,
        LearningNeedStatusType = 6,
        LearningNeedDueDate = 7,
        LearningPlanStatusType = 8,
        LearningPlanDueDate = 9,
        ApprovalGroup = 10,
        UserPool = 11,
        OtherUserGroup = 12,
        SystemRole = 13,
        Role = 14,
        PersonnelGroup = 15,
        DevelopmentalRole = 16,
        CareerPath = 17,
        ExperienceCategory = 18,
        LNACompletionRate = 19
    }    
}