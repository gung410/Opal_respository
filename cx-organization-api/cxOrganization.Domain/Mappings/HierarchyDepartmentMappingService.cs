using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Mappings
{
    public class HierarchyDepartmentMappingService : DepartmentMappingService, IHierarchyDepartmentMappingService
    {
        private readonly IAdvancedWorkContext _workContext;

        public IDepartmentTypeRepository _departmentTypeRepository { get; }

        private List<ArchetypeEnum> DepartmentTypeArcheTypes = new List<ArchetypeEnum> { ArchetypeEnum.Level, ArchetypeEnum.OrganizationalUnitType };

        public HierarchyDepartmentMappingService(IPropertyService propertyService,
           IAdvancedWorkContext workContext, ILanguageRepository languageRepository,
           IDepartmentTypeRepository departmentTypeRepository)
           : base(propertyService, languageRepository, departmentTypeRepository)
        {
            _workContext = workContext;
            _departmentTypeRepository = departmentTypeRepository;
        }

        public HierachyDepartmentIdentityDto ToDto(HierarchyDepartmentEntity value, HierarchyDepartmentEntity parentHd = null, int? childrenCount = null, bool getDetailDepartment = true,
            bool includeDepartmentType = false)
        {
            var hierachyDepartmentIdentityDto = new HierachyDepartmentIdentityDto
            {
                ParentDepartmentId = parentHd == null ? 0 : parentHd.DepartmentId,
                Identity = new IdentityDto
                {
                    Id = value.DepartmentId,
                    Archetype = value.Department.ArchetypeId.HasValue ? (ArchetypeEnum)value.Department.ArchetypeId.Value : ArchetypeEnum.Unknown,
                    ExtId = value.Department.ExtId,
                    OwnerId = value.Department.OwnerId,
                    CustomerId = value.Department.CustomerId ?? 0
                },
                DepartmentName = value.Department.Name,
                ChildrenCount = childrenCount,                           
                Path = value.Path,
                PathName = value.PathName,
                EntityStatus = new EntityStatusDto
                {
                    EntityVersion = value.Department.EntityVersion,
                    LastUpdated = value.Department.LastUpdated,
                    LastUpdatedBy = value.Department.LastUpdatedBy ?? 0,
                    StatusId = (EntityStatusEnum)value.Department.EntityStatusId,
                    StatusReasonId = value.Department.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)value.Department.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                    LastExternallySynchronized = value.Department.LastSynchronized,
                    ExternallyMastered = value.Department.Locked == 1,
                    Deleted = value.Deleted != 0
                }
            };

            if (getDetailDepartment)
            {
                hierachyDepartmentIdentityDto.DepartmentDescription = value.Department.Description;
                hierachyDepartmentIdentityDto.Address = value.Department.Adress;
                hierachyDepartmentIdentityDto.CountryCode = value.Department.CountryCode;
                hierachyDepartmentIdentityDto.City = value.Department.City;
                hierachyDepartmentIdentityDto.LanguageId = value.Department.LanguageId;
                hierachyDepartmentIdentityDto.OrganizationNumber = value.Department.OrgNo;
                hierachyDepartmentIdentityDto.PostalCode = value.Department.PostalCode;
                hierachyDepartmentIdentityDto.Tag = value.Department.Tag;
                hierachyDepartmentIdentityDto.JsonDynamicAttributes = value.Department.DynamicAttributes == null ? null : JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(value.Department.DynamicAttributes);
            }
           

            if (includeDepartmentType)
            {
                SetDepartmentTypes(value, hierachyDepartmentIdentityDto);
            }
            return hierachyDepartmentIdentityDto;
        }
        private void SetDepartmentTypes(HierarchyDepartmentEntity hierarchyDepartmentEntity, HierachyDepartmentIdentityDto hierachyDepartmentIdentityDto)
        {
            var departmentTypes = ToDepartmentType(hierarchyDepartmentEntity.Department.DT_Ds.ToList(), string.IsNullOrEmpty(_workContext.CurrentLocale)
                                        ? _workContext.FallBackLanguage.LanguageCode : _workContext.CurrentLocale);
            hierachyDepartmentIdentityDto.CustomData = new Dictionary<string, object>();
            foreach (var item in DepartmentTypeArcheTypes)
            {
                var userTypeByArcheTypes = departmentTypes.Where(x => x.Identity.Archetype == item).ToList();
                if (userTypeByArcheTypes.Any())
                {
                    var propertyName = item.ToString().ToCharArray();
                    propertyName[0] = Char.ToLower(propertyName[0]);
                    hierachyDepartmentIdentityDto.CustomData.Add($"{new string(propertyName)}s", userTypeByArcheTypes);
                }
            }
        }
    }
}
