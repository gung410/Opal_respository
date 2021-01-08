using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public class UserTypeMappingService : IUserTypeMappingService
    {
        private readonly ILanguageRepository _languageRepository;
        private List<LanguageEntity> _languageEntities;
        public UserTypeMappingService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;

        }

        private List<LanguageEntity> GetLanguageEntities()
        {
            return _languageEntities = _languageEntities ?? _languageRepository.GetLanguages();
        }
        public IdentityStatusDto ToIdentityStatusDto(UserTypeEntity userTypeEntity)
        {
            return new IdentityStatusDto
            {
                Identity = new IdentityDto
                {
                    Id = userTypeEntity.UserTypeId,
                    Archetype = userTypeEntity.ArchetypeId.HasValue ? (ArchetypeEnum)userTypeEntity.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = userTypeEntity.ExtId,
                    OwnerId = userTypeEntity.OwnerId
                },
                EntityStatus = new EntityStatusDto
                {
                    StatusId = EntityStatusEnum.Active
                }
            };
        }


        public UserTypeDto ToUserTypeDto(UserTypeEntity userTypeEntity, string langCode = "en-US")
        {
            var roleDto = ToUserTypeDtoInternal(userTypeEntity);
            roleDto.LocalizedData = ToLocalizedDataDto(userTypeEntity.LT_UserType, langCode);
            return roleDto;
        }

        public UserTypeDto ToUserTypeDto(UserTypeEntity userTypeEntity, int languageId)
        {
            var roleDto = ToUserTypeDtoInternal(userTypeEntity);
            roleDto.LocalizedData = ToLocalizedDataDto(userTypeEntity.LT_UserType, languageId);
            return roleDto;
        }
        private UserTypeDto ToUserTypeDtoInternal(UserTypeEntity userTypeEntity)
        {
            var roleDto = new UserTypeDto()
            {
                Identity = new IdentityDto()
                {
                    Archetype = userTypeEntity.ArchetypeId.HasValue
                        ? (ArchetypeEnum) userTypeEntity.ArchetypeId
                        : ArchetypeEnum.Unknown,
                    Id = userTypeEntity.UserTypeId,
                    OwnerId = userTypeEntity.OwnerId,
                    ExtId = userTypeEntity.ExtId
                },
                EntityStatus = new EntityStatusDto()
                {
                    StatusId = EntityStatusEnum.Active
                },
                ParentId = userTypeEntity.ParentId
            };
            return roleDto;
        }

        public List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtUserTypeEntity> lTUserTypeEntities, string langCode = "")
        {

            if (!string.IsNullOrEmpty(langCode))
            {
                var language = GetLanguageEntities().FirstOrDefault(x => x.LanguageCode == langCode);
                lTUserTypeEntities = lTUserTypeEntities.Where(x => x.LanguageId == language.LanguageId);
            }
            if (lTUserTypeEntities != null && lTUserTypeEntities.Any())
            {
                return ToLocalizedDataDtos(lTUserTypeEntities);
            }
            return new List<LocalizedDataDto>();
        }

        public List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtUserTypeEntity> lTUserTypeEntities, int languageId)
        {

            if (languageId > 0)
            {
                var language = GetLanguageEntities().FirstOrDefault(x => x.LanguageId == languageId);
                lTUserTypeEntities = lTUserTypeEntities.Where(x => x.LanguageId == language.LanguageId);
            }
            if (lTUserTypeEntities != null && lTUserTypeEntities.Any())
            {
                return ToLocalizedDataDtos(lTUserTypeEntities);
            }
            return new List<LocalizedDataDto>();
        }


        public List<UserTypeDto> ToUserTypeDtos(ICollection<UserTypeEntity> userTypes, string langCode = "en-US")
        {
            if (userTypes == null || !userTypes.Any()) return new List<UserTypeDto>();
            return userTypes.Select(userType => ToUserTypeDto(userType, langCode)).ToList();
        }

        public List<UserTypeDto> ToUserTypeDtos(ICollection<UserTypeEntity> userTypes, int languageId)
        {
            if (userTypes == null || !userTypes.Any()) return new List<UserTypeDto>();
            return userTypes.Select(userType => ToUserTypeDto(userType, languageId)).ToList();
        }


        private List<LocalizedDataDto> ToLocalizedDataDtos(IEnumerable<LtUserTypeEntity> lTUserTypeEntities)
        {
            var localizedData = new List<LocalizedDataDto>();

            var localizedProperties = typeof(LtUserTypeEntity).GetProperties()
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
    }
}
