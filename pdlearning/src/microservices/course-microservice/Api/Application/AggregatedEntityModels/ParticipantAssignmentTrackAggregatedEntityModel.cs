using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class ParticipantAssignmentTrackAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public ParticipantAssignmentTrack ParticipantAssignmentTrack { get; private set; }

        public Assignment Assignment { get; private set; }

        public Registration Registration { get; private set; }

        public CourseEntity Course { get; private set; }

        public ClassRun ClassRun { get; private set; }

        public CourseUser Learner { get; private set; }

        public static ParticipantAssignmentTrackAggregatedEntityModel New()
        {
            return new ParticipantAssignmentTrackAggregatedEntityModel();
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithParticipantAssignmentTrack(ParticipantAssignmentTrack participantAssignmentTrack)
        {
            ParticipantAssignmentTrack = participantAssignmentTrack;
            return this;
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithAssignment(Assignment assignment)
        {
            Assignment = assignment;

            return this;
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithRegistration(Registration registration)
        {
            Registration = registration;

            return this;
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithCourse(CourseEntity course)
        {
            Course = course;

            return this;
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithClassRun(ClassRun classRun)
        {
            ClassRun = classRun;

            return this;
        }

        public ParticipantAssignmentTrackAggregatedEntityModel WithLearner(CourseUser user)
        {
            Learner = user;

            return this;
        }
    }
}
