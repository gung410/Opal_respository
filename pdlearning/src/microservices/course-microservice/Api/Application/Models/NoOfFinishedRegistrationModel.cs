using System;

namespace Microservice.Course.Application.Models
{
    public class NoOfFinishedRegistrationModel
    {
        public Guid CourseId { get; set; }

        public int TotalFinishedLearner { get; set; }
    }
}
