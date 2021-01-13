using System;

namespace Microservice.Analytics.Domain.ValueObject
{
    [Flags]
    public enum AnalyticSAMArchetype
    {
        None = 0,
        Employee = 1,
        Learner = 2,
        Class = 3,
        School = 4,
        SchoolOwner = 5,
        Level = 6,
        TeachingGroup = 7,
        Assessment = 8,
        Activity = 10,
        Survey = 11,
        Role = 12,
        Partner = 13,
        DataOwner = 14,
        Subject = 15,
        Candidate = 16,
        CandidatePool = 17,
        Company = 18,
        Country = 19,
        CandidateDepartment = 20,
        BusinessEvent = 21,
        SecurityEvent = 22,
        LearningActivityEvent = 23,
        UserEvent = 24,
        AssessmentEvent = 25,
        Team = 26,
        OrganizationalUnit = 27,
        Flag = 28,
        DomainEvent = 29,
        AssessmentAnswer = 30,
        ActivityViewType = 31,
        Category = 32,
        Question = 33,
        Alternative = 34,
        Page = 35,
        PortalPage = 36,
        PortalPart = 37,
        ExternalUser = 38,
    }
}