using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Security.AccessServices
{
    public abstract class AccessServiceBase
    {
        protected readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        protected readonly ILogger _logger;
        protected readonly IDTDEntityRepository _dtdEntityRepository;
        protected readonly IDepartmentTypeRepository _departmentTypeRepository;
        protected readonly List<DepartmentTypeEntity> _departmentTypeEntities;
        protected AccessServiceBase(ILogger logger,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository, 
            IDTDEntityRepository dtdEntityRepository,
            IDepartmentTypeRepository departmentTypeRepository)
        {
            _logger = logger;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _dtdEntityRepository = dtdEntityRepository;
            _departmentTypeRepository = departmentTypeRepository;
        }

     

        protected AccessSettingElement GetFinalAccessSettingOfRoles(IList<UserRole> executorRoles, Dictionary<string, AccessSettingElement> accessSettingByRole)
        {
            if (accessSettingByRole == null) return null;
            var readUserAccessSettings = GetAccessSettings(executorRoles, accessSettingByRole);
            if (readUserAccessSettings.Count == 0)
            {
                _logger.LogWarning($"Missing access configuration for system roles {string.Join(',', executorRoles.Select(r => r.ExtId))}");
                return null;
            }

            var finalAccessSetting = readUserAccessSettings.Count == 1 ? readUserAccessSettings.First() : UnionAccessSettings(readUserAccessSettings);

            return finalAccessSetting;
        }

        protected List<T> GetAccessSettings<T>(IList<UserRole> executorRoles, Dictionary<string, T> accessSettings)
            where T : AccessSettingElement
        {
            var readUserAccessSettings = accessSettings
                .Where(userAccess => userAccess.Value != null
                                     && executorRoles.Any(r => string.Equals(r.ExtId, userAccess.Key,
                                         StringComparison.CurrentCultureIgnoreCase)))
                .Select(v => v.Value).ToList();
            return readUserAccessSettings;
        }

        protected T UnionAccessSettings<T>(List<T> readUserAccessSettings) where T : AccessSettingElement, new()
        {
            var finalAccessSetting = new T()
            {
                HasFullAccessOnHierarchy = readUserAccessSettings.Any(r => r.HasFullAccessOnHierarchy),
                InSameDepartment = readUserAccessSettings.Any(r => r.InSameDepartment),
                InAncestorDepartmentTypeExtIds = readUserAccessSettings
                    .SelectMany(r => r.InAncestorDepartmentTypeExtIds)
                    .Distinct()
                    .ToList(),
                InDescendantDepartmentTypeExtIds = readUserAccessSettings
                    .SelectMany(r => r.InDescendantDepartmentTypeExtIds)
                    .Distinct()
                    .ToList(),
                InOwnedUserGroupArchetypes = readUserAccessSettings
                    .SelectMany(r => r.InOwnedUserGroupArchetypes)
                    .Distinct()
                    .ToList(),
                InDepartmentUserGroupArchetypes = readUserAccessSettings
                    .SelectMany(r => r.InDepartmentUserGroupArchetypes)
                    .Distinct()
                    .ToList(),
                InRelativeDepartmentTypeExtIds = readUserAccessSettings
                    .SelectMany(r => r.InRelativeDepartmentTypeExtIds)
                    .Distinct()
                    .ToList(),
                OnlyMoveUpOneAncestor = readUserAccessSettings
                    .Any(r => r.OnlyMoveUpOneAncestor)
            };

            var unitedOnlyUserWithUserTypeExtIds = new Dictionary<ArchetypeEnum, List<string>>();

            var allConfiguredUserTypeArchetype = readUserAccessSettings
                .Where(r => r.OnlyUserWithUserTypeExtIds != null)
                .SelectMany(s => s.OnlyUserWithUserTypeExtIds.Keys.ToList())
                .Distinct()
                .ToList();

            foreach (var archetype in allConfiguredUserTypeArchetype)
            {
                if (!unitedOnlyUserWithUserTypeExtIds.TryGetValue(archetype, out var userTypeExtIds))
                {
                    userTypeExtIds = new List<string>();
                    unitedOnlyUserWithUserTypeExtIds.Add(archetype, userTypeExtIds);
                }

                foreach (var readUserAccessSetting in readUserAccessSettings)
                {
                    //If there is no config access for any archetype or this archetype in this readUserAccessSetting, we consider this readUserAccessSetting is allow access all user type 
                  
                    List<string> configuredUserTypeExtIds = null;

                    if (readUserAccessSetting.OnlyUserWithUserTypeExtIds?.TryGetValue(archetype, out configuredUserTypeExtIds) == true)
                    {
                        userTypeExtIds.AddRange(configuredUserTypeExtIds ?? new List<string>());
                    }
                    else
                    {
                        userTypeExtIds.Add(AccessSettingElement.AllSymbol);
                    }
                }

                unitedOnlyUserWithUserTypeExtIds[archetype] = userTypeExtIds.Distinct().ToList();
            }

            finalAccessSetting.OnlyUserWithUserTypeExtIds = unitedOnlyUserWithUserTypeExtIds;

            return finalAccessSetting;
        }

        protected bool IsSelfAccess(UserEntity executorUser, int userId)
        {
            return executorUser.UserId == userId;
        }

        protected bool IsSelfAccess(UserEntity executorUser, string executorClaim, List<string> userExtIds, List<int> userIds, List<string> loginServiceClaims)
        {
            var hasSingleUserId = userIds != null && userIds.Count == 1;
            var hasSingleExtId = userExtIds != null && userExtIds.Count == 1;
            var hasSingleLoginServiceClaim = loginServiceClaims != null && loginServiceClaims.Count == 1;
            if (!hasSingleUserId && !hasSingleExtId && !hasSingleLoginServiceClaim) return false;

            return (userIds.IsNullOrEmpty() || (hasSingleUserId && userIds.First() == executorUser.UserId))
                   && (userExtIds.IsNullOrEmpty() || (hasSingleExtId && string.Equals(userExtIds.First(), executorUser.ExtId, StringComparison.CurrentCultureIgnoreCase)))
                   && (loginServiceClaims.IsNullOrEmpty() || (hasSingleLoginServiceClaim && string.Equals(loginServiceClaims.First(), executorClaim)));

        }

        protected IList<UserRole> MapToUserRoles(ICollection<UTUEntity> uTUEntities)
        {
            if (uTUEntities == null) return new List<UserRole>();

            return uTUEntities
                .Where(utu => utu.UserType != null)
                .Select(utu => new UserRole { Id = utu.UserType.UserTypeId, ExtId = utu.UserType.ExtId }).ToList();
        }
        protected HierarchyInfo FindTopHierarchyInfo(List<HierarchyInfo> hierarchyInfos)
        {
            if (hierarchyInfos.Count == 1)
                return hierarchyInfos.First();

            foreach (var hierarchyInfo in hierarchyInfos)
            {
                if (hierarchyInfo.ParentHdId == null || hierarchyInfos.All(h => h.HdId != hierarchyInfo.ParentHdId))
                {
                    return hierarchyInfo;
                }
            }

            return null;
        }

        protected List<HierarchyInfo> GetAccessibleHierarchyInfos(IWorkContext workContext, int executorDepartmentId, AccessSettingElement accessSetting)
        {
            var accessibleHierarchyInfos = new List<HierarchyInfo>();
            var currentHd =  _hierarchyDepartmentRepository.GetById(workContext.CurrentHdId);

            var departmentTypes = _departmentTypeRepository.GetAllDepartmentTypesInCache();

            var ancestorHierarchyInfos =
                GetAccessibleAncestorHierarchyInfos(executorDepartmentId, accessSetting, currentHd, departmentTypes);
            List<HierarchyInfo> topToBelowHierarchyInfos = null;

            if (!ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos.IsNullOrEmpty())
            {
                accessibleHierarchyInfos.AddRange(ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos);

                var accessibleAncestorHierarchyInfos = ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos;

                //Get accessible hierarchy info which have same ancestor

                var relativeHierarchyInfos = GetAccessibleRelativeHierarchyInfos(accessSetting, accessibleAncestorHierarchyInfos, currentHd, ancestorHierarchyInfos, departmentTypes);

                topToBelowHierarchyInfos = relativeHierarchyInfos.TopToBelowHierarchyInfos;

                if (!relativeHierarchyInfos.AccessibleRelativeHierarchyInfos.IsNullOrEmpty())
                {
                    var nonExistingHierarchyInfos = relativeHierarchyInfos.AccessibleRelativeHierarchyInfos.Where(hd =>
                        accessibleHierarchyInfos.All(a => a.HdId != hd.HdId)).ToList();

                    accessibleHierarchyInfos.AddRange(nonExistingHierarchyInfos);
                }
            }

            var currentHierarchyInfo = ancestorHierarchyInfos.CurrentHierarchyInfo;


            var descendantHierarchyInfos =  GetAccessibleDescendantHierarchyInfos(executorDepartmentId,
                accessSetting, currentHd, departmentTypes, topToBelowHierarchyInfos);

            if (!descendantHierarchyInfos.AccessibleDescendantHierarchyInfos.IsNullOrEmpty())
            {
                accessibleHierarchyInfos.AddRange(descendantHierarchyInfos.AccessibleDescendantHierarchyInfos);
            }

            if (currentHierarchyInfo == null)
            {
                currentHierarchyInfo = descendantHierarchyInfos.CurrentHierarchyInfo;
            }


            if (accessSetting.InSameDepartment)
            {
                if (currentHierarchyInfo == null)
                {
                    currentHierarchyInfo = _hierarchyDepartmentRepository.GetHierarchyInfos(currentHd.HDId,
                            new List<int> {executorDepartmentId}, false, hierarchyId: currentHd.HierarchyId)
                        .FirstOrDefault();
                }

                if (currentHierarchyInfo != null)
                {
                    accessibleHierarchyInfos.Add(currentHierarchyInfo);
                }
            }

            return accessibleHierarchyInfos;
        }


        protected async Task<List<HierarchyInfo>> GetAccessibleHierarchyInfosAsync(
            IWorkContext workContext,
            int executorDepartmentId,
            AccessSettingElement accessSetting,
            List<int> parentDepartmentId = null)
        {
            var accessibleHierarchyInfos = new List<HierarchyInfo>();
            var currentHd = await _hierarchyDepartmentRepository.GetByIdAsync(workContext.CurrentHdId);

            var departmentTypes = await _departmentTypeRepository.GetAllDepartmentTypesInCacheAsync();

            var ancestorHierarchyInfos  = await GetAccessibleAncestorHierarchyInfosAsync(
                executorDepartmentId,
                accessSetting,
                currentHd,
                departmentTypes);

            // Process for DLC in Division
            if (accessSetting.OnlyMoveUpOneAncestor)
            {
                var currentDtdEntity = _dtdEntityRepository.GetDepartmentDepartmentTypes(new List<int>() { ancestorHierarchyInfos.CurrentHierarchyInfo.DepartmentId });

                var isCurrentUserInBranchOfDivision = departmentTypes.Any(dt => dt.DepartmentTypeId == currentDtdEntity[0].DepartmentTypeId
                    && dt.ExtId == "branch");

                if (isCurrentUserInBranchOfDivision)
                {
                    ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos = ancestorHierarchyInfos
                        .AccessibleAncestorHierarchyInfos
                        .Where(hierarchy => hierarchy.HdId == ancestorHierarchyInfos.CurrentHierarchyInfo.ParentHdId.Value)
                        .ToList();
                }    
            }    

            List<HierarchyInfo> topToBelowHierarchyInfos = null;

            if (!ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos.IsNullOrEmpty())
            {
                accessibleHierarchyInfos.AddRange(ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos);

                var accessibleAncestorHierarchyInfos = ancestorHierarchyInfos.AccessibleAncestorHierarchyInfos;

                //Get accessible hierarchy info which have same ancestor

                var relativeHierarchyInfos = await GetAccessibleRelativeHierarchyInfosAsync(
                    accessSetting,
                    accessibleAncestorHierarchyInfos,
                    currentHd, ancestorHierarchyInfos,
                    departmentTypes,
                    parentDepartmentId,
                    accessSetting.OnlyMoveUpOneAncestor);

                topToBelowHierarchyInfos = relativeHierarchyInfos.TopToBelowHierarchyInfos;

                if (!relativeHierarchyInfos.AccessibleRelativeHierarchyInfos.IsNullOrEmpty())
                {
                    var nonExistingHierarchyInfos = relativeHierarchyInfos.AccessibleRelativeHierarchyInfos.Where(hd =>
                        accessibleHierarchyInfos.All(a => a.HdId != hd.HdId)).ToList();

                    accessibleHierarchyInfos.AddRange(nonExistingHierarchyInfos);
                }
            }

            var currentHierarchyInfo = ancestorHierarchyInfos.CurrentHierarchyInfo;


            var descendantHierarchyInfos = await GetAccessibleDescendantHierarchyInfosAsync(executorDepartmentId,
                accessSetting, currentHd, departmentTypes, topToBelowHierarchyInfos);

            if (!descendantHierarchyInfos.AccessibleDescendantHierarchyInfos.IsNullOrEmpty())
            {
                accessibleHierarchyInfos.AddRange(descendantHierarchyInfos.AccessibleDescendantHierarchyInfos);
            }

            if (currentHierarchyInfo == null)
            {
                currentHierarchyInfo = descendantHierarchyInfos.CurrentHierarchyInfo;
            }


            if (accessSetting.InSameDepartment)
            {
                if (currentHierarchyInfo == null)
                {
                    currentHierarchyInfo = (await _hierarchyDepartmentRepository.GetHierarchyInfosAsync(currentHd.HDId,
                            new List<int> {executorDepartmentId}, false, hierarchyId: currentHd.HierarchyId))
                        .FirstOrDefault();
                }

                if (currentHierarchyInfo != null)
                {
                    accessibleHierarchyInfos.Add(currentHierarchyInfo);
                }
            }

            if (accessSetting.OnlyMoveUpOneAncestor && topToBelowHierarchyInfos is object)
            {
                accessibleHierarchyInfos = accessibleHierarchyInfos
                    .Concat(topToBelowHierarchyInfos)
                    .Distinct()
                    .ToList();
            }

            return accessibleHierarchyInfos;
        }
        private (List<HierarchyInfo> AccessibleRelativeHierarchyInfos, List<HierarchyInfo> TopToBelowHierarchyInfos) GetAccessibleRelativeHierarchyInfos(AccessSettingElement accessSetting,
         List<HierarchyInfo> accessibleAncestorHierarchyInfos, HierarchyDepartmentEntity currentHd,
         (HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleAncestorHierarchyInfos) ancestorHierarchyInfos,
         List<DepartmentTypeEntity> departmentTypes)
        {

            if (!accessSetting.InRelativeDepartmentTypeExtIds.IsNullOrEmpty())
            {
                List<HierarchyInfo> accessibleRelativeHierarchyInfos = null;
                var accessibleTopDepartmentId = FindTopHierarchyInfo(accessibleAncestorHierarchyInfos)?.DepartmentId;


                var topToBelowHierarchyInfos = accessibleTopDepartmentId == null
                    ? new List<HierarchyInfo>()
                    : _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToBelow(
                        currentHd.HierarchyId, parentDepartmentIds: new List<int> {accessibleTopDepartmentId.Value});

                var relativeHierarchyInfos = topToBelowHierarchyInfos
                    .Where(hd => hd.Path.StartsWith(ancestorHierarchyInfos.CurrentHierarchyInfo.Path)).ToList();

                if (AccessSettingElement.ContainsAllSymbol(accessSetting.InRelativeDepartmentTypeExtIds))
                {
                    accessibleRelativeHierarchyInfos = relativeHierarchyInfos;
                }
                else
                {
                    var departmentTypeIds = departmentTypes.Where(dt =>
                            accessSetting.InRelativeDepartmentTypeExtIds.Contains(dt.ExtId,
                                StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                        .ToList();

                    if (departmentTypeIds.Count > 0)
                    {
                        var validDtdEntities =  _dtdEntityRepository.GetDepartmentDepartmentTypes(
                            departmentIds: relativeHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                            departmentTypeIds: departmentTypeIds);

                        accessibleRelativeHierarchyInfos = relativeHierarchyInfos
                            .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                    }
                }

                return (accessibleRelativeHierarchyInfos, topToBelowHierarchyInfos);
            }

            return (null, null);
        }
        private async Task<(List<HierarchyInfo> AccessibleRelativeHierarchyInfos, List<HierarchyInfo> TopToBelowHierarchyInfos)> GetAccessibleRelativeHierarchyInfosAsync(AccessSettingElement accessSetting,
            List<HierarchyInfo> accessibleAncestorHierarchyInfos, HierarchyDepartmentEntity currentHd,
            (HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleAncestorHierarchyInfos) ancestorHierarchyInfos,
            List<DepartmentTypeEntity> departmentTypes,
            List<int> parentDepartmentId,
            bool isAccessFromDLC = false)
        {

            if (!accessSetting.InRelativeDepartmentTypeExtIds.IsNullOrEmpty())
            {
                List<HierarchyInfo> accessibleRelativeHierarchyInfos = null;
                var accessibleTopDepartmentId = FindTopHierarchyInfo(accessibleAncestorHierarchyInfos)?.DepartmentId;


                var topToBelowHierarchyInfos = accessibleTopDepartmentId == null
                    ? new List<HierarchyInfo>()
                    : await _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToBelowAsync(
                        currentHd.HierarchyId, parentDepartmentIds: new List<int> {accessibleTopDepartmentId.Value});

                var relativeHierarchyInfos = topToBelowHierarchyInfos
                    .Where(hd => hd.Path.StartsWith(ancestorHierarchyInfos.CurrentHierarchyInfo.Path))
                    .ToList();

                if (isAccessFromDLC)
                {
                    relativeHierarchyInfos
                        .AddRange(topToBelowHierarchyInfos
                        .Where(hd => parentDepartmentId is object && parentDepartmentId.Contains(hd.DepartmentId))
                        .ToList());
                    relativeHierarchyInfos.Distinct();
                }
                
                if (AccessSettingElement.ContainsAllSymbol(accessSetting.InRelativeDepartmentTypeExtIds))
                {
                    accessibleRelativeHierarchyInfos = relativeHierarchyInfos;
                }
                else
                {
                    var departmentTypeIds = departmentTypes.Where(dt =>
                            accessSetting.InRelativeDepartmentTypeExtIds.Contains(dt.ExtId,
                                StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                        .ToList();

                    if (departmentTypeIds.Count > 0)
                    {
                        var validDtdEntities = await _dtdEntityRepository.GetDepartmentDepartmentTypesAsync(
                            departmentIds: relativeHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                            departmentTypeIds: departmentTypeIds);

                        accessibleRelativeHierarchyInfos = relativeHierarchyInfos
                            .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                    }
                }

                return (accessibleRelativeHierarchyInfos, topToBelowHierarchyInfos);
            }
         
            return (null, null);
        }

        private (HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleAncestorHierarchyInfos) GetAccessibleAncestorHierarchyInfos(int executorDepartmentId,
           AccessSettingElement accessSetting, HierarchyDepartmentEntity currentHd, List<DepartmentTypeEntity> departmentTypeEntities)
        {
            var isAccessibleOnAllAncestorDepartmentType = AccessSettingElement.ContainsAllSymbol(accessSetting.InAncestorDepartmentTypeExtIds);
            var hasAnyAccessOnAncestorDepartmentType = isAccessibleOnAllAncestorDepartmentType || !accessSetting.InAncestorDepartmentTypeExtIds.IsNullOrEmpty();
            if (!hasAnyAccessOnAncestorDepartmentType) return (null, null);

            var allCurrentToTopHierarchyInfos =
                 _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToTheTop(
                    currentHd.HierarchyId, new List<int> { executorDepartmentId });

            var currentHierarchyInfo = allCurrentToTopHierarchyInfos.FirstOrDefault(hd => hd.DepartmentId == executorDepartmentId);

            var ancestorHierarchyInfos = allCurrentToTopHierarchyInfos.Where(hd => hd.DepartmentId != executorDepartmentId).ToList();

            if (ancestorHierarchyInfos.Count == 0) return (currentHierarchyInfo, null);
            List<HierarchyInfo> accessibleAncestorHierarchyInfos;
            if (isAccessibleOnAllAncestorDepartmentType)
            {
                accessibleAncestorHierarchyInfos = ancestorHierarchyInfos;
            }
            else
            {
                var departmentTypeIds = departmentTypeEntities.Where(dt =>
                        accessSetting.InAncestorDepartmentTypeExtIds.Contains(dt.ExtId,
                            StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                    .ToList();

                if (departmentTypeIds.Count > 0)
                {
                    var validDtdEntities =  _dtdEntityRepository.GetDepartmentDepartmentTypes(
                        departmentIds: ancestorHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                        departmentTypeIds: departmentTypeIds);

                    accessibleAncestorHierarchyInfos = ancestorHierarchyInfos
                        .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                }
                else
                {
                    accessibleAncestorHierarchyInfos = new List<HierarchyInfo>();
                }
            }

            return (currentHierarchyInfo, accessibleAncestorHierarchyInfos);
        }



        private async Task<(HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleAncestorHierarchyInfos)> GetAccessibleAncestorHierarchyInfosAsync(
            int executorDepartmentId, 
            AccessSettingElement accessSetting,
            HierarchyDepartmentEntity currentHd,
            List<DepartmentTypeEntity> departmentTypeEntities)
        {
            var isAccessibleOnAllAncestorDepartmentType = AccessSettingElement.ContainsAllSymbol(accessSetting.InAncestorDepartmentTypeExtIds);
            var hasAnyAccessOnAncestorDepartmentType = isAccessibleOnAllAncestorDepartmentType || !accessSetting.InAncestorDepartmentTypeExtIds.IsNullOrEmpty();
            if (!hasAnyAccessOnAncestorDepartmentType) return (null, null);

            var allCurrentToTopHierarchyInfos =
                await _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToTheTopAsync(
                    currentHd.HierarchyId, new List<int> {executorDepartmentId});

            var currentHierarchyInfo = allCurrentToTopHierarchyInfos.FirstOrDefault(hd => hd.DepartmentId == executorDepartmentId);

            var ancestorHierarchyInfos = allCurrentToTopHierarchyInfos.Where(hd => hd.DepartmentId != executorDepartmentId).ToList();

            if (ancestorHierarchyInfos.Count == 0) return (currentHierarchyInfo, null);
            List<HierarchyInfo> accessibleAncestorHierarchyInfos;
            if (isAccessibleOnAllAncestorDepartmentType)
            {
                accessibleAncestorHierarchyInfos = ancestorHierarchyInfos;
            }
            else
            {
                var departmentTypeIds = departmentTypeEntities.Where(dt =>
                        accessSetting.InAncestorDepartmentTypeExtIds.Contains(dt.ExtId,
                            StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                    .ToList();

                if (departmentTypeIds.Count > 0)
                {
                    var validDtdEntities = await _dtdEntityRepository.GetDepartmentDepartmentTypesAsync(
                        departmentIds: ancestorHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                        departmentTypeIds: departmentTypeIds);

                    accessibleAncestorHierarchyInfos = ancestorHierarchyInfos
                        .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                }
                else
                {
                    accessibleAncestorHierarchyInfos = new List<HierarchyInfo>();
                }
            }

            return (currentHierarchyInfo, accessibleAncestorHierarchyInfos);
        }

        private (HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleDescendantHierarchyInfos) GetAccessibleDescendantHierarchyInfos(int executorDepartmentId, AccessSettingElement accessSetting,
                HierarchyDepartmentEntity currentHd, List<DepartmentTypeEntity> departmentTypeEntities, List<HierarchyInfo> topToBelowHierarchyInfos)
        {
            var isAccessibleOnAllDescendantDepartmentType =
                AccessSettingElement.ContainsAllSymbol(accessSetting.InDescendantDepartmentTypeExtIds);
            var hasAnyAccessOnDescendantDepartmentType = isAccessibleOnAllDescendantDepartmentType ||
                                                         !accessSetting.InDescendantDepartmentTypeExtIds
                                                             .IsNullOrEmpty();
            if (!hasAnyAccessOnDescendantDepartmentType) return (null, null);
            HierarchyInfo currentHierarchyInfo;
            List<HierarchyInfo> descendantHierarchyInfos;
            if (topToBelowHierarchyInfos == null)
            {
                var allCurrentToBelowHierarchyInfos =
                     _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToBelow(
                        currentHd.HierarchyId, new List<int> { executorDepartmentId });

                currentHierarchyInfo =
                   allCurrentToBelowHierarchyInfos.FirstOrDefault(hd => hd.DepartmentId == executorDepartmentId);

                descendantHierarchyInfos = allCurrentToBelowHierarchyInfos
                   .Where(hd => hd.DepartmentId != executorDepartmentId).ToList();
            }
            else
            {
                currentHierarchyInfo =
                    topToBelowHierarchyInfos.FirstOrDefault(h => h.DepartmentId == executorDepartmentId);

                descendantHierarchyInfos = currentHierarchyInfo == null
                    ? new List<HierarchyInfo>()
                    : topToBelowHierarchyInfos.Where(h =>
                            h.DepartmentId != executorDepartmentId && h.Path.StartsWith(currentHierarchyInfo.Path))
                        .ToList();
            }

            if (descendantHierarchyInfos.Count == 0) return (null, null);

            List<HierarchyInfo> accessibleDescendantHierarchyInfos;
            if (isAccessibleOnAllDescendantDepartmentType)
            {
                accessibleDescendantHierarchyInfos = descendantHierarchyInfos;
            }
            else
            {

                var departmentTypeIds = departmentTypeEntities.Where(dt =>
                        accessSetting.InDescendantDepartmentTypeExtIds.Contains(dt.ExtId,
                            StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                    .ToList();
                if (departmentTypeIds.Count > 0)
                {
                    var validDtdEntities = _dtdEntityRepository.GetDepartmentDepartmentTypes(
                        departmentIds: descendantHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                        departmentTypeIds: departmentTypeIds);


                    accessibleDescendantHierarchyInfos = descendantHierarchyInfos
                        .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                }
                else
                {
                    accessibleDescendantHierarchyInfos = new List<HierarchyInfo>();
                }

            }

            return (currentHierarchyInfo, accessibleDescendantHierarchyInfos);
        }
        private async Task<(HierarchyInfo CurrentHierarchyInfo, List<HierarchyInfo> AccessibleDescendantHierarchyInfos)> GetAccessibleDescendantHierarchyInfosAsync(int executorDepartmentId, AccessSettingElement accessSetting,
                HierarchyDepartmentEntity currentHd , List<DepartmentTypeEntity> departmentTypeEntities, List<HierarchyInfo> topToBelowHierarchyInfos)
        {
            var isAccessibleOnAllDescendantDepartmentType =
                AccessSettingElement.ContainsAllSymbol(accessSetting.InDescendantDepartmentTypeExtIds);
            var hasAnyAccessOnDescendantDepartmentType = isAccessibleOnAllDescendantDepartmentType ||
                                                         !accessSetting.InDescendantDepartmentTypeExtIds
                                                             .IsNullOrEmpty();
            if (!hasAnyAccessOnDescendantDepartmentType) return (null, null);
            HierarchyInfo currentHierarchyInfo;
            List<HierarchyInfo> descendantHierarchyInfos;
            if (topToBelowHierarchyInfos == null)
            {
                var allCurrentToBelowHierarchyInfos =
                    await _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToBelowAsync(
                        currentHd.HierarchyId, new List<int> {executorDepartmentId});

                 currentHierarchyInfo =
                    allCurrentToBelowHierarchyInfos.FirstOrDefault(hd => hd.DepartmentId == executorDepartmentId);

                 descendantHierarchyInfos = allCurrentToBelowHierarchyInfos
                    .Where(hd => hd.DepartmentId != executorDepartmentId).ToList();
            }
            else
            {
                currentHierarchyInfo =
                    topToBelowHierarchyInfos.FirstOrDefault(h => h.DepartmentId == executorDepartmentId);

                descendantHierarchyInfos = currentHierarchyInfo == null
                    ? new List<HierarchyInfo>()
                    : topToBelowHierarchyInfos.Where(h =>
                            h.DepartmentId != executorDepartmentId && h.Path.StartsWith(currentHierarchyInfo.Path))
                        .ToList();
            }

            if (descendantHierarchyInfos.Count == 0) return (null, null);

            List<HierarchyInfo> accessibleDescendantHierarchyInfos;
            if (isAccessibleOnAllDescendantDepartmentType)
            {
                accessibleDescendantHierarchyInfos = descendantHierarchyInfos;
            }
            else
            {

                var departmentTypeIds = departmentTypeEntities.Where(dt =>
                        accessSetting.InDescendantDepartmentTypeExtIds.Contains(dt.ExtId,
                            StringComparer.CurrentCultureIgnoreCase)).Select(dt => dt.DepartmentTypeId)
                    .ToList();
                if (departmentTypeIds.Count > 0)
                {
                    var validDtdEntities = await _dtdEntityRepository.GetDepartmentDepartmentTypesAsync(
                        departmentIds: descendantHierarchyInfos.Select(h => h.DepartmentId).ToList(),
                        departmentTypeIds: departmentTypeIds);


                    accessibleDescendantHierarchyInfos = descendantHierarchyInfos
                        .Where(hd => validDtdEntities.Any(dt => dt.DepartmentId == hd.DepartmentId)).ToList();
                }
                else
                {
                    accessibleDescendantHierarchyInfos = new List<HierarchyInfo>();
                }

            }

            return (currentHierarchyInfo, accessibleDescendantHierarchyInfos);
        }

    }
}