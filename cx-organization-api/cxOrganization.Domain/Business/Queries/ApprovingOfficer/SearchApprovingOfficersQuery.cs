using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Business.Queries.ApprovingOfficer
{
    public class SearchApprovingOfficersQuery
    {
        public int ParentDepartmentId { get; set; }
        public List<int> ApprovalGroupIds { get; set; }
        public List<int> ApproverIds { get; set; }
        public List<int> EmployeeIds { get; set; }
        public List<EntityStatusEnum> StatusEnums { get; set; }
        public List<EntityStatusEnum> UserStatusEnums { get; set; }
        public List<GrouptypeEnum> GroupTypes { get; set; }
        public List<string> ExtIds { get; set; }
        public DateTime? LastUpdatedBefore { get; set; }
        public DateTime? LastUpdatedAfter { get; set; }
        public int? AssigneeDepartmentId { get; set; }
        public bool SearchInSameDepartment { get; set; }
        public bool SearchFromDepartmentToTop { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public string SearchKey { get; set; }
    }
    public class SearchApprovingOfficersQueryHandler
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly ApprovalGroupMappingService _approvalGroupMappingService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly IApprovalGroupAccessService _aprovalGroupAccessService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly AppSettings _appSettings;

        public SearchApprovingOfficersQueryHandler(IUserGroupRepository userGroupRepository,
            ApprovalGroupMappingService approvalGroupMappingService,
            IAdvancedWorkContext workContext,
            IHierarchyDepartmentService hierarchyDepartmentService,
            IApprovalGroupAccessService aprovalGroupAccessService,
            IDepartmentRepository departmentRepository,
            IOptions<AppSettings> appSettingOptions)
        {
            _userGroupRepository = userGroupRepository;
            _approvalGroupMappingService = approvalGroupMappingService;
            _workContext = workContext;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _aprovalGroupAccessService = aprovalGroupAccessService;
            _departmentRepository = departmentRepository;
            _appSettings = appSettingOptions.Value;
        }
        public PaginatedList<ApprovalGroupDto> Handle(SearchApprovingOfficersQuery query)
        {
            if (query.SearchInSameDepartment || query.SearchFromDepartmentToTop)
            {
                if (!query.AssigneeDepartmentId.HasValue || query.AssigneeDepartmentId == 0)
                    throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                        "SearchInSameDepartment, SearchFromDepartmentToTop required AssigneeDepartmentId value");
            }
            var parentDepartmentIds = new List<int>();
            if (query.ParentDepartmentId > 0)
                parentDepartmentIds.Add(query.ParentDepartmentId);

            var userIds = query.ApproverIds;

            if (!_appSettings.IsCrossOrganizationalUnit)
            {
                if (query.SearchInSameDepartment)
                {
                    parentDepartmentIds.Add(query.AssigneeDepartmentId.Value);
                }
                else if (query.SearchFromDepartmentToTop)
                {
                    var ids = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(query.AssigneeDepartmentId.Value);
                    parentDepartmentIds.AddRange(ids);
                }

                if (!_aprovalGroupAccessService.CheckReadApprovalGroupAccess(_workContext, ref userIds, ref parentDepartmentIds).IsAllowedAccess())
                {
                    return new PaginatedList<ApprovalGroupDto>();
                }
            }

            var pagingEntity = _userGroupRepository.GetUserGroups(
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                userGroupIds: query.ApprovalGroupIds,
                parentUserIds: userIds,
                memberUserIds: query.EmployeeIds,
                parentDepartmentIds: parentDepartmentIds,
                statusIds: query.StatusEnums,
                userStatusIds: query.UserStatusEnums,
                groupTypes: query.GroupTypes,
                extIds: query.ExtIds,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.ApprovalGroup },
                lastUpdatedBefore: query.LastUpdatedBefore,
                lastUpdatedAfter: query.LastUpdatedAfter,
                pageIndex: query.PageIndex,
                pageSize: query.PageSize,
                orderBy: query.OrderBy,
                searchKey: query.SearchKey,
                includeDepartment: true,
                includeUser: true);
            Func<UserGroupEntity, bool?, ApprovalGroupDto> mapEntityToDtoFunc = (entity, getDynamicPropertiesFlag)
                => (ApprovalGroupDto)_approvalGroupMappingService.ToUserGroupDto(entity, getDynamicPropertiesFlag);

            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, false);
        }
        public async Task<PaginatedList<ApprovalGroupDto>> HandleAsync(SearchApprovingOfficersQuery query)
        {
            if ((query.SearchInSameDepartment || query.SearchFromDepartmentToTop) && !_appSettings.IsCrossOrganizationalUnit)
            {
                {
                    if (!query.AssigneeDepartmentId.HasValue || query.AssigneeDepartmentId == 0)
                        throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                            "SearchInSameDepartment, SearchFromDepartmentToTop required AssigneeDepartmentId value");
                }
            }

            var parentDepartmentIds = new List<int>();
            if (query.ParentDepartmentId > 0)
                parentDepartmentIds.Add(query.ParentDepartmentId);

            var userIds = query.ApproverIds;

            if (!_appSettings.IsCrossOrganizationalUnit)
            {
                if (query.SearchInSameDepartment)
                {
                    parentDepartmentIds.Add(query.AssigneeDepartmentId.Value);
                }
                else if (query.SearchFromDepartmentToTop)
                {
                    var ids = await _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(query.AssigneeDepartmentId.Value);
                    parentDepartmentIds.AddRange(ids);
                }

                var userGroupAccess = await _aprovalGroupAccessService.CheckReadApprovalGroupAccessAsync(_workContext,
                    userIds,
                    parentDepartmentIds);

                userIds = userGroupAccess.UserIds;
                parentDepartmentIds = userGroupAccess.ParentDepartmentIds;
                if (!userGroupAccess.AccessStatus.IsAllowedAccess())
                {
                    return new PaginatedList<ApprovalGroupDto>();
                }
            }

            var pagingEntity = await _userGroupRepository.GetUserGroupsAsync(
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                userGroupIds: query.ApprovalGroupIds,
                parentUserIds: userIds,
                memberUserIds: query.EmployeeIds,
                parentDepartmentIds: parentDepartmentIds,
                statusIds: query.StatusEnums,
                userStatusIds: query.UserStatusEnums,
                groupTypes: query.GroupTypes,
                extIds: query.ExtIds,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.ApprovalGroup },
                lastUpdatedBefore: query.LastUpdatedBefore,
                lastUpdatedAfter: query.LastUpdatedAfter,
                pageIndex: query.PageIndex,
                pageSize: query.PageSize,
                orderBy: query.OrderBy,
                assigneeDepartmentId: query.AssigneeDepartmentId,
                searchKey: query.SearchKey,
                includeDepartment: true,
                includeUser: true);
            Func<UserGroupEntity, bool?, ApprovalGroupDto> mapEntityToDtoFunc = (entity, getDynamicPropertiesFlag)
                => (ApprovalGroupDto)_approvalGroupMappingService.ToUserGroupDto(entity, getDynamicPropertiesFlag);

            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, false);
        }
    }
}
