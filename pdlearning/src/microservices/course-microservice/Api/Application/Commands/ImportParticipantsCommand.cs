using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ImportParticipantsCommand : BaseThunderCommand
    {
        public ImportParticipantsCommand(Guid courseId)
        {
            CourseId = courseId;
        }

        public Guid CourseId { get; }

        public List<Registration> ToConfirmRegistrations { get; } = new List<Registration>();

        public List<Registration> ToRejectRegistrations { get; } = new List<Registration>();

        public List<Registration> ToCreateRegistrations { get; } = new List<Registration>();

        public int NumberOfAddedParticipants()
        {
            return ToConfirmRegistrations.Count + ToCreateRegistrations.Count;
        }
    }
}
