﻿using System;
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
    public class SchoolMappingService : DepartmentMappingService
    {
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IWorkContext _workContext;
        private readonly List<DepartmentTypeEntity> _departmentTypeEntities;
        public SchoolMappingService(List<int> departmentTypeIds,
            IDepartmentTypeRepository departmentTypeRepository,
            IPropertyService propertyService,ILanguageRepository languageRepository,
            IWorkContext workContext) 
            :base(propertyService, languageRepository, departmentTypeRepository)
        {
            DepartmentTypeIds = departmentTypeIds;
            _departmentTypeRepository = departmentTypeRepository;
            _workContext = workContext;
            _departmentTypeEntities = _departmentTypeRepository.GetAllDepartmentTypes(0, includeLocalizedData: true);
            //Inject services needed
        }
        private List<ArchetypeEnum> DepartmentTypeArcheTypes = new List<ArchetypeEnum> { ArchetypeEnum.Level, ArchetypeEnum.OrganizationalUnitType };
        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department, bool? getDynamicProperties = null)
        {
            int departmentId = base.GetParentDepartmentID(department);
            return this.ToDepartmentDto(department, departmentId, getDynamicProperties);
        }

        public override ConexusBaseDto ToDepartmentDto(DepartmentEntity department,int parentDepartmentId, bool? getDynamicProperties = null) 
        {
            if (department == null || department.ArchetypeId != (short)ArchetypeEnum.School)
            {
                return null;
            }

            var departmentDto = new SchoolDto
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
                //Phone =department.Phone,
                //Email= department.Email,
                ParentDepartmentId = parentDepartmentId
            };
            var departmentId = (int)departmentDto.Identity.Id.Value;
            departmentDto.DynamicAttributes = GetDynamicProperties(departmentId, getDynamicProperties);
            departmentDto.CustomData = new Dictionary<string, object>();
            var departmentTypes = ToDepartmentType(department.DT_Ds.ToList(), _workContext.CurrentLocale);
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
            var departmentDto = (SchoolDto)department;
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
            entity.DynamicAttributes = departmentDto.JsonDynamicAttributes == null ? null : JsonConvert.SerializeObject(departmentDto.JsonDynamicAttributes);
            if (departmentDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                entity.LastSynchronized = departmentDto.EntityStatus.LastExternallySynchronized.Value;
            }
            var receivedDepartmentTypes = new List<DepartmentTypeDto>();
            var currentDepartmentTypes = entity.DT_Ds.Select(x => x.DepartmentTypeId).ToList();
            if (departmentDto.CustomData.Any())
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
                            var departmentType = _departmentTypeEntities.FirstOrDefault(x => x.DepartmentTypeId == (int)item.Identity.Id);
                            entity.DT_Ds.Add(new DTDEntity { DepartmentTypeId = (int)item.Identity.Id, DepartmentId = entity.DepartmentId, DepartmentType = departmentType });
                        }
                    }
                }
                foreach (var item in currentDepartmentTypes)
                {
                    if (!receiveUserTypeIds.Contains(item))
                    {
                        var removeUt = entity.DT_Ds.FirstOrDefault(x => x.DepartmentId == item);
                        entity.DT_Ds.Remove(removeUt);
                    }
                }
            }
            entity.Locked = (short)(departmentDto.EntityStatus.ExternallyMastered ? 1 : 0);
            return entity;
        }

        private DepartmentEntity NewDepartmentEntity(HierarchyDepartmentEntity parentHd, SchoolDto departmentDto)
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
                ArchetypeId = (int)ArchetypeEnum.School,
                H_D = new List<HierarchyDepartmentEntity>(),
                DT_Ds = new List<DTDEntity>(),
                DynamicAttributes = departmentDto.JsonDynamicAttributes == null ? null : JsonConvert.SerializeObject(departmentDto.JsonDynamicAttributes),
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

            var newHd = new HierarchyDepartmentEntity {Path = "", PathName = "", Created = DateTime.Now};
            if (parentHd != null)
            {
                newHd.ParentId = parentHd.HDId;
                newHd.HierarchyId = parentHd.HierarchyId;
            }
            entity.H_D.Add(newHd);

            //Map DepartmentType
            var departmentTypeSchool = DepartmentTypeIds.FirstOrDefault();
            entity.DT_Ds.Add(new DTDEntity { DepartmentTypeId = departmentTypeSchool, DepartmentId = entity.DepartmentId });
            if (departmentDto.CustomData != null && departmentDto.CustomData.Any())
            {
                List<DTDEntity> dtds = new List<DTDEntity>();

                foreach (var item in departmentDto.CustomData)
                {
                    var listDepartmentType = (item.Value as JArray).ToObject<List<DepartmentTypeDto>>();

                    foreach (var dt in listDepartmentType)
                    {
                        var departmentType = _departmentTypeEntities.FirstOrDefault(x => x.DepartmentTypeId == (int)dt.Identity.Id);
                        dtds.Add(new DTDEntity { DepartmentTypeId = (int)dt.Identity.Id, DepartmentType = departmentType });
                    }
                }
                entity.DT_Ds = dtds;
            }
            return entity;
        }
    }
}