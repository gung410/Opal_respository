using System;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxEvent.Client
{
    public class DomainEventBuilder
    {
        private readonly DomainEventDto _dto;

        public DomainEventBuilder()
        {
            _dto = new DomainEventDto { ApplicationName = "" };
            _dto.Identity.Archetype = ArchetypeEnum.DomainEvent;
        }

        /// <summary>
        /// Set User Identity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public DomainEventBuilder ForUserIdentity(int userId, ArchetypeEnum archetype)
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
        public DomainEventBuilder CreatedBy(int createdByUserId)
        {
            _dto.CreatedBy = createdByUserId;
            return this;
        }

        /// <summary>
        /// Set CreatedDate
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public DomainEventBuilder CreatedDate(DateTime createdDate)
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
        public DomainEventBuilder InDepartment(int departmentId, ArchetypeEnum archetype)
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
        public DomainEventBuilder InGroup(int groupId, ArchetypeEnum archetype)
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
        public DomainEventBuilder InCustomer(int customerId)
        {
            _dto.Identity.CustomerId = customerId;

            return this;
        }

        /// <summary>
        /// Set owner where event belong to
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public DomainEventBuilder InOwner(int ownerId)
        {
            _dto.Identity.OwnerId = ownerId;

            return this;
        }

        /// <summary>
        /// Set application name
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public DomainEventBuilder WithApplicationName(string applicationName)
        {
            _dto.ApplicationName = applicationName;

            return this;
        }

        /// <summary>
        /// Set EventType Name
        /// </summary>
        /// <param name="eventTypeName"></param>
        /// <returns></returns>
        public DomainEventBuilder WithEventTypeName(string eventTypeName)
        {
            _dto.EventTypeName = eventTypeName;

            return this;
        }
        /// <summary>
        /// set Additional Information
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public DomainEventBuilder WithAdditionalInformation(object userData)
        {
            _dto.AdditionalInformation = userData;
            return this;
        }
        /// <summary>
        /// set CorrelationId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public DomainEventBuilder WithCorrelationId(string correlationId)
        {
            _dto.CorrelationId = correlationId;
            return this;
        }
        /// <summary>
        /// set ItemId and TableTypeId
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public DomainEventBuilder WithObjectIdentity(long itemId, ArchetypeEnum archetype)
        {
            _dto.ObjectIdentity.Id = itemId;
            _dto.ObjectIdentity.Archetype = archetype;
            return this;
        }
        /// <summary>
        /// Create Dto after map the request information to Dto
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public DomainEventDto CreateWithRequestContext(IRequestContext requestContext)
        {
            return (DomainEventDto)BuilderHelper.MapInformationToDto(this._dto, requestContext);
        }
        /// <summary>
        /// Return DTO
        /// </summary>
        /// <returns></returns>
        public DomainEventDto Create()
        {
            return _dto;
        }
    }
}
