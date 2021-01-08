using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateUserSharingCommand : BaseThunderCommand
    {
        public UpdateUserSharingCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public List<SaveUserSharingDetailRequestDto> UsersShared { get; set; } = new List<SaveUserSharingDetailRequestDto>();
    }
}
