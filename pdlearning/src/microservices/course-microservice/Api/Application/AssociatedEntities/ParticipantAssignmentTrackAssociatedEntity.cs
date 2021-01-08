using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AssociatedEntities
{
    public class ParticipantAssignmentTrackAssociatedEntity : ParticipantAssignmentTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantAssignmentTrackAssociatedEntity"/> class.
        /// Need to have default constructor for F.Copy.
        /// </summary>
        public ParticipantAssignmentTrackAssociatedEntity()
        {
        }

        public ParticipantAssignmentTrackAssociatedEntity(
            ParticipantAssignmentTrack participantAssignmentTrack,
            Assignment assignment,
            Registration participant)
        {
            F.Copy(participantAssignmentTrack, this);
            Assignment = assignment;
            Participant = participant;
        }

        public Assignment Assignment { get; set; }

        public Registration Participant { get; set; }
    }
}
