using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class SaveUserSharingRequestDto
    {
        public Guid? Id { get; set; }

        public Guid ItemId { get; set; }

        public SharingType ItemType { get; set; }

        public List<SaveUserSharingDetailRequestDto> UsersShared { get; set; } = new List<SaveUserSharingDetailRequestDto>();
    }
}
