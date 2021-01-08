#pragma warning disable SA1402 // File may only contain a single type
using System;
using Conexus.Opal.Dtos;
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class LearningNeedsAnalysisChangedMessage
    {
        public ResultLNA Result { get; set; }

        public string PdPlanURL { get; set; }
    }

    public class ResultLNA
    {
        public IdentityInfo ResultIdentity { get; set; }

        public ObjectiveInfoModel ObjectiveInfo { get; set; }

        public DateTime Created { get; set; }

        public DateTime DueDate { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
