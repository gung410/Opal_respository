using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Dtos.DepartmentType;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Mappings
{
    public class DepartmentMappingService : IDepartmentMappingService
    {
        private readonly IPropertyService _propertyService;
        private readonly ILanguageRepository _languageRepository;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private List<LanguageEntity> _languageEntities;
        public DepartmentMappingService(IPropertyService propertyService,
            ILanguageRepository languageRepository,
            IDepartmentTypeRepository departmentTypeRepository)
        {
            _propertyService = propertyService;
            _languageRepository = languageRepository;
            _departmentTypeRepository = departmentTypeRepository;
        }

        public List<int> DepartmentTypeIds
        {
            get; set;
        }

        public virtual ConexusBaseDto ToDepartmentDto(DepartmentEntity department,int parentDepartmentId, bool? getDynamicProperties = null)
        {
            return ToIdentityStatusDto(department);
        }
       
        public virtual DepartmentEntity ToDepartmentEntity(HierarchyDepartmentEntity parentHd, DepartmentEntity entity, DepartmentDtoBase department)
        {
            throw new NotImplementedException();
        }

        public virtual IdentityStatusDto ToIdentityStatusDto(DepartmentEntity department)
        {
            return new IdentityStatusDto
            {
                Identity = ToIdentityDto(department),
                EntityStatus = ToEntityStatusDto(department)
            };
        }

        public virtual MemberDto ToMemberDto(DepartmentEntity department)
        {
            if (!department.ArchetypeId.HasValue)
                return null;
            return new MemberDto
            {
                Identity = ToIdentityDto(department),
                EntityStatus = ToEntityStatusDto(department),
                Role = string.Empty
            };
        }

        protected IdentityDto ToIdentityDto(DepartmentEntity department)
        {
            return new IdentityDto
            {
                Id = department.DepartmentId,
                Archetype = department.ArchetypeId.HasValue ? (ArchetypeEnum)department.ArchetypeId : ArchetypeEnum.Unknown,
                CustomerId = department.CustomerId ?? 0,
                ExtId = department.ExtId,
                OwnerId = department.OwnerId
            };
        }
        protected int GetParentDepartmentID(DepartmentEntity department)
        {
            int parentDepartmentId = 0;
            var parentDepartment = department.H_D.Select(t => t.Parent).FirstOrDefault();
            if (parentDepartment != null)
            {
                parentDepartmentId = parentDepartment.DepartmentId;
            }
            return parentDepartmentId;
        }
        protected EntityStatusDto ToEntityStatusDto(DepartmentEntity department)
        {
            return new EntityStatusDto
            {
                EntityVersion = department.EntityVersion,
                LastUpdated = department.LastUpdated,
                LastUpdatedBy = department.LastUpdatedBy ?? 0,
                StatusId =  (EntityStatusEnum)department.EntityStatusId,
                StatusReasonId = department.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)department.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                LastExternallySynchronized = department.LastSynchronized,
                ExternallyMastered = department.Locked == 1,
                Deleted = department.Deleted.HasValue
            };
        }

        protected void CheckEntityVersion(byte[] clientVersion, byte[] dbVersion)
        {
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(clientVersion, dbVersion))
            {
                throw new Exception(MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED);
            }
        }

        public static short MapEntityStatusEnumToStatus(EntityStatusEnum entityStatus)
        {
            switch (entityStatus)
            {
                case EntityStatusEnum.Active:
                    return 0;
                case EntityStatusEnum.Inactive:
                    return 1;
                case EntityStatusEnum.Deactive:
                    return -10;
                //Default is Inactive
                default:
                    return 1;
            }
        }

        protected short ToStatus(EntityStatusEnum entityStatus)
        {
            return MapEntityStatusEnumToStatus(entityStatus);
        }

        protected EntityStatusEnum ToEntityStatusEnum(short status, int? entityStatusId)
        {
            var entityStatus = EntityStatusEnum.Unknown;
            if (entityStatusId.HasValue)
            {
                entityStatus = (EntityStatusEnum)entityStatusId;
            }
            //Return Active
            if (status == 0 && (entityStatus == EntityStatusEnum.Active || entityStatus == EntityStatusEnum.Unknown))
            {
                return EntityStatusEnum.Active;
            }

            //Return Inactive
            if (status == 1 || entityStatus == EntityStatusEnum.Inactive)
            {
                return EntityStatusEnum.Inactive;
            }

            return EntityStatusEnum.Deactive;
        }

        protected List<EntityKeyValueDto> GetDynamicProperties(int departmentId, bool? getDynamicProperties = null)
        {
            return getDynamicProperties.HasValue && getDynamicProperties.Value ?
                _propertyService.GetDynamicProperties(departmentId, TableTypes.Department) :
                new List<EntityKeyValueDto>();
        }

        public virtual ConexusBaseDto ToDepartmentDto(DepartmentEntity department, bool? getDynamicProperties = null)
        {
            return this.ToIdentityStatusDto(department);
        }

        private List<DepartmentTypeEntity> _departmentTypeEntities;
        protected List<DepartmentTypeEntity> DepartmentTypeEntities
        {
            get
            {
                if (_departmentTypeEntities == null)
                    _departmentTypeEntities = _departmentTypeRepository.GetAllDepartmentTypesInCache();
                return _departmentTypeEntities;
            }
        }
        public virtual List<DepartmentTypeDto> ToDepartmentType(List<DTDEntity> dTDs, string langCode)
        {
            var departmentTypeEntities = DepartmentTypeEntities.Where(t => dTDs.Any(dtd => dtd.DepartmentTypeId == t.DepartmentTypeId)).ToList();
            var result = new List<DepartmentTypeDto>();
            foreach (var item in departmentTypeEntities)
            {
                var departmentTypeDto = new DepartmentTypeDto()
                {
                    Identity = new IdentityDto()
                    {
                        Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                        Id = item.DepartmentTypeId,
                        OwnerId = item.OwnerId,
                        ExtId = item.ExtId
                    },
                    LocalizedData = ToLocalizedDataDto(item.LT_DepartmentType, langCode),
                    EntityStatus = new EntityStatusDto()
                    {
                        StatusId = EntityStatusEnum.Active
                    },
                 
                };
                result.Add(departmentTypeDto);
            }
            return result;
        }
        private List<LanguageEntity> GetLanguageEntities()
        {
            return _languageEntities = _languageEntities ?? _languageRepository.GetLanguages();
        }
        public List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtDepartmentTypeEntity> lTUserTypeEntities, string langCode = "en-US")
        {
            if (!string.IsNullOrEmpty(langCode))
            {
                var language = GetLanguageEntities().FirstOrDefault(x => x.LanguageCode == langCode);
                lTUserTypeEntities = lTUserTypeEntities.Where(x => x.LanguageId == language.LanguageId);
            }
            if (lTUserTypeEntities != null && lTUserTypeEntities.Any())
            {
                var localizedData = new List<LocalizedDataDto>();

                var localizedProperties = typeof(LtDepartmentTypeEntity).GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .ToList();
                var languageEntities = GetLanguageEntities();
                foreach (var ltUserType in lTUserTypeEntities)
                {
                    var languageEntity = languageEntities.FirstOrDefault(l => l.LanguageId == ltUserType.LanguageId);
                    var localizedItem = new LocalizedDataDto()
                    {
                        Id = languageEntity.LanguageId,
                        LanguageCode = languageEntity.LanguageCode,
                        Fields = new List<LocalizedDataFieldDto>()
                    };
                    foreach (var property in localizedProperties)
                    {
                        localizedItem.Fields.Add(new LocalizedDataFieldDto
                        {
                            LocalizedText = Convert.ToString(property.GetValue(ltUserType)),
                            Name = property.Name
                        });
                    }

                    localizedData.Add(localizedItem);
                }
                return localizedData;
            }
            return new List<LocalizedDataDto>();
        }
    }
}
