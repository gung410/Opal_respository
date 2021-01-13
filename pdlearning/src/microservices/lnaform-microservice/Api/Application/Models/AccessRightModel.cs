using System;
using Microservice.LnaForm.Domain.Entities;

namespace Microservice.LnaForm.Application.Models
{
    public class AccessRightModel
    {
        public AccessRightModel()
        {
        }

        public AccessRightModel(AccessRight accessRight)
        {
            Id = accessRight.Id;
            UserId = accessRight.UserId;
            ObjectId = accessRight.ObjectId;
            CreatedDate = accessRight.CreatedDate;
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}