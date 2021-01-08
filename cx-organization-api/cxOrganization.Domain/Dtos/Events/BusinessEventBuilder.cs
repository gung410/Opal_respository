using System;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxEvent.Client
{
    public class BusinessEventBuilder
    {
        private readonly BusinessEventDto _dto;

        public BusinessEventBuilder()
        {
            _dto = new BusinessEventDto { ApplicationName = ""};
            _dto.Identity.Archetype = ArchetypeEnum.BusinessEvent;
        }
        /// <summary>
        /// Set AdditionalInformation
        /// </summary>
        /// <param name="additionalInformation"></param>
        /// <returns></returns>
        public BusinessEventBuilder WithAdditionalInformation(object additionalInformation)
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
        public BusinessEventBuilder ForUserIdentity(int userId, ArchetypeEnum archetype)
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
        public BusinessEventBuilder CreatedBy(int createdByUserId)
        {
            _dto.CreatedBy = createdByUserId;
            return this;
        }

        /// <summary>
        /// Set CreatedDate
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public BusinessEventBuilder CreatedDate(DateTime createdDate)
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
        public BusinessEventBuilder InDepartment(int departmentId, ArchetypeEnum archetype)
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
        public BusinessEventBuilder InGroup(int groupId, ArchetypeEnum archetype)
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
        public BusinessEventBuilder InCustomer(int customerId)
        {
            _dto.Identity.CustomerId = customerId;

            return this;
        }

        /// <summary>
        /// Set owner where event belong to
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public BusinessEventBuilder InOwner(int ownerId)
        {
            _dto.Identity.OwnerId = ownerId;

            return this;
        }

        /// <summary>
        /// Set application name
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public BusinessEventBuilder WithApplicationName(string applicationName)
        {
            _dto.ApplicationName = applicationName;

            return this;
        }

        /// <summary>
        /// Set EventType Name
        /// </summary>
        /// <param name="eventTypeName"></param>
        /// <returns></returns>
        public BusinessEventBuilder WithEventTypeName(string eventTypeName)
        {
            _dto.EventTypeName = eventTypeName;

            return this;
        }

        /// <summary>
        /// Create Dto after map the request information to Dto
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public BusinessEventDto CreateWithRequestContext(IRequestContext requestContext)
        {
            return (BusinessEventDto)BuilderHelper.MapInformationToDto(this._dto, requestContext);
        }

        /// <summary>
        /// Return DTO
        /// </summary>
        /// <returns></returns>
        public BusinessEventDto Create()
        {
            return _dto;
        }
    }
}
