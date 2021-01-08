using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class LectureQuizConfigModel
    {
        public LectureQuizConfigModel()
        {
        }

        public LectureQuizConfigModel(LectureQuizConfig data)
        {
            ByPassPassingRate = data.ByPassPassingRate;
            DisplayPollResultToLearners = data.DisplayPollResultToLearners;
        }

        public bool ByPassPassingRate { get; set; }

        public bool DisplayPollResultToLearners { get; set; }
    }
}
