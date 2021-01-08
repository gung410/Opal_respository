using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public class DepartmentTypeMappingService: IDepartmentTypeMappingService
    {
        private readonly ILanguageRepository _languageRepository;
        private List<LanguageEntity> _languageEntities;
        public DepartmentTypeMappingService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;

        }
        private List<LanguageEntity> GetLanguageEntities()
        {
            return _languageEntities = _languageEntities ?? _languageRepository.GetLanguages();
        }
        public IdentityStatusDto ToIdentityStatusDto(DepartmentTypeEntity departmentTypeEntity)
        {
            return new IdentityStatusDto
            {
                Identity = new IdentityDto
                {
                    Id = departmentTypeEntity.DepartmentTypeId,
                    Archetype = departmentTypeEntity.ArchetypeId.HasValue ? (ArchetypeEnum)departmentTypeEntity.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = departmentTypeEntity.ExtId,
                    OwnerId = departmentTypeEntity.OwnerId
                },
                EntityStatus = new EntityStatusDto
                {
                    StatusId = EntityStatusEnum.Active
                }
            };
        }
        public DepartmentTypeDto ToDepartmentTypeDto(DepartmentTypeEntity departmentTypeEntity, string langCode = "en-US")
        {
            var departmentTypeDto = ToDepartmentTypeDtoInternal(departmentTypeEntity);
            departmentTypeDto.LocalizedData = ToLocalizedDataDto(departmentTypeEntity.LT_DepartmentType, langCode);
            return departmentTypeDto;
        }
        public DepartmentTypeDto ToDepartmentTypeDto(DepartmentTypeEntity departmentTypeEntity, int languageId)
        {
            var departmentTypeDto = ToDepartmentTypeDtoInternal(departmentTypeEntity);
            departmentTypeDto.LocalizedData = ToLocalizedDataDto(departmentTypeEntity.LT_DepartmentType, languageId);
            return departmentTypeDto;
        }
        public List<DepartmentTypeDto> ToDepartmentTypeDtos(List<DepartmentTypeEntity> departmentTypeEntities, string langCode = "en-US")
        {
            if(departmentTypeEntities.IsNullOrEmpty()) return new List<DepartmentTypeDto>();
            return departmentTypeEntities.Select(d => ToDepartmentTypeDto(d, langCode)).ToList();
        }
        public List<DepartmentTypeDto> ToDepartmentTypeDtos(List<DepartmentTypeEntity> departmentTypeEntities, int languageId)
        {
            if (departmentTypeEntities.IsNullOrEmpty()) return new List<DepartmentTypeDto>();
            return departmentTypeEntities.Select(d => ToDepartmentTypeDto(d, languageId)).ToList();

        }
        private static DepartmentTypeDto ToDepartmentTypeDtoInternal(DepartmentTypeEntity departmentTypeEntity)
        {
            var departmentTypeDto = new DepartmentTypeDto()
            {
                Identity = new IdentityDto()
                {
                    Archetype = departmentTypeEntity.ArchetypeId.HasValue
                        ? (ArchetypeEnum) departmentTypeEntity.ArchetypeId
                        : ArchetypeEnum.Unknown,
                    Id = departmentTypeEntity.DepartmentTypeId,
                    OwnerId = departmentTypeEntity.OwnerId,
                    ExtId = departmentTypeEntity.ExtId
                },
                EntityStatus = new EntityStatusDto()
                {
                    StatusId = EntityStatusEnum.Active
                }
            };
            return departmentTypeDto;
        }

        public List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtDepartmentTypeEntity> lTDepartmentTypeEntities, string langCode = "")
        {
            if (!string.IsNullOrEmpty(langCode))
            {
                var language = GetLanguageEntities().FirstOrDefault(x => x.LanguageCode == langCode);
                lTDepartmentTypeEntities = lTDepartmentTypeEntities.Where(x => x.LanguageId == language.LanguageId);
            }
            if (lTDepartmentTypeEntities != null && lTDepartmentTypeEntities.Any())
            {
                return ToLocalizedDataDtos(lTDepartmentTypeEntities);
            }
            return new List<LocalizedDataDto>();
        }

        public List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtDepartmentTypeEntity> lTDepartmentTypeEntities,
            int languageId)
        {
            if (languageId > 0)
            {
                var language = GetLanguageEntities().FirstOrDefault(x => x.LanguageId == languageId);
                lTDepartmentTypeEntities = lTDepartmentTypeEntities.Where(x => x.LanguageId == language.LanguageId);
            }

            if (lTDepartmentTypeEntities != null && lTDepartmentTypeEntities.Any())
            {
                return ToLocalizedDataDtos(lTDepartmentTypeEntities);
            }

            return new List<LocalizedDataDto>();
        }

        private List<LocalizedDataDto> ToLocalizedDataDtos(IEnumerable<LtDepartmentTypeEntity> lTDepartmentTypeEntities)
        {
            var localizedData = new List<LocalizedDataDto>();

            var localizedProperties = typeof(LtDepartmentTypeEntity).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .ToList();
            var languageEntities = GetLanguageEntities();
            foreach (var ltdepartmentType in lTDepartmentTypeEntities)
            {
                var languageEntity = languageEntities.FirstOrDefault(l => l.LanguageId == ltdepartmentType.LanguageId);
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
                        LocalizedText = Convert.ToString(property.GetValue(ltdepartmentType)),
                        Name = property.Name
                    });
                }

                localizedData.Add(localizedItem);
            }

            return localizedData;
        }
    }
}
