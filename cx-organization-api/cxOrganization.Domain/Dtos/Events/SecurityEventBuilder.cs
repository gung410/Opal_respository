using System;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxEvent.Client
{
    public class SecurityEventBuilder
    {
        private readonly SecurityEventDto _dto;

        public SecurityEventBuilder()
        {
            _dto = new SecurityEventDto { ApplicationName = "" };
            _dto.Identity.Archetype = ArchetypeEnum.SecurityEvent;
        }
        /// <summary>
        /// Set AdditionalInformation
        /// </summary>
        /// <param name="additionalInformation"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithAdditionalInformation(object additionalInformation)
        {
            _dto.AdditionalInformation = additionalInformation;
            return this;
        }
        /// <summary>
        /// Set User Identity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public SecurityEventBuilder ForUserIdentity(int userId, ArchetypeEnum archetype)
        {
            _dto.UserIdentity = new IdentityBaseDto
            {
                Id = userId,
                Archetype = archetype
            };

            return this;
        }
        /// <summary>
        /// Set created by
        /// </summary>
        /// <param name="createdByUserId"></param>
        /// <returns></returns>
        public SecurityEventBuilder CreatedBy(int createdByUserId)
        {
            _dto.CreatedBy = createdByUserId;
            return this;
        }
        /// <summary>
        /// Set IPNumber
        /// </summary>
        /// <param name="iPNumber"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithIPNumber(string iPNumber)
        {
            _dto.IPNumber = iPNumber;
            return this;
        }
        /// <summary>
        /// Set CreatedDate
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public SecurityEventBuilder CreatedDate(DateTime createdDate)
        {
            _dto.CreatedDate = createdDate;

            return this;
        }

        /// <summary>
        /// Set department where event belong to
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public SecurityEventBuilder InDepartment(int departmentId, ArchetypeEnum archetype)
        {
            _dto.DepartmentIdentity = new IdentityBaseDto
            {
                Id = departmentId,
                Archetype = archetype
            };

            return this;
        }

        /// <summary>
        /// Set department where event belong to
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public SecurityEventBuilder InGroup(int groupId, ArchetypeEnum archetype)
        {
            _dto.GroupIdentity = new IdentityBaseDto
            {
                Id = groupId,
                Archetype = archetype
            };
            return this;
        }

        /// <summary>
        /// Set department where event belong to
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public SecurityEventBuilder InCustomer(int customerId)
        {
            _dto.Identity.CustomerId = customerId;

            return this;
        }

        /// <summary>
        /// Set owner where event belong to
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public SecurityEventBuilder InOwner(int ownerId)
        {
            _dto.Identity.OwnerId = ownerId;

            return this;
        }

        /// <summary>
        /// Set application name
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithApplicationName(string applicationName)
        {
            _dto.ApplicationName = applicationName;

            return this;
        }

        /// <summary>
        /// Set EventType Name
        /// </summary>
        /// <param name="eventTypeName"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithEventTypeName(string eventTypeName)
        {
            _dto.EventTypeName = eventTypeName;

            return this;
        }

        /// <summary>
        /// Set UserName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithUserName(string userName)
        {
            _dto.UserName = userName;

            return this;
        }

        /// <summary>
        /// Set PasswordType
        /// </summary>
        /// <param name="passwordType"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithPasswordType(string passwordType)
        {
            _dto.PasswordType = passwordType;

            return this;
        }

        /// <summary>
        /// Set LoginServiceId
        /// </summary>
        /// <param name="loginServiceId"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithLoginServiceId(int loginServiceId)
        {
            _dto.LoginServiceId = loginServiceId;

            return this;
        }

        /// <summary>
        /// Set UrlLogon
        /// </summary>
        /// <param name="urlLogon"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithUrlLogon(string urlLogon)
        {
            _dto.UrlLogon = urlLogon;

            return this;
        }

        /// <summary>
        /// Set Reason
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public SecurityEventBuilder WithReason(string reason)
        {
            _dto.Reason = reason;

            return this;
        }

        /// <summary>
        /// Create Dto after map the request information to Dto
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public SecurityEventDto CreateWithRequestContext(IRequestContext requestContext)
        {
            return (SecurityEventDto)BuilderHelper.MapInformationToDto(this._dto, requestContext);
        }

        /// <summary>
        /// Return DTO
        /// </summary>
        /// <returns></returns>
        public SecurityEventDto Create()
        {
            return _dto;
        }
    }
}
