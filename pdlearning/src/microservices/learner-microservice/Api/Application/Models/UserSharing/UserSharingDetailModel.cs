using System;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Models
{
    public class UserSharingDetailModel
    {
        public UserSharingDetailModel(UserSharingDetail entity, bool isFollowing = false)
        {
            Id = entity.Id;
            UserSharingId = entity.UserSharingId;
            UserId = entity.UserId;
            IsFollowing = isFollowing;
        }

        public Guid Id { get; set; }

        public Guid UserSharingId { get; set; }

        public Guid UserId { get; set; }

        public bool IsFollowing { get; set; }
    }
}
