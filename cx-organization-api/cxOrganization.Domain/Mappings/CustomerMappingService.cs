using System;
using cxOrganization.Client.Customers;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public class CustomerMappingService : ICustomerMappingService
    {
        public CustomerMappingService()
        {
        }

        public CustomerDto ToCustomerDto(CustomerEntity customer)
        {
            if (customer == null)
            {
                return null;
            }

            var customerDto = new CustomerDto
            {
                Identity = ToIdentityDto(customer),
                EntityStatus = ToEntityStatusDto(customer),
                Name = customer.Name,
                LogoUrl = customer.Logo,
                //not have infomation yet
                HasUserIntegration = customer.HasUserIntegration,
                RootMenuId = customer.RootMenuId ?? 0,
                CodeName = customer.CodeName,
                Created = customer.Created,
                CssVariables = customer.CssVariables,
                Status = customer.Status,
                LanguageId = customer.LanguageId,
                ExtId = customer.ExtId
            };

            return customerDto;
        }

        private IdentityDto ToIdentityDto(CustomerEntity customer)
        {
            return new IdentityDto
            {
                Id = customer.CustomerId,
                Archetype = ArchetypeEnum.Unknown,
                CustomerId = customer.CustomerId,
                ExtId = customer.ExtId,
                OwnerId = customer.OwnerId
            };
        }

        private EntityStatusDto ToEntityStatusDto(CustomerEntity customer)
        {
            //since Customer entity does not have fields EntityStatusId and EntityStatusReasonId,
            //we need to map these values from field Status which is Active=1 and Inactive=0

            var entityStatus = new EntityStatusDto();
            if (customer.Status == 1) //Active
            {
                entityStatus.StatusId = EntityStatusEnum.Active;
                entityStatus.StatusReasonId = EntityStatusReasonEnum.Active_None;
            }
            else
            {
                entityStatus.StatusId = EntityStatusEnum.Inactive;
                entityStatus.StatusReasonId = EntityStatusReasonEnum.Unknown;
            }
            
            return entityStatus;
        }

        public CustomerEntity ToCustomerEntity(CustomerDto customerDto, CustomerEntity entity)
        {
            //Initial  entity
            if (customerDto.Identity.Id == 0)
            {
                return NewCustomerEntity(customerDto);
            }
            //Entity is existed
            if (entity == null) return null;
            entity.Name = customerDto.Name;
            entity.ExtId = customerDto.Identity.ExtId ?? string.Empty;
            entity.LanguageId = customerDto.LanguageId > 0 ? customerDto.LanguageId : entity.LanguageId;
            entity.RootMenuId = customerDto.RootMenuId == 0 ? (int?)null : customerDto.RootMenuId;
            entity.Logo = customerDto.LogoUrl;
            entity.CodeName = customerDto.CodeName;
            entity.CssVariables = customerDto.CssVariables;
            entity.HasUserIntegration = customerDto.HasUserIntegration;
            entity.Status = customerDto.Status;

            MapStatusFromEntityStatus(customerDto, entity);

            return entity;
        }

        public IdentityStatusDto ToIdentityStatusDto(CustomerEntity customer)
        {
            return new IdentityStatusDto
            {
                Identity = ToIdentityDto(customer),
                EntityStatus = ToEntityStatusDto(customer)
            };
        }

        private CustomerEntity NewCustomerEntity(CustomerDto customerDto)
        {
            var entity = new CustomerEntity
            {
                CustomerId = customerDto.Identity.CustomerId,
                OwnerId = customerDto.Identity.OwnerId,
                ExtId = customerDto.Identity.ExtId ?? string.Empty,
                Name = customerDto.Name,
                LanguageId = customerDto.LanguageId,
                Created = DateTime.Now,
                HasUserIntegration = customerDto.HasUserIntegration,
                RootMenuId = customerDto.RootMenuId == 0 ? (int?)null : customerDto.RootMenuId,
                Logo = customerDto.LogoUrl,
                CodeName = customerDto.CodeName,
                CssVariables = customerDto.CssVariables,
                Status = customerDto.Status,
            };

            MapStatusFromEntityStatus(customerDto, entity);

            return entity;
        }

        private static void MapStatusFromEntityStatus(CustomerDto customerDto, CustomerEntity customerEntity)
        {
            if (customerDto.EntityStatus != null && customerDto.EntityStatus.StatusId != EntityStatusEnum.Unknown)
            {
                customerEntity.Status = (short) (customerDto.EntityStatus.StatusId == EntityStatusEnum.Active ? 1 : 0);
            }
        }
    }
}
