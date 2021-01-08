using System;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    internal class UserListItemEntity
    {

        public int DepartmentId { get; set; }
        public int DepartmentArchetypeId { get; set; }
        public string DepartmentExtId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
        public int UserId { get; set; }
        public string ExtId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ArchetypeId { get; set; }
        public int EntityStatusId { get; set; }
        public int EntityStatusReasonId { get; set; }
        public DateTime? Deleted { get; set; }
        public DateTime? Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime? EntityActiveDate { get; set; }
        public DateTime? EntityExpirationDate { get; set; }
        public DateTime? LastSynchronized { get; set; }
        public string DynamicAttributes { get; set; }
        public short Locked { get; set; }
        public long? NeedResultId { get; set; }
        public string NeedResultExtId { get; set; }
        public DateTime? NeedResultDueDate { get; set; }
        public short NeedResultCompletionRate { get; set; }
        public int NeedStatusTypeId { get; set; }
        public int NeedStatusTypeNo { get; set; }      
        public string NeedStatusTypeCodeName { get; set; }
        public string NeedStatusTypeName { get; set; }
        public string NeedStatusTypeDescription { get; set; }
        public long? PlanResultId { get; set; }
        public string PlanResultExtId { get; set; }
        public DateTime? PlanResultDueDate { get; set; }
        public int PlanStatusTypeId { get; set; }
        public int PlanStatusTypeNo { get; set; }
        public string PlanStatusTypeCodeName { get; set; }
        public string PlanStatusTypeName { get; set; }
        public string PlanStatusTypeDescription { get; set; }
        public string UserTypes { get; set; }
        public string UserGroups { get; set; }


    }
}
