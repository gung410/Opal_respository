using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetNewestAssignedSurveyLinkQuery : BaseThunderQuery<AssignedLinkModel>
    {
        public Guid User { get; set; }
    }
}
