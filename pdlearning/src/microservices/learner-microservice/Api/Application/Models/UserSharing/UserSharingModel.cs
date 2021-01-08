using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class UserSharingModel
    {
        public UserSharingModel(UserSharing entity, IList<UserSharingDetailModel> sharedUsers)
        {
            Id = entity.Id;
            ItemId = entity.ItemId;
            ItemType = entity.ItemType;
            CreatedBy = entity.CreatedBy;
            Users = sharedUsers.ToList();
        }

        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        public SharingType ItemType { get; set; }

        public Guid CreatedBy { get; set; }

        public List<UserSharingDetailModel> Users { get; set; }
    }
}
