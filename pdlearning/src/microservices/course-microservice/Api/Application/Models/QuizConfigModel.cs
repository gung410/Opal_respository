using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class QuizConfigModel
    {
        public bool ByPassPassingRate { get; set; }

        public bool DisplayPollResultToLearners { get; set; }

        public LectureQuizConfig ToLectureQuizConfig()
        {
            return new LectureQuizConfig
            {
                ByPassPassingRate = ByPassPassingRate,
                DisplayPollResultToLearners = DisplayPollResultToLearners
            };
        }
    }
}
