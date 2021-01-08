using System;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Dtos
{
    public class SaveUserSharingDetailRequestDto
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public UserSharingDetail CreateNewUserSharingDetail(Guid userSharingId, Guid userId)
        {
            return new UserSharingDetail
            {
                Id = Guid.NewGuid(),
                UserSharingId = userSharingId,
                CreatedDate = Clock.Now,
                UserId = userId
            };
        }

        public UserSharingDetail UpdateExistUserSharingDetail(UserSharingDetail currentUserSharingDetail)
        {
            currentUserSharingDetail.ChangedDate = Clock.Now;
            return currentUserSharingDetail;
        }
    }
}
