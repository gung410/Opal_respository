using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Dtos.DepartmentType;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cxOrganization.Domain.Mappings
{
    public class OrganizationalUnitMappingService : DepartmentMappingService
    {
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IWorkContext _workContext;
        private List<ArchetypeEnum> DepartmentTypeArcheTypes = new List<ArchetypeEnum> { ArchetypeEnum.Level, ArchetypeEnum.OrganizationalUnitType };

        public OrganizationalUnitMappingService(IPropertyService propertyService, IDepartmentTypeRepository  departmentTypeRepository,
            IWorkContext workContext, ILanguageRepository languageRepository) 
            : base(propertyService, languageRepository, departmentTypeRepository)
        {
            _departmentTypeRepository = departmentTypeRepository;
            _workContext = workContext;
        }

        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department, bool? getDynamicProperties = null)
        {
            int departmentId = base.GetParentDepartmentID(department);
            return this.ToDepartmentDto(department, departmentId, getDynamicProperties);
        }
        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department, int parentDepartmentId, bool? getDynamicProperties = null)
        {
            if (department == null || department.ArchetypeId != (short)ArchetypeEnum.OrganizationalUnit)
            {
                return null;
            }

            var departmentDto = new OrganizationalUnitDto
            {
                Address=department.Adress,
                City=department.City,
                OrganizationNumber=department.OrgNo,
                PostalCode=department.PostalCode,
                Identity = ToIdentityDto(department),
                EntityStatus = ToEntityStatusDto(department),
                Name = department.Name,
                Description = string.IsNullOrEmpty(department.Description) ? null : department.Description,
                Tag = string.IsNullOrEmpty(department.Tag) ? null : department.Tag,
                LanguageId = department.LanguageId,
                CountryCode = department.CountryCode,
                ParentDepartmentId = parentDepartmentId,
                JsonDynamicAttributes = department.DynamicAttributes == null ? null : JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(department.DynamicAttributes),
            };
            var departmentId = (int)departmentDto.Identity.Id.Value;
            departmentDto.DynamicAttributes = GetDynamicProperties(departmentId, getDynamicProperties);
            var departmentTypes = ToDepartmentType(department.DT_Ds.ToList(), string.IsNullOrEmpty(_workContext.CurrentLocale)
                ? _workContext.FallBackLanguage.LanguageCode : _workContext.CurrentLocale);
            departmentDto.CustomData = new Dictionary<string, object>();
            foreach (var item in DepartmentTypeArcheTypes)
            {
                var userTypeByArcheTypes = departmentTypes.Where(x => x.Identity.Archetype == item).ToList();
                if (userTypeByArcheTypes.Any())
                {
                    var propertyName = item.ToString().ToCharArray();
                    propertyName[0] = Char.ToLower(propertyName[0]);
                    departmentDto.CustomData.Add($"{new string(propertyName)}s", userTypeByArcheTypes);
                }
            }
            return departmentDto;
        }

        public override DepartmentEntity ToDepartmentEntity(HierarchyDepartmentEntity parentHd, DepartmentEntity entity, DepartmentDtoBase department)
        {
            //this.Log("ToDepartmentEntity").Debug(department.ToLogString());

            var departmentDto = (OrganizationalUnitDto)department;
            //Initial entity
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
            entity.Tag = departmentDto.Tag ?? string.Empty;
            entity.Description = departmentDto.Description ?? string.Empty;
            entity.ExtId = departmentDto.Identity.ExtId ?? string.Empty;
            //entity.OwnerId = departmentDto.Identity.OwnerId;
            //entity.CustomerId = departmentDto.Identity.CustomerId;
            entity.LanguageId = departmentDto.LanguageId > 0 ? departmentDto.LanguageId.Value : entity.LanguageId;
            entity.CountryCode = departmentDto.CountryCode ?? 0;
            entity.OrgNo = departmentDto.OrganizationNumber ?? string.Empty;
            entity.Adress = departmentDto.Address ?? string.Empty;
            entity.PostalCode = departmentDto.PostalCode ?? string.Empty;
            entity.City = departmentDto.City ?? string.Empty;
            entity.Tag = departmentDto.Tag ?? string.Empty;
            entity.EntityStatusId = (short)departmentDto.EntityStatus.StatusId;
            entity.EntityStatusId = (int)departmentDto.EntityStatus.StatusId;
            entity.EntityStatusReasonId = (int)departmentDto.EntityStatus.StatusReasonId;
            entity.LastUpdated = DateTime.Now;
            entity.LastUpdatedBy = departmentDto.EntityStatus.LastUpdatedBy;
            entity.DynamicAttributes = departmentDto.JsonDynamicAttributes == null ? null : JsonConvert.SerializeObject(departmentDto.JsonDynamicAttributes);
            if (departmentDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                entity.LastSynchronized = departmentDto.EntityStatus.LastExternallySynchronized.Value;
            }
            var receivedDepartmentTypes = new List<DepartmentTypeDto>();
            var currentDepartmentTypes = entity.DT_Ds.Select(x => x.DepartmentTypeId).ToList();
            if (departmentDto.CustomData != null && departmentDto.CustomData.Any())
            {
                foreach (var item in departmentDto.CustomData)
                {
                    receivedDepartmentTypes.AddRange((item.Value as JArray).ToObject<List<DepartmentTypeDto>>());
                }
                var receiveUserTypeIds = receivedDepartmentTypes.Select(x => (int)x.Identity.Id).ToList();
                if (receivedDepartmentTypes != null)
                {
                    foreach (var item in receivedDepartmentTypes)
                    {
                        if (!currentDepartmentTypes.Contains((int)item.Identity.Id))
                        {
                            entity.DT_Ds.Add(new DTDEntity { DepartmentTypeId = (int)item.Identity.Id, DepartmentId = entity.DepartmentId });
                        }
                    }
                }
                foreach (var item in currentDepartmentTypes)
                {
                    if (!receiveUserTypeIds.Contains(item))
                    {
                        var removeUt = entity.DT_Ds.FirstOrDefault(x => x.DepartmentTypeId == item);
                        entity.DT_Ds.Remove(removeUt);
                    }
                }
            }
            entity.Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0);
            return entity;
        }

        private DepartmentEntity NewDepartmentEntity(HierarchyDepartmentEntity parentHd, OrganizationalUnitDto departmentDto)
        {
            var entity = new DepartmentEntity
            {
                CustomerId = departmentDto.Identity.CustomerId,
                Description = departmentDto.Description ?? string.Empty,
                Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0),
                OrgNo = string.IsNullOrEmpty(departmentDto.OrganizationNumber)? string.Empty:departmentDto.OrganizationNumber,
                OwnerId = departmentDto.Identity.OwnerId,
                ExtId = departmentDto.Identity.ExtId ?? string.Empty,
                Name = departmentDto.Name,
                Tag = departmentDto.Tag ?? string.Empty,
                LanguageId = departmentDto.LanguageId ?? 0,
                CountryCode = departmentDto.CountryCode ?? 0,
                Adress = string.IsNullOrEmpty(departmentDto.Address) ? string.Empty : departmentDto.Address,
                PostalCode = string.IsNullOrEmpty(departmentDto.PostalCode) ? string.Empty : departmentDto.PostalCode,
                City = string.IsNullOrEmpty(departmentDto.City) ? string.Empty : departmentDto.City,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = departmentDto.EntityStatus.LastUpdatedBy,
                EntityStatusId = (int)departmentDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)departmentDto.EntityStatus.StatusReasonId,
                ArchetypeId = (int)ArchetypeEnum.OrganizationalUnit,
                DynamicAttributes = departmentDto.JsonDynamicAttributes == null ? null : JsonConvert.SerializeObject(departmentDto.JsonDynamicAttributes),
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
            return entity;
        }
    }
}
