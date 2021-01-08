using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class AnnouncementAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public Announcement Announcement { get; set; }

        public CourseEntity Course { get; set; }

        public ClassRun ClassRun { get; set; }

        public List<Registration> Participants { get; set; }

        public Dictionary<Guid, CourseUser> ParticipantUsersDic { get; set; }

        public static AnnouncementAggregatedEntityModel New(Announcement announcement)
        {
            return new AnnouncementAggregatedEntityModel()
            {
                Announcement = announcement
            };
        }

        public AnnouncementAggregatedEntityModel WithCourse(CourseEntity course)
        {
            Course = course;

            return this;
        }

        public AnnouncementAggregatedEntityModel WithClassRun(ClassRun classRun)
        {
            ClassRun = classRun;

            return this;
        }

        public AnnouncementAggregatedEntityModel WithParticipants(List<Registration> participants)
        {
            Participants = participants;

            return this;
        }

        public AnnouncementAggregatedEntityModel WithParticipantUsers(List<CourseUser> participantUsers)
        {
            ParticipantUsersDic = participantUsers.ToDictionary(p => p.Id, p => p);

            return this;
        }
    }
}
