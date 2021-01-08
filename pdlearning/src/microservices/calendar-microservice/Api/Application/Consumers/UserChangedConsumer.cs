using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Calendar.Application.Consumers.Messages.Enums;
using Microservice.Calendar.Application.Consumers.Messages.Models;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    public class UserChangedConsumer : ScopedOpalMessageConsumer<UserChangedMessage>
    {
        public async Task InternalHandleAsync(
            UserChangedMessage message,
            IRepository<CalendarUser> userRepository)
        {
            var userExisted = await userRepository
                .FirstOrDefaultAsync(u => u.OriginalUserId == message.UserData.Identity.Id && u.Id == message.UserData.Identity.ExtId);

            if (userExisted == null)
            {
                var newUser = new CalendarUser
                {
                    Id = message.UserData.Identity.ExtId,
                    OriginalUserId = message.UserData.Identity.Id,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    Email = message.UserData.EmailAddress,
                    AlternativeApprovalOfficerId = GetApprovalOfficerByType(message.UserData.Groups, UserGroupType.AlternativeApprovalGroup),
                    PrimaryApprovalOfficerId = GetApprovalOfficerByType(message.UserData.Groups, UserGroupType.PrimaryApprovalGroup),
                    Status = Convert.ToInt32(message.UserData.EntityStatus.StatusId)
                };
                await userRepository.InsertAsync(newUser);
                return;
            }

            userExisted.Email = message.UserData.EmailAddress;
            userExisted.FirstName = message.UserData.FirstName;
            userExisted.LastName = message.UserData.LastName;
            userExisted.AlternativeApprovalOfficerId = GetApprovalOfficerByType(message.UserData.Groups, UserGroupType.AlternativeApprovalGroup);
            userExisted.PrimaryApprovalOfficerId = GetApprovalOfficerByType(message.UserData.Groups, UserGroupType.PrimaryApprovalGroup);
            userExisted.Status = Convert.ToInt32(message.UserData.EntityStatus.StatusId);
            await userRepository.UpdateAsync(userExisted);
        }

        private Guid? GetApprovalOfficerByType(List<UserGroupItem> groups, UserGroupType type)
        {
            return groups?.FirstOrDefault(g => g.Type == type)?.UserIdentity.ExtId;
        }
    }
}
