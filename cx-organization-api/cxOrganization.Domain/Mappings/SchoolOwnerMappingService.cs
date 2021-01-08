using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Mappings
{
    public class SchoolOwnerMappingService : DepartmentMappingService
    {
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private const int DepartmentTypeCounty = 34;
        private const int DepartmentTypeMunicipality = 35;
        private readonly IWorkContext _workContext;

        public SchoolOwnerMappingService(List<int> departmentTypeIds,
            IDepartmentTypeRepository departmentTypeRepository,
            IPropertyService propertyService, ILanguageRepository languageRepository,
            IWorkContext workcontext)
            :base(propertyService, languageRepository, departmentTypeRepository)
        {
            DepartmentTypeIds = departmentTypeIds;
            _departmentTypeRepository = departmentTypeRepository;
            _workContext = workcontext;
        }

        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department, bool? getDynamicProperties = null)
        {
            int departmentId = base.GetParentDepartmentID(department);
            return this.ToDepartmentDto(department, departmentId, getDynamicProperties);
        }

        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department,int parentDepartmentId, bool? getDynamicProperties= null)
        {
            if (department == null || department.ArchetypeId != (short)ArchetypeEnum.SchoolOwner)
            {
                return null;
            }

            var departmentDto = new SchoolOwnerDto
            {
                Identity = ToIdentityDto(department),
                EntityStatus = ToEntityStatusDto(department),
                Name = department.Name,
                Description = string.IsNullOrEmpty(department.Description) ? null : department.Description,
                OrganizationNumber = string.IsNullOrEmpty(department.OrgNo) ? null : department.OrgNo,
                Tag = string.IsNullOrEmpty(department.Tag) ? null : department.Tag,
                LanguageId = department.LanguageId,
                CountryCode = department.CountryCode,
                Address = string.IsNullOrEmpty(department.Adress) ? null : department.Adress,
                PostalCode = string.IsNullOrEmpty(department.PostalCode) ? null : department.PostalCode,
                City = string.IsNullOrEmpty(department.City) ? null : department.City,
                ParentDepartmentId = parentDepartmentId
            };
            //Map DepartmentType
            if (department.DT_Ds != null)
            {
                if (department.DT_Ds.Any(p => p.DepartmentTypeId == DepartmentTypeCounty))
                {
                    departmentDto.DepartmentType = DepartmentTypeEnum.County;
                }
                else if (department.DT_Ds.Any(p => p.DepartmentTypeId == DepartmentTypeMunicipality))
                {
                    departmentDto.DepartmentType = DepartmentTypeEnum.Municipality;
                }
            }

            //Map Propvalues
            var departmentId = (int)departmentDto.Identity.Id.Value;
            departmentDto.DynamicAttributes = GetDynamicProperties(departmentId, getDynamicProperties);

            return departmentDto;
        }

        public override DepartmentEntity ToDepartmentEntity(HierarchyDepartmentEntity parentHd, DepartmentEntity entity, DepartmentDtoBase department)
        {
            var departmentDto = (SchoolOwnerDto)department;
            //Initial  entity
            departmentDto.EntityStatus.LastUpdatedBy = departmentDto.EntityStatus.LastUpdatedBy > 0
                                                             ? departmentDto.EntityStatus.LastUpdatedBy
                                                             : _workContext.CurrentUserId;
            if (departmentDto.Identity.Id == 0)
            {
                return NewDepartmentEntity(parentHd, departmentDto);
            }

            //Entity is existed
            if (entity == null) return null;
            CheckEntityVersion(departmentDto.EntityStatus.EntityVersion, entity.EntityVersion);
            entity.Name = departmentDto.Name;
            entity.Description = departmentDto.Description ?? string.Empty;
            entity.OrgNo = departmentDto.OrganizationNumber ?? string.Empty;
            entity.ExtId = departmentDto.Identity.ExtId ?? string.Empty;
            //entity.OwnerId = departmentDto.Identity.OwnerId;
            //entity.CustomerId = departmentDto.Identity.CustomerId;
            entity.LanguageId = departmentDto.LanguageId > 0 ? departmentDto.LanguageId.Value : entity.LanguageId;
            entity.CountryCode = departmentDto.CountryCode ?? 0;

            entity.Adress = departmentDto.Address ?? string.Empty;
            entity.PostalCode = departmentDto.PostalCode ?? string.Empty;
            entity.City = departmentDto.City ?? string.Empty;
            entity.Tag = departmentDto.Tag ?? string.Empty;
            entity.EntityStatusId = (int)departmentDto.EntityStatus.StatusId;
            entity.EntityStatusReasonId = (int)departmentDto.EntityStatus.StatusReasonId;
            entity.LastUpdated = DateTime.Now;
            entity.LastUpdatedBy = departmentDto.EntityStatus.LastUpdatedBy;
            if (departmentDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                entity.LastSynchronized = departmentDto.EntityStatus.LastExternallySynchronized.Value;
            }
            entity.Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0);
            return entity;
        }

        private DepartmentEntity NewDepartmentEntity(HierarchyDepartmentEntity parentHd, SchoolOwnerDto departmentDto)
        {
            var entity = new DepartmentEntity
            {
                CustomerId = departmentDto.Identity.CustomerId,
                Description = departmentDto.Description ?? string.Empty,
                Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0),
                OrgNo = departmentDto.OrganizationNumber ?? string.Empty,
                OwnerId = departmentDto.Identity.OwnerId,
                ExtId = departmentDto.Identity.ExtId ?? string.Empty,
                Name = departmentDto.Name,
                Tag = departmentDto.Tag ?? string.Empty,
                LanguageId = departmentDto.LanguageId ?? 0,
                CountryCode = departmentDto.CountryCode ?? 0,
                Adress = departmentDto.Address ?? string.Empty,
                PostalCode = departmentDto.PostalCode ?? string.Empty,
                City = departmentDto.City ?? string.Empty,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = departmentDto.EntityStatus.LastUpdatedBy,
                EntityStatusId = (int)departmentDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)departmentDto.EntityStatus.StatusReasonId,
                ArchetypeId = (int)ArchetypeEnum.SchoolOwner,
                H_D = new List<HierarchyDepartmentEntity>()
            };
            if (departmentDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                entity.LastSynchronized = departmentDto.EntityStatus.LastExternallySynchronized.Value;
            }
            entity.Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0);


            //Map H_D
            if (parentHd != null && parentHd.Department != null)
            {
                //Set default customer
                if (!entity.CustomerId.HasValue && parentHd.Department.CustomerId > 0)
                {
                    entity.CustomerId = parentHd.Department.CustomerId;
                }
                //Set default language
                if (entity.LanguageId <= 0)
                {
                    entity.LanguageId = parentHd.Department.LanguageId;
                }
                //Set default country code
                if (entity.CountryCode <= 0)
                {
                    entity.CountryCode = parentHd.Department.CountryCode;
                }
            }

            var newHd = new HierarchyDepartmentEntity { Path = "", PathName = "", Created = DateTime.Now };
            if (parentHd != null)
            {
                newHd.ParentId = parentHd.HDId;
                newHd.HierarchyId = parentHd.HierarchyId;
            }
            entity.H_D.Add(newHd);

            //Map DepartmentType
            if (departmentDto.DepartmentType == DepartmentTypeEnum.County)
            {
                entity.DT_Ds.Add(new DTDEntity { DepartmentId = DepartmentTypeCounty, DepartmentTypeId = DepartmentTypeCounty } );
            }
            else
            {
                entity.DT_Ds.Add(new DTDEntity { DepartmentId = DepartmentTypeCounty, DepartmentTypeId = DepartmentTypeMunicipality });
            }
            

            return entity;
        }
    }
}
