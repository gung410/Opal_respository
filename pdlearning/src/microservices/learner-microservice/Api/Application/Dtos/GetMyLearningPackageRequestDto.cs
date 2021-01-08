using System;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Learner.Application.Dtos
{
    public class GetMyLearningPackageRequestDto
    {
        /// <summary>
        /// Id of LectureInMyCourse. Only use when the Learning Package is inside a Course.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        [FromQuery]
        public Guid? MyLectureId { get; set; }

        /// <summary>
        /// Id of MyDigitalContent. Only use when the Learning Package is inside a standalone DigitalContent.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        [FromQuery]
        public Guid? MyDigitalContentId { get; set; }
    }
}
