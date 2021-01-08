using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeLearningMethodRequest
    {
        public Guid Id { get; set; }

        public bool LearningMethod { get; set; }

        public ChangeLearningMethodCommand ToChangeLearningMethodCommand()
        {
            return new ChangeLearningMethodCommand
            {
                Id = Id,
                LearningMethod = LearningMethod
            };
        }
    }
}
