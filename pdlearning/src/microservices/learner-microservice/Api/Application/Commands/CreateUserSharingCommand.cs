using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CreateUserSharingCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid ItemId { get; set; }

        public SharingType ItemType { get; set; }

        public List<SaveUserSharingDetailRequestDto> UsersShared { get; set; } = new List<SaveUserSharingDetailRequestDto>();

        public bool IsPdo()
        {
            return ItemType == SharingType.Course
                   || ItemType == SharingType.DigitalContent
                   || ItemType == SharingType.Microlearning;
        }

        public bool IsLearningPath()
        {
            return ItemType == SharingType.LearningPath;
        }
    }
}
