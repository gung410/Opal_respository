using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace cxOrganization.Domain.Common
{
    public static class DomainHelper
    {
        public const string MOEDepartmentExtId = "10000658";

        public static object BuildEntityStatusChangedEventInfo(int? oldEntityStatusId, int? oldEntityStatusReasonId, int? newEntityStatusId, int? newEntityStatusReasonId)
        {
            return new
            {
                FromEntityStatusId = oldEntityStatusId,
                ToEntityStatusId = newEntityStatusId,
                FromEntityStatusReasonId = oldEntityStatusReasonId,
                ToEntityStatusReasonId = newEntityStatusReasonId
            };
        }

        public static object BuildMoveDepartmentInfo(IAdvancedWorkContext workContext,
            DepartmentEntity fromDepartmentEntity,
            DepartmentEntity toDepartmentEntity,
            int? updatedBy,
            List<int> movedUserGroupIds = null)
        {
            dynamic BuildDepartmentInfo(DepartmentEntity departmentEntity)
            {
                return new
                {
                    Id = departmentEntity.DepartmentId,
                    ExtId = departmentEntity.ExtId,
                    OwnerId = departmentEntity.OwnerId,
                    CustomerId = departmentEntity.CustomerId ?? workContext.CurrentCustomerId,
                    ArchetypeId = departmentEntity.ArchetypeId ?? 0,
                    Name = departmentEntity.Name
                };
            };

            var moveDepartmentInfo = new
            {
                FromDepartment = BuildDepartmentInfo(fromDepartmentEntity),
                ToDepartment = BuildDepartmentInfo(toDepartmentEntity),
                UpdatedBy = updatedBy,
                MovedUserGroupIds = movedUserGroupIds
            };
            return moveDepartmentInfo;
        }

        public static IdentityDto ToIdentityDto(this DepartmentEntity department)
        {
            if (department == null) return null;
            return new IdentityDto
            {
                Id = department.DepartmentId,
                Archetype = department.ArchetypeId.HasValue ? (ArchetypeEnum)department.ArchetypeId : ArchetypeEnum.Unknown,
                CustomerId = department.CustomerId ?? 0,
                ExtId = department.ExtId,
                OwnerId = department.OwnerId
            };
        }

        public static T GetUserByExtId<T>(IUserService userService, string userExtId, List<EntityStatusEnum> statusIds = null, bool? getRoles = false)
             where T : ConexusBaseDto
        {
            var userPaginatedList = userService.GetUsers<T>(extIds: new List<string>() { userExtId }, statusIds: statusIds, getRoles: getRoles);
            if (userPaginatedList.Items.Count > 0)
            {
                return userPaginatedList.Items.First();
            }

            return default(T);
        }

        public static UserGenericDto GetUserFromWorkContext(IAdvancedWorkContext workContext, IUserService userService,
            bool getRoles = true, bool getLoginServiceClaims = false)
        {
            if (!string.IsNullOrEmpty(workContext.Sub))
            {
                var userFoundByClaim = userService.GetUsers<UserGenericDto>(workContext.CurrentOwnerId,
                    customerIds: new List<int> { workContext.CurrentCustomerId },
                    extIds: new List<string> { workContext.Sub }, getRoles: getRoles, getLoginServiceClaims: getLoginServiceClaims, ignoreCheckReadUserAccess: true).Items.FirstOrDefault();
                return userFoundByClaim;
            }

            return null;
        }

        public static UserEntity GetUserEntityFromWorkContextSub(IAdvancedWorkContext workContext, IUserRepository userRepository, bool includeUserTypes = true)
        {
            if (string.IsNullOrEmpty(workContext.Sub))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, $"WorkContext.Sub is empty.");
            }
            var customerIds = workContext.CurrentCustomerId > 0 ? new List<int> { workContext.CurrentCustomerId } : null;
            var userFoundByClaim = userRepository.GetUsers(workContext.CurrentOwnerId, customerIds,
                     extIds: new List<string> { workContext.Sub }, includeUserTypes: IncludeUserTypeOption.UserType).Items.FirstOrDefault();

            if (userFoundByClaim == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, $"Not found the logged-in user with sub '{workContext.Sub}'.");
            }

            return userFoundByClaim;
        }

        public static List<UserTypeDto> GetAllUserTypes(this UserDtoBase userDto, params ArchetypeEnum[] archetypes)
        {
            var userRoles = new List<UserTypeDto>();
            if (userDto == null) return userRoles;
            if (userDto.Roles != null)
                userRoles.AddRange(userDto.Roles);
            if (userDto.CustomData != null)
            {
                foreach (var customData in userDto.CustomData)
                {
                    if (customData.Value != null && customData.Value is List<UserTypeDto>)
                    {
                        userRoles.AddRange((List<UserTypeDto>)customData.Value);
                    }
                }
            }

            if (archetypes != null && archetypes.Length > 0)
            {
                userRoles = userRoles.Where(r => archetypes.Contains(r.Identity.Archetype)).ToList();
            }

            return userRoles;
        }

        public static List<UserTypeDto> GetAllRoleUserTypes(this UserDtoBase userDto)
        {
            return GetAllUserTypes(userDto, ArchetypeEnum.Role, ArchetypeEnum.SystemRole);
        }

        public static List<UserTypeEntity> GetAllRoleUserTypeEntities(this UserEntity userEntity,
            List<UserTypeEntity> userTypeEntities)
        {
            return userTypeEntities.Where(t =>
                (t.ArchetypeId == (int)ArchetypeEnum.Role || t.ArchetypeId == (int)ArchetypeEnum.SystemRole)
                && userEntity.UT_Us.Any(u => u.UserTypeId == t.UserTypeId)).ToList();
        }

        public static ArchetypeEnum ParseToArchetype(string value)
        {
            if (Enum.TryParse(value, true, out ArchetypeEnum archetype)) return archetype;
            return ArchetypeEnum.Unknown;
        }

        public static LogCommandMessage GenerateCommunicationCommand(string correlationId,
            IUserService userService,
            UserGenericDto executorUser,
            UserGenericDto objectiveUser,
            string emailSubject,
            CommunicationApiTemplate communicationApiTemplate,
            string routingAction,
            SendEmailToDto sendEmailToDto)
        {
            var user = executorUser ?? objectiveUser;

            var executorUserClaims = user.LoginServiceClaims == null
                ? new List<string>()
                : user.LoginServiceClaims.Select(l => l.Value).ToList();

            if (executorUserClaims.Count == 0)
                executorUserClaims.Add(user.Identity.ExtId);

            var senderIdentityId = executorUserClaims.First();
            var customerId = user.Identity.CustomerId;

            var recipient = userService.BuildCommunicationCommandRecipient(executorUser, objectiveUser, sendEmailToDto);
            return GenerateCommunicationCommand(correlationId, customerId, senderIdentityId, emailSubject,
                communicationApiTemplate, routingAction, recipient);
        }

        public static LogCommandMessage GenerateCommunicationCommand(string correlationId,
            int customerId,
            string senderIdentityId,
            string emailSubject,
            CommunicationApiTemplate communicationApiTemplate,
            string routingAction,
            string recipientCxToken,
            string recipientEmail,
            string receipientExtId)
        {
            var recipient = new
            {
                CxToken = recipientCxToken,
                Emails = new List<string> { recipientEmail },
                UserIds = new List<string> { receipientExtId }
            };

            return GenerateCommunicationCommand(
                correlationId: correlationId,
                customerId: customerId,
                senderIdentityId: senderIdentityId,
                emailSubject: emailSubject,
                communicationApiTemplate: communicationApiTemplate,
                routingAction: routingAction,
                recipient: recipient);
        }

        public static LogCommandMessage GenerateCommunicationCommand(string correlationId,
            int customerId,
            string senderIdentityId,
            string emailSubject,
            CommunicationApiTemplate communicationApiTemplate,
            string routingAction,
            dynamic recipient)
        {
            dynamic body = null;
            if (emailSubject == "OPAL2.0 - Change of Email and Username" || emailSubject == "OPAL2.0 - Account Email Change")
            {
                body = new
                {
                    Channel = "Email",
                    DirectMessage = true,
                    Recipient = new
                    {
                        CxToken = recipient.CxToken,
                        Emails = recipient.Emails
                    },
                    Message = new
                    {
                        Subject = emailSubject,
                    },
                    TemplateData = communicationApiTemplate
                };
            }
            else
            {
                body = new
                {
                    Recipient = recipient,
                    Message = new
                    {
                        Subject = emailSubject,
                    },
                    TemplateData = communicationApiTemplate
                };
            }

            return new LogCommandMessage()
            {
                Routing = new LogMessageRouting()
                {
                    Action = routingAction,
                    ActionVersion = "1.0",
                    Entity = "cx-organization-api.domain.user",
                    EntityId = senderIdentityId
                },
                Payload = new LogMessagePayload()
                {
                    References = new LogMessageReferences()
                    {
                        CorrelationId = correlationId,
                    },
                    Identity = new LogMessageIdentity
                    {
                        ClientId = "cx-organization-api",
                        CustomerId = customerId.ToString(),
                        UserId = senderIdentityId
                    },
                    Body = body
                },
                Type = "command",
                Version = "1.0"
            };
        }

        public static LogCommandMessage GenerateBroadcastCommunicationCommand(
            string correlationId,
            string senderIdentityId,
            string subject,
            string displayMessage,
            List<int> departmentIds,
            List<int> groupIds,
            List<int> roleIds,
            List<string> userIds,
            TargetUserType targetUserType,
            SendMode sendMode,
            DateTime validFromDate,
            DateTime validToDate,
            string externalId)
        {
            dynamic recipient = new ExpandoObject();

            if (targetUserType != TargetUserType.AllUser)
            {
                recipient.DepartmentIds = departmentIds.Any() ? departmentIds.ConvertAll(departmentId => departmentId.ToString()) : null;
                recipient.userGroupIds = groupIds.Any() ? groupIds.ConvertAll(groupId => groupId.ToString()) : null;
                recipient.UserTypeIds = roleIds.Any() ? roleIds.ConvertAll(roleId => roleId.ToString()) : null;
                recipient.UserIds = userIds.Any() ? userIds : null;
                recipient.forHrmsUsers = targetUserType == TargetUserType.HRMSUser ? true : (bool?)null;
                recipient.forExternalUsers = targetUserType == TargetUserType.ExternalUser ? true : (bool?)null;
            }    

            return BuildLogCommandMessage(
                recipient,
                correlationId,
                senderIdentityId,
                subject,
                displayMessage,
                targetUserType,
                sendMode,
                validFromDate,
                validToDate,
                externalId);
        }

        /// <summary>
        /// Todo: Too many parameters
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="correlationId"></param>
        /// <param name="senderIdentityId"></param>
        /// <param name="subject"></param>
        /// <param name="displayMessage"></param>
        /// <param name="targetUserType"></param>
        /// <param name="sendMode"></param>
        /// <param name="validFromDate"></param>
        /// <param name="validToDate"></param>
        /// <param name="externalId"></param>
        /// <returns></returns>
        private static LogCommandMessage BuildLogCommandMessage(
            dynamic recipient,
            string correlationId,
            string senderIdentityId,
            string subject,
            string displayMessage,
            TargetUserType targetUserType,
            SendMode sendMode,
            DateTime validFromDate,
            DateTime validToDate,
            string externalId)
        {
            return new LogCommandMessage()
            {
                Routing = new LogMessageRouting()
                {
                    Action = "cx-organization-api.communication.send.message.success",
                    Entity = "cx-organization-api.notification.message",
                    EntityId = senderIdentityId,
                    ActionVersion = string.Empty
                },
                Payload = BuildLogMessagePayload(
                    recipient,
                    correlationId,
                    subject,
                    displayMessage,
                    targetUserType,
                    sendMode,
                    validFromDate,
                    validToDate,
                    externalId,
                    senderIdentityId
                    ),
                Type = "command",
                Version = "1.0"
            };
        }
        public static string GenerateDummyEmail()
        {
            var dummyEmailAddress = Guid.NewGuid().ToString();
            return string.Format("{0}@dummy.net", dummyEmailAddress.Replace("-", ".").Replace("@", "."));
        }
        private static LogMessagePayload BuildLogMessagePayload(
            dynamic recipient,
            string correlationId,
            string subject,
            string displayMessage,
            TargetUserType targetUserType,
            SendMode sendMode,
            DateTime validFromDate,
            DateTime validToDate,
            string externalId,
            string senderIdentityId)
        {
            var payload = new LogMessagePayload()
            {
                References = new LogMessageReferences()
                {
                    CorrelationId = correlationId,
                },
                Identity = new LogMessageIdentity
                {
                    ClientId = "organization_api",
                    CustomerId = string.Empty,
                    UserId = senderIdentityId
                }
            };

            switch (sendMode)
            {
                case SendMode.Banner:
                    payload.Body = new
                    {
                        Recipient = recipient,
                        Message = new
                        {
                            ClientId = "organization_api",
                            ExternalId = externalId,
                            Subject = subject,
                            DisplayMessage = displayMessage,
                            StartDate = validFromDate,
                            EndDate = validToDate,
                            MessageType = "Banner",
                            IsGlobal = targetUserType == TargetUserType.AllUser
                        }
                    };
                    break;
                case SendMode.Email: 
                    payload.Body = new
                    {
                        Recipient = recipient,
                        TemplateData = new
                        {
                            Project = "Opal",
                            Module = "SystemAdmin",
                            TemplateName = "SystemNotification",
                            Data = new
                            {
                                Title = "OPAL 2.0 System notification",
                                Body = displayMessage
                            },
                            ReferenceData = new { }
                        },
                        Channel = "Email",
                        Message = new
                        {
                            ClientId = "organization_api",
                            ExternalId = externalId,
                            Subject = subject,
                            StartDate = validFromDate,
                            EndDate = validToDate,
                            IsGlobal = targetUserType == TargetUserType.AllUser
                        }
                    };
                    break;
                case SendMode.EmailAndBanner:
                    payload.Body = new
                    {
                        Recipient = recipient,
                        TemplateData = new
                        {
                            Project = "Opal",
                            Module = "SystemAdmin",
                            TemplateName = "SystemNotification",
                            Data = new
                            {
                                Title = "OPAL 2.0 System notification",
                                Body = displayMessage
                            },
                            ReferenceData = new { }
                        },
                        Channel = "Email",
                        Message = new
                        {
                            ClientId = "organization_api",
                            ExternalId = externalId,
                            Subject = subject,
                            DisplayMessage = displayMessage,
                            StartDate = validFromDate,
                            EndDate = validToDate,
                            MessageType = "Banner",
                            IsGlobal = targetUserType == TargetUserType.AllUser
                        }
                    };
                    break;
            }

            return payload;
        }

        public static bool IsChangedInfo(List<string> modifiedProperties, params string[] excludedCheckingChangedEntityProperties)
        {
            if (modifiedProperties.IsNullOrEmpty()) return false;

            return excludedCheckingChangedEntityProperties == null ||
                   excludedCheckingChangedEntityProperties.Length == 0
                ? modifiedProperties.Any()
                : modifiedProperties.Except(excludedCheckingChangedEntityProperties).Any();
        }

        public static string MapEntityStatusToUserStatus(EntityStatusEnum entityStatus)
        {
            switch (entityStatus)
            {
                case EntityStatusEnum.New:
                    return "New";
                case EntityStatusEnum.Active:    
                    return "Active";
                case EntityStatusEnum.Deactive:
                    return "Deleted";
                case EntityStatusEnum.Inactive:
                    return "Suspended";
                case EntityStatusEnum.Rejected:
                    return "Rejected";
                case EntityStatusEnum.IdentityServerLocked:
                    return "Locked";
                case EntityStatusEnum.Archived:
                    return "Archived";
                default:
                    return "Unknown";
            }    
        }
    }
}