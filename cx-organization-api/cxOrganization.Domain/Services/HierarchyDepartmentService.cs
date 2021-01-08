using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public class HierarchyDepartmentService : IHierarchyDepartmentService
    {
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IWorkContext _workContext;
        private readonly IHierarchyRepository _hierarchyRepository;
        private readonly IHierarchyDepartmentMappingService _hierarchyDepartmentMappingService;
        private readonly IUserRepository _userRepository;
        private readonly IHierarchyDepartmentPermissionService _hierarchyDepartmentPermissionService;

        public HierarchyDepartmentService(IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IWorkContext workContext,
            IHierarchyRepository hierarchyRepository,
            IDepartmentRepository departmentRepository,
            IHierarchyDepartmentMappingService hierarchyDepartmentMappingService,
            IUserRepository userRepository,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService)
        {
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _workContext = workContext;
            _hierarchyRepository = hierarchyRepository;
            _hierarchyDepartmentMappingService = hierarchyDepartmentMappingService;
            _userRepository = userRepository;
            _hierarchyDepartmentPermissionService = hierarchyDepartmentPermissionService;
        }

        public HierarchyDepartmentEntity GetH_DByDepartmentID(int departmentId, bool includeDepartment = false, bool allowGetDepartmentDeleted = false)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            return _hierarchyRepository.GetH_DByHierarchyID(currentHD.HierarchyId, departmentId, includeDepartment, allowGetDepartmentDeleted);
        }

        public HierarchyDepartmentEntity GetHierachyDepartment(int hierarchyId, int departmentId)
        {
            return _hierarchyDepartmentRepository.GetHierachyDepartment(hierarchyId, departmentId);
        }
        public Task<HierarchyDepartmentEntity> GetHierachyDepartmentAsync(int hierarchyId, int departmentId)
        {
            return _hierarchyDepartmentRepository.GetHierachyDepartmentAsync(hierarchyId, departmentId);
        }

        public IList<HierarchyDepartmentEntity> GetChildHds(string path, bool includeDepartment = true, bool includeInActiveStatus = false, List<int> departmentTypeIds = null, List<int> departmentIds = null)
        {
            return _hierarchyDepartmentRepository.GetChildHds(path, includeDepartment, includeInActiveStatus, departmentTypeIds, departmentIds).ToList();
        }

        public HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndDepartmentId(int hierarchyId, int departmentId)
        {
            return _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(hierarchyId, departmentId);
        }

        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int departmentId)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hdId = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, departmentId, true).HDId;
            return _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(hdId);
        }
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelowByHdId(int hierarchyDepartmentId, bool getAllStatus = false)
        {
            return _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(hierarchyDepartmentId, getAllStatus);
        }
        public List<int> GetAllHDIdsFromAHierachyDepartmentToBelowByHdId(int hierarchyDepartmentId, bool getAllStatus = false)
        {
            return _hierarchyDepartmentRepository.GetAllHDIdsFromAHierachyDepartmentToBelow(hierarchyDepartmentId, getAllStatus);
        }

        /// <summary>
        /// Get list HD by a filter argument
        /// </summary>
        /// <returns>H_D.</returns>
        public List<HierarchyDepartmentEntity> GetHierarchyDepartmentEntities(int ownerId,
            int hierarchyId,
            List<int?> customerIds = null,
            List<int> hdIds = null,
            List<int> departmentIds = null,
            List<string> departmentExtIds = null,
            List<ArchetypeEnum> departmentArchetypes = null,
            List<EntityStatusEnum> departmentStatuses = null,
            bool includeDepartment = false,
            string orderBy = null)
        {
            return _hierarchyDepartmentRepository.GetHierarchyDepartmentEntities(ownerId: ownerId,
                hierarchyId: hierarchyId,
                customerIds: customerIds,
                hdIds: hdIds,
                departmentIds: departmentIds,
                departmentExtIds: departmentExtIds,
                departmentArchetypes: departmentArchetypes,
                departmentStatuses: departmentStatuses,
                includeDepartment: includeDepartment,
                orderBy: orderBy);
        }
        /// <summary>Update the hierarchy department.</summary>
        /// <param name="hierarchyDepartment">The hierarchy department.</param>
        /// <returns>H_D.</returns>
        public HierarchyDepartmentEntity UpdateHierarchyDepartment(HierarchyDepartmentEntity hierarchyDepartment)
        {
            return _hierarchyDepartmentRepository.Update(hierarchyDepartment);
        }

        public HierarchyDepartmentEntity GetById(int hdId)
        {
            return _hierarchyDepartmentRepository.GetById(hdId);
        }
        public HierarchyDepartmentEntity GetCurrentHierarchyDepartment()
        {
            return GetById(_workContext.CurrentHdId);
        }
        public List<HierachyDepartmentIdentityDto> GetHierarchyDepartmentIdentities(int hierarchyId, int departmentId, bool includeParentHDs = true, bool includeChildrenHDs = false,
           int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null,
           int? maxChildrenLevel = null, bool countChildren = false, List<int> departmentTypeIds = null,
           string departmentName = null,
           bool includeDepartmentType = false, bool getParentNode = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
           List<string> jsonDynamicData = null,
           bool checkPermission = false)
        {
            var departmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(hierarchyId, departmentId, ownerId, customerIds, departmentEntityStatuses);
            var result = new List<HierachyDepartmentIdentityDto>();
            if (departmentHd == null)
                return result;

            HierarchyDepartmentEntity parentHd = null;

            if (includeParentHDs)
            {
                result.AddRange(GetParentHierarchyDepartmentIdentities(departmentHd, out parentHd, ownerId, customerIds, departmentEntityStatuses, countChildren, departmentTypeIds, includeDepartmentType));
            }
            if (departmentHd.ParentId > 0)
            {
                parentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndHdId(hierarchyId, departmentHd.ParentId.Value, ownerId, customerIds, departmentEntityStatuses, includeDepartmentType: includeDepartmentType);
            }

            var chidrenHds = new List<HierachyDepartmentIdentityDto>();
            if (includeChildrenHDs || countChildren)
            {
                //Only need to count children of children hd when we need to include them
                chidrenHds = GetChildrenHierarchyDepartmentIdentities(departmentHd, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel,
                    countChildren && includeChildrenHDs, departmentTypeIds, departmentName, includeDepartmentType: includeDepartmentType,
                    jsonDynamicData: jsonDynamicData,
                    checkPermission: checkPermission);
            }
            result.Add(_hierarchyDepartmentMappingService.ToDto(departmentHd, parentHd, countChildren ? chidrenHds.Count(h => h.ParentDepartmentId == departmentId) : (int?)null, includeDepartmentType: includeDepartmentType));

            if (includeChildrenHDs)
            {
                result.AddRange(chidrenHds);
            }
            if (getParentNode)
            {
                var nodes = chidrenHds.SelectMany(x => x.Path.Trim().Split("\\").Where(t => !string.IsNullOrEmpty(t)).Select(y => int.Parse(y))).Distinct().ToList();

                var nodeHds = _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntity(hierarchyId, nodes);
                foreach (var item in nodeHds)
                {
                    if (!result.Any(x => x.Identity.Id == item.HDId))
                        result.Add(_hierarchyDepartmentMappingService.ToDto(item, item.Parent, 0, includeDepartmentType: includeDepartmentType));
                }
            }

            if (countUser && result.Count > 0)
            {
                var departmentIds = result.Select(r => (int)r.Identity.Id).ToList();

                var numberOfUserGroupByDepartment = _userRepository.CountUsersGroupByDepartment(ownerId, customerIds,
                    parentDepartmentIds: departmentIds, statusIds: countUserEntityStatuses);
                foreach (var hierachyDepartment in result)
                {
                    numberOfUserGroupByDepartment.TryGetValue((int)hierachyDepartment.Identity.Id, out var countOfUser);
                    hierachyDepartment.UserCount = countOfUser;
                }
            }

            result.SetCurrentDepartment(departmentId);

            return result;
        }


        public async Task<List<HierachyDepartmentIdentityDto>> GetHierarchyDepartmentIdentitiesAsync(int hierarchyId, int departmentId,
            bool includeParentHDs = true, bool includeChildrenHDs = false,
           int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null,
           int? maxChildrenLevel = null, bool countChildren = false, List<int> departmentTypeIds = null,
           string departmentName = null,
           bool includeDepartmentType = false, bool getParentNode = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
           List<string> jsonDynamicData = null,
           bool checkPermission = false)
        {
            var departmentHd = await _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentIdAsync(hierarchyId, departmentId, ownerId, customerIds, departmentEntityStatuses);
            var result = new List<HierachyDepartmentIdentityDto>();
            if (departmentHd == null)
                return result;

            HierarchyDepartmentEntity parentHd = null;

            if (includeParentHDs)
            {
                var parentHDsResult = await GetParentHierarchyDepartmentIdentitiesAsync(departmentHd, ownerId, customerIds, departmentEntityStatuses, countChildren, departmentTypeIds, includeDepartmentType);
                result.AddRange(parentHDsResult.ListHierachyDepartmentIdentityDto);
                parentHd = parentHDsResult.HD;
            }
            if (departmentHd.ParentId > 0)
            {
                parentHd = await _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndHdIdAsync(hierarchyId, departmentHd.ParentId.Value, ownerId, customerIds, departmentEntityStatuses, includeDepartmentType: includeDepartmentType);
            }

            var chidrenHds = new List<HierachyDepartmentIdentityDto>();
            if (includeChildrenHDs || countChildren)
            {
                //Only need to count children of children hd when we need to include them
                chidrenHds = await GetChildrenHierarchyDepartmentIdentitiesAsync(departmentHd, ownerId, customerIds, 
                    departmentEntityStatuses, maxChildrenLevel,
                    countChildren && includeChildrenHDs, departmentTypeIds, departmentName,
                    includeDepartmentType: includeDepartmentType,
                    jsonDynamicData: jsonDynamicData,
                    checkPermission: checkPermission);
            }
            result.Add(_hierarchyDepartmentMappingService.ToDto(departmentHd, parentHd, countChildren ? chidrenHds.Count(h => h.ParentDepartmentId == departmentId) : (int?)null, includeDepartmentType: includeDepartmentType));

            if (includeChildrenHDs)
            {
                result.AddRange(chidrenHds);
            }
            if (getParentNode)
            {
                var nodes = chidrenHds.SelectMany(x => x.Path.Trim().Split("\\").Where(t => !string.IsNullOrEmpty(t)).Select(y => int.Parse(y))).Distinct().ToList();

                var nodeHds = await _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntityAsync(hierarchyId, nodes);
                foreach (var item in nodeHds)
                {
                    if (!result.Any(x => x.Identity.Id == item.HDId))
                        result.Add(_hierarchyDepartmentMappingService.ToDto(item, item.Parent, 0, includeDepartmentType: includeDepartmentType));
                }
            }

            if (countUser && result.Count > 0)
            {
                var departmentIds = result.Select(r => (int)r.Identity.Id).ToList();

                var numberOfUserGroupByDepartment = await _userRepository.CountUsersGroupByDepartmentAsync(ownerId, customerIds,
                    parentDepartmentIds: departmentIds, statusIds: countUserEntityStatuses);
                foreach (var hierachyDepartment in result)
                {
                    numberOfUserGroupByDepartment.TryGetValue((int)hierachyDepartment.Identity.Id, out var countOfUser);
                    hierachyDepartment.UserCount = countOfUser;
                }
            }

            result.SetCurrentDepartment(departmentId);

            return result;
        }

        private List<HierachyDepartmentIdentityDto> GetChildrenHierarchyDepartmentIdentities(HierarchyDepartmentEntity departmentHd, int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null, int? maxLevel = null, bool countChildren = false, List<int> departmentTypeIds = null,
            string departmentName = null, bool includeDepartmentType = false, List<string> jsonDynamicData = null,
            bool checkPermission = false)
        {
            List<HierachyDepartmentIdentityDto> result = new List<HierachyDepartmentIdentityDto>();
            //There is some case, an department has valid status but  it's ancestor 
            //We will not include H_D that its ancestor have invalid status, so we need to filter statuses manually here, instead of filter from repository
            var childrenHDs = _hierarchyDepartmentRepository.GetChildHds(departmentHd.Path, ownerId: ownerId, customerIds: customerIds, departmentEntityStatuses: null,
                maxLevel: maxLevel, includeChildren: countChildren, departmentTypeIds: departmentTypeIds,
                departmentName: departmentName, includeDepartmentType: includeDepartmentType || checkPermission, jsonDynamicData: jsonDynamicData);

            if (departmentEntityStatuses.IsNotNullOrEmpty() && !departmentEntityStatuses.Contains(EntityStatusEnum.All))
            {
                var statusesId = departmentEntityStatuses.Select(s => (int?)s).ToList();
                var invalidStatusChildrenHDs = childrenHDs.Where(hd => !statusesId.Contains(hd.Department.EntityStatusId)).ToList();
                if (invalidStatusChildrenHDs.Count > 0)
                {
                    childrenHDs = childrenHDs.Where(hd => !invalidStatusChildrenHDs.Contains(hd) && !invalidStatusChildrenHDs.Any(invaliHd => hd.Path.StartsWith(invaliHd.Path))).ToList();
                }
            }

            if (checkPermission)
            {
                childrenHDs = _hierarchyDepartmentPermissionService.SecurityCheck(childrenHDs).ToList();
            }

            childrenHDs = childrenHDs.OrderBy(hd => hd.PathName).ToList();
            foreach (var childrenHd in childrenHDs)
            {
                if (childrenHd.HDId == departmentHd.HDId) continue;
                var parentDepartment = departmentHd.HDId == childrenHd.ParentId
                    ? departmentHd
                    : childrenHd.Parent;

                //TODO
                result.Add(_hierarchyDepartmentMappingService.ToDto(childrenHd, parentDepartment, countChildren ? childrenHd.H_Ds.Count(h => h.Department != null) : (int?)null, includeDepartmentType: includeDepartmentType));
            }
            return result;
        }
        private async Task<List<HierachyDepartmentIdentityDto>> GetChildrenHierarchyDepartmentIdentitiesAsync(HierarchyDepartmentEntity departmentHd, int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null, int? maxLevel = null, bool countChildren = false, List<int> departmentTypeIds = null,
            string departmentName = null, bool includeDepartmentType = false, List<string> jsonDynamicData = null,
            bool checkPermission = false)
        {
            List<HierachyDepartmentIdentityDto> result = new List<HierachyDepartmentIdentityDto>();
            //There is some case, an department has valid status but  it's ancestor 
            //We will not include H_D that its ancestor have invalid status, so we need to filter statuses manually here, instead of filter from repository
            var childrenHDs = await _hierarchyDepartmentRepository.GetChildHdsAsync(departmentHd.Path, ownerId: ownerId, customerIds: customerIds, departmentEntityStatuses: null,
                maxLevel: maxLevel, includeChildren: countChildren, departmentTypeIds: departmentTypeIds,
                departmentName: departmentName, includeDepartmentType: includeDepartmentType || checkPermission, jsonDynamicData: jsonDynamicData);

            if (departmentEntityStatuses.IsNotNullOrEmpty() && !departmentEntityStatuses.Contains(EntityStatusEnum.All))
            {
                var statusesId = departmentEntityStatuses.Select(s => (int?)s).ToList();
                var invalidStatusChildrenHDs = childrenHDs.Where(hd => !statusesId.Contains(hd.Department.EntityStatusId)).ToList();
                if (invalidStatusChildrenHDs.Count > 0)
                {
                    childrenHDs = childrenHDs.Where(hd => !invalidStatusChildrenHDs.Contains(hd) && !invalidStatusChildrenHDs.Any(invaliHd => hd.Path.StartsWith(invaliHd.Path))).ToList();
                }
            }

            if (checkPermission)
            {
                childrenHDs = (await _hierarchyDepartmentPermissionService.SecurityCheckAsync(childrenHDs)).ToList();
            }

            childrenHDs = childrenHDs.OrderBy(hd => hd.PathName).ToList();
            foreach (var childrenHd in childrenHDs)
            {
                if (childrenHd.HDId == departmentHd.HDId) continue;
                var parentDepartment = departmentHd.HDId == childrenHd.ParentId
                    ? departmentHd
                    : childrenHd.Parent;

                //TODO
                result.Add(_hierarchyDepartmentMappingService.ToDto(childrenHd, parentDepartment, countChildren ? childrenHd.H_Ds.Count(h => h.Department != null) : (int?)null, includeDepartmentType: includeDepartmentType));
            }
            return result;
        }
        private List<HierachyDepartmentIdentityDto> GetParentHierarchyDepartmentIdentities(HierarchyDepartmentEntity departmentHd, out HierarchyDepartmentEntity parentHd, int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null, bool countChildren = false, List<int> departmentTypeIds = null, bool includeDepartmentType = false)
        {
            List<HierachyDepartmentIdentityDto> result = new List<HierachyDepartmentIdentityDto>();
            var parentHds = _hierarchyDepartmentRepository.GetParentHDs(departmentHd, ownerId, customerIds, departmentEntityStatuses, countChildren, departmentTypeIds: departmentTypeIds, includeDepartmentType: includeDepartmentType);
            foreach (var item in parentHds)
            {
                var parentDepartment = parentHds.FirstOrDefault(x => x.HDId == item.ParentId);
                //TODO
                result.Add(_hierarchyDepartmentMappingService.ToDto(item, parentDepartment, countChildren ? item.H_Ds.Count(h => h.Department != null) : (int?)null, includeDepartmentType: includeDepartmentType));
            }
            parentHd = parentHds.FirstOrDefault(x => x.HDId == departmentHd.ParentId);

            return result;
        }
        private async Task<(List<HierachyDepartmentIdentityDto> ListHierachyDepartmentIdentityDto, HierarchyDepartmentEntity HD)> GetParentHierarchyDepartmentIdentitiesAsync(HierarchyDepartmentEntity departmentHd,
            int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            bool countChildren = false, List<int> departmentTypeIds = null,
            bool includeDepartmentType = false)
        {
            List<HierachyDepartmentIdentityDto> result = new List<HierachyDepartmentIdentityDto>();
            var parentHds = await _hierarchyDepartmentRepository.GetParentHDsAsync(departmentHd, ownerId, customerIds, departmentEntityStatuses, countChildren, departmentTypeIds: departmentTypeIds, includeDepartmentType: includeDepartmentType);
            foreach (var item in parentHds)
            {
                var parentDepartment = parentHds.FirstOrDefault(x => x.HDId == item.ParentId);
                //TODO
                result.Add(_hierarchyDepartmentMappingService.ToDto(item, parentDepartment, countChildren ? item.H_Ds.Count(h => h.Department != null) : (int?)null, includeDepartmentType: includeDepartmentType));
            }

            return (result, parentHds.FirstOrDefault(x => x.HDId == departmentHd.ParentId));
        }

        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int departmentId)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hdId = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, departmentId, true).HDId;
            return _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(hdId);
        }
        public async Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int departmentId)
        {
            var currentHD =  await _hierarchyDepartmentRepository.GetByIdAsync(_workContext.CurrentHdId);
            var hdId = (await _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentIdAsync(currentHD.HierarchyId, departmentId, true)).HDId;
            return await _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(hdId);
        }
        public PaginatedList<HierachyDepartmentIdentityDto> GetAllHdsByPath(string path, string departmentName,
            int pageIndex, int pageSize, string orderBy,
            List<string> jsonDynamicData = null,
            bool getDetailDepartment = true,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool getParentDepartmentId = false)
        {
            var paginatedEntities = _hierarchyDepartmentRepository.GetHierarchyDepartments(path, departmentName, pageIndex, pageSize, orderBy, jsonDynamicData,
                departmentEntityStatuses: departmentEntityStatuses,
                departmentTypeIds: departmentTypeIds,
                includeDepartmentType: includeDepartmentType,
                includeParent: getParentDepartmentId);

            Func<HierarchyDepartmentEntity, HierachyDepartmentIdentityDto> mapEntityToDtoFunc = (entity)
                => _hierarchyDepartmentMappingService.ToDto(entity, parentHd: entity.Parent, getDetailDepartment: getDetailDepartment, includeDepartmentType: includeDepartmentType);

            return paginatedEntities.ToPaginatedListDto(mapEntityToDtoFunc);
        }

        public async Task<PaginatedList<HierachyDepartmentIdentityDto>> GetAllHdsByPathAsync(string path, string departmentName,
           int pageIndex, int pageSize, string orderBy,
           List<string> jsonDynamicData = null,
           bool getDetailDepartment = true,
           List<EntityStatusEnum> departmentEntityStatuses = null,
           List<int> departmentTypeIds = null,
           bool includeDepartmentType = false,
           bool getParentDepartmentId = false,
           List<int> departmentIds = null)
        {
            var paginatedEntities = await _hierarchyDepartmentRepository.GetHierarchyDepartmentsAsync(path, departmentName, pageIndex, pageSize, orderBy, jsonDynamicData,
                departmentEntityStatuses: departmentEntityStatuses,
                departmentTypeIds: departmentTypeIds,
                includeDepartmentType: includeDepartmentType,
                includeParent: getParentDepartmentId,
                departmentIds: departmentIds);

            Func<HierarchyDepartmentEntity, HierachyDepartmentIdentityDto> mapEntityToDtoFunc = (entity)
                => _hierarchyDepartmentMappingService.ToDto(entity, parentHd: entity.Parent, getDetailDepartment: getDetailDepartment, includeDepartmentType: includeDepartmentType);

            return paginatedEntities.ToPaginatedListDto(mapEntityToDtoFunc);
        }


    }
}
