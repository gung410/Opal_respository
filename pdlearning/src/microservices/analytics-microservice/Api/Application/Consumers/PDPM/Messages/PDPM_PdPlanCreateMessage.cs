using System;

namespace Microservice.Analytics.Application.Consumers.PDPM.Messages
{
    public class PDPM_PDPlanCreateMessage
    {
        public PdpmAssignmentResult Result { get; set; }
    }

    public class PdpmAssignmentResult
    {
        public ResultIdentity ResultIdentity { get; set; }

        public AssessmentStatusInfo AssessmentStatusInfo { get; set; }

        public string PdPlanType { get; set; }

        public string PdPlanActivity { get; set; }

        public LastUpdatedBy LastUpdatedBy { get; set; }
    }

    public class ResultIdentity
    {
        public Guid ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; }

        public int Id { get; set; }
    }

    public class ObjectiveInfo
    {
        public string Email { get; set; }

        public string Avatar { get; set; }

        public ObjectiveInfoIdentity Identity { get; set; }

        public string Name { get; set; }
    }

    public class ObjectiveInfoIdentity
    {
        public Guid ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; }

        public int Id { get; set; }
    }

    public class AssessmentStatusInfo
    {
        public int AssessmentStatusId { get; set; }

        public string AssessmentStatusCode { get; set; }

        public string AssessmentStatusName { get; set; }

        public string AssessmentStatusDescription { get; set; }

        public int No { get; set; }
    }

    public class LastUpdatedBy
    {
        public string Email { get; set; }

        public object Avatar { get; set; }

        public LastUpdatedByIdentity Identity { get; set; }

        public string Name { get; set; }

        public object Description { get; set; }
    }

    public class LastUpdatedByIdentity
    {
        public Guid ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; }

        public int Id { get; set; }
    }
}
