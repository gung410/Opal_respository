using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public class DepartmentTypeService : IDepartmentTypeService
    {
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IDepartmentTypeMappingService _departmentTypeMappingService;

        public DepartmentTypeService(IDepartmentTypeRepository departmentTypeRepository,
            IDepartmentTypeMappingService departmentTypeMappingService)
        {
            _departmentTypeRepository = departmentTypeRepository;
            _departmentTypeMappingService = departmentTypeMappingService;
        }

        public List<IdentityStatusDto> GetDepartmentTypes(int ownerId,
            List<int> departmentIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null)
        {
            var departmentTypeEntities = _departmentTypeRepository.GetDepartmentTypes(ownerId: ownerId,
                departmentIds: departmentIds,
                archetypeIds: archetypeIds,
                departmentTypeIds: departmentTypeIds,
                extIds: extIds);
            List<IdentityStatusDto> result = new List<IdentityStatusDto>();
            foreach (var departmentTypeEntity in departmentTypeEntities)
            {
                result.Add(ToIdentityStatusDto(departmentTypeEntity));
            }
            return result;
        }
        private IdentityStatusDto ToIdentityStatusDto(DepartmentTypeEntity departmentTypeEntity)
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
                EntityStatus = new EntityStatusDto()

            };
        }

        public bool HasDepartmentType(int ownerId, int departmentId, int departmentTypeId)
        {
            return _departmentTypeRepository.HasDepartmentType(ownerId, departmentId, departmentTypeId);
        }

        public List<DepartmentTypeDto> GetAllDepartmentTypes(int ownerId,
            List<ArchetypeEnum> archetypeIds,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US")
        {
            var departmentTypeEntities = _departmentTypeRepository.GetAllDepartmentTypes(ownerId: ownerId,
                departmentIds: departmentIds,
                archetypeIds: archetypeIds,
                departmentTypeIds: departmentTypeIds,
                extIds: extIds,
                includeLocalizedData: includeLocalizedData);
            List<DepartmentTypeDto> result = new List<DepartmentTypeDto>();
            foreach (var departmentTypeEntity in departmentTypeEntities)
            {
                result.Add(_departmentTypeMappingService.ToDepartmentTypeDto(departmentTypeEntity, langCode));
            }
            return result;
        }
        public async Task<List<DepartmentTypeDto>> GetAllDepartmentTypesAsync(int ownerId,
            List<ArchetypeEnum> archetypeIds,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US")
        {
            var departmentTypeEntities = await _departmentTypeRepository.GetAllDepartmentTypesAsync(ownerId: ownerId,
                departmentIds: departmentIds,
                archetypeIds: archetypeIds,
                departmentTypeIds: departmentTypeIds,
                extIds: extIds,
                includeLocalizedData: includeLocalizedData);
            List<DepartmentTypeDto> result = new List<DepartmentTypeDto>();
            foreach (var departmentTypeEntity in departmentTypeEntities)
            {
                result.Add(_departmentTypeMappingService.ToDepartmentTypeDto(departmentTypeEntity, langCode));
            }
            return result;
        }

        public DepartmentTypeEntity GetDepartmentTypeByExtId(string extId, int? archeTypeId = null)
        {
            return _departmentTypeRepository.GetDepartmentTypeByExtId(extId, archeTypeId);
        }
    }
}