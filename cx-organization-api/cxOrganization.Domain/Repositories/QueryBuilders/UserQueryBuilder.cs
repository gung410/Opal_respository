using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public class UserQueryBuilder
    {
        private readonly AppSettings _appSettings;
        private readonly IUserCryptoService _userCryptoService;
        private IQueryable<UserEntity> _query;

        public UserQueryBuilder(IOptions<AppSettings> options,
            IUserCryptoService userCryptoService,
            IQueryable<UserEntity> query)
        {
            _appSettings = options.Value;
            _userCryptoService = userCryptoService;
            _query = query;
        }
        public IQueryable<UserEntity> Build()
        {
            return _query;
        }
        public static UserQueryBuilder InitQueryBuilder(
            IOptions<AppSettings> options,
            IUserCryptoService userCryptoService,
            IQueryable<UserEntity> query)
        {
            return new UserQueryBuilder(options, userCryptoService, query);
        }
        public UserQueryBuilder FilterByUserExtIds(List<string> userExtIds)
        {
            if (userExtIds.IsNotNullOrEmpty())
            {
                if (userExtIds.Count == 1)
                {
                    string value = userExtIds[0];
                    _query = _query.Where(t => t.ExtId == value);
                }
                else
                {
                    _query = _query.Where(t => userExtIds.Contains(t.ExtId));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByCustomerIds(List<int> customerIds)
        {
            if (customerIds.IsNotNullOrEmpty())
            {
                if (customerIds.Count == 1)
                {
                    int value = customerIds[0];
                    _query = _query.Where(t => t.CustomerId == value);
                }
                else
                {
                    _query = _query.Where(t => customerIds.Contains(t.CustomerId.Value));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByDepartmentIds(List<int> parentDepartmentIds, bool filterOnParentHd = true, bool filterOnUd = false)
        {
            //TODO: Need to check why we do filter parentDepartmentIds on parent department of user department 
            //For backward compatible, need to add new filterOnParentHd, default is true. Consumer can decide when it should not used
            if (parentDepartmentIds.IsNotNullOrEmpty())
            {
                parentDepartmentIds = parentDepartmentIds.Distinct().ToList();
                if (parentDepartmentIds.Count == 1)
                {
                    int value = parentDepartmentIds[0];

                    if (filterOnParentHd)
                    {
                        if (filterOnUd)
                        {
                            _query = _query.Where(u => u.DepartmentId == value
                                                       || u.Department.H_D.Any(t => t.Parent.DepartmentId == value)
                                                       || u.U_D.Any(x => x.DepartmentId == value));
                        }
                        else
                        {
                            _query = _query.Where(u => u.DepartmentId == value
                                                       || u.Department.H_D.Any(t => t.Parent.DepartmentId == value));
                        }

                    }
                    else if (filterOnUd)
                    {

                        _query = _query.Where(u => u.DepartmentId == value || u.U_D.Any(x => x.DepartmentId == value));
                    }
                    else
                    {
                        _query = _query.Where(u => u.DepartmentId == value);
                    }
                }
                else
                {
                    if (filterOnParentHd)
                    {
                        if (filterOnUd)
                        {
                            _query = _query.Where(u => parentDepartmentIds.Contains(u.DepartmentId)
                                                       || u.Department.H_D.Any(t =>
                                                           parentDepartmentIds.Contains(t.Parent.DepartmentId))
                                                       || u.U_D.Any(x => parentDepartmentIds.Contains(x.DepartmentId)));
                        }
                        else
                        {
                            _query = _query.Where(u => parentDepartmentIds.Contains(u.DepartmentId)
                                                       || u.Department.H_D.Any(t =>
                                                           parentDepartmentIds.Contains(t.Parent.DepartmentId)));
                        }
                    }
                    else if (filterOnUd)
                    {
                        _query = _query.Where(u => parentDepartmentIds.Contains(u.DepartmentId)
                                                   || u.U_D.Any(x => parentDepartmentIds.Contains(x.DepartmentId)));
                    }
                    else
                    {
                        _query = _query.Where(u => parentDepartmentIds.Contains(u.DepartmentId));
                    }
                }
            }
            return this;
        }

        public UserQueryBuilder FilterByDepartmentExtIds(List<string> departmentExtIds)
        {
            if (departmentExtIds.IsNotNullOrEmpty())
            {
                _query = _query.Include(user => user.Department)
                                    .ThenInclude(department => department.DT_Ds)
                               .Where(user => user.Department.DT_Ds
                               .Select(dtd => dtd.DepartmentType)
                               .Any(departmentType => departmentExtIds
                               .Contains(departmentType.ExtId)));
            }
            return this;
        }

        public UserQueryBuilder FilterByUserIds(List<int> userIds, List<int> exceptUserIds = null)
        {
            if (userIds.IsNotNullOrEmpty())
            {
                if (userIds.Count == 1)
                {
                    int value = userIds[0];
                    _query = _query.Where(t => t.UserId == value);
                }
                else
                {
                    _query = _query.Where(t => userIds.Contains(t.UserId));
                }
            }
            if (exceptUserIds.IsNotNullOrEmpty())
            {
                if (exceptUserIds.Count == 1)
                {
                    int value = exceptUserIds[0];
                    _query = _query.Where(t => t.UserId != value);
                }
                else
                {
                    _query = _query.Where(t => !exceptUserIds.Contains(t.UserId));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByUserGroupIds(List<int> userGroupIds,
            List<EntityStatusEnum> memberStatuses,
            DateTime? memberValidFromBefore,
            DateTime? memberValidFromAfter,
            DateTime? memberValidToBefore,
            DateTime? memberValidToAfter)
        {
            if (userGroupIds.IsNotNullOrEmpty())
            {
                memberStatuses = memberStatuses.IsNotNullOrEmpty() ? memberStatuses : new List<EntityStatusEnum> { EntityStatusEnum.Active };

                if (userGroupIds.Count == 1)
                {
                    int value = userGroupIds[0];
                    _query = _query.Where(FilterStatusAndValidDay(value,
                                                                  memberStatuses,
                                                                  memberValidFromBefore,
                                                                  memberValidFromAfter,
                                                                  memberValidToBefore,
                                                                  memberValidToAfter));
                }
                else
                {
                    _query = _query.Where(FilterStatusAndValidDay(userGroupIds,
                                                                  memberStatuses,
                                                                  memberValidFromBefore,
                                                                  memberValidFromAfter,
                                                                  memberValidToBefore,
                                                                  memberValidToAfter));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByMultiUserGroupFilters(List<List<int>> multiUserGroupFilters,
           List<EntityStatusEnum> memberStatuses,
           DateTime? memberValidFromBefore,
           DateTime? memberValidFromAfter,
           DateTime? memberValidToBefore,
           DateTime? memberValidToAfter)
        {
            if (multiUserGroupFilters != null)
            {
                foreach (var userGroupIds in multiUserGroupFilters)
                {
                    FilterByUserGroupIds(userGroupIds, memberStatuses, memberValidFromBefore, memberValidFromAfter, memberValidToBefore, memberValidToAfter);
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByMultiUserTypeFilters(List<List<int>> multiUserTypefilters)
        {
            if (multiUserTypefilters != null)
            {
                foreach (var userTypeIds in multiUserTypefilters)
                {
                    FilterByUserTypeIds(userTypeIds);
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByMultiUserTypeExtIdFilters(List<List<string>> multiUserTypefilters)
        {
            if (multiUserTypefilters != null)
            {
                foreach (var userTypeIds in multiUserTypefilters)
                {
                    FilterByUserTypeExtIds(userTypeIds);
                }
            }
            return this;
        }

        public UserQueryBuilder FilterByEmails(List<string> emails)
        {
            if (emails.IsNotNullOrEmpty())
            {
                if (emails.Count == 1)
                {
                    string value = emails[0];
                    _query = _query.Where(t => t.Email == value);
                }
                else
                {
                    _query = _query.Where(p => emails.Contains(p.Email));
                }
            }
            return this;
        }

        private Expression<Func<UserEntity, bool>> FilterStatusAndValidDay(List<int> userGroupIds,
            List<EntityStatusEnum> memberStatuses,
            DateTime? memberValidFromBefore,
            DateTime? memberValidFromAfter,
            DateTime? memberValidToBefore,
            DateTime? memberValidToAfter)
        {
            return u => u.UGMembers.Any(j => userGroupIds.Contains(j.UserGroupId)
                    && (j.validFrom == null || (
                                            (!memberValidFromBefore.HasValue || j.validFrom <= memberValidFromBefore) &&
                                            (!memberValidFromAfter.HasValue || j.validFrom >= memberValidFromAfter)))
                    && (j.ValidTo == null || (
                                            (!memberValidToBefore.HasValue || j.ValidTo <= memberValidToBefore) &&
                                            (!memberValidToAfter.HasValue || j.ValidTo >= memberValidToAfter)))
                    && (memberStatuses.Contains(EntityStatusEnum.All) || memberStatuses.Contains((EntityStatusEnum)j.EntityStatusId)));
        }
        private Expression<Func<UserEntity, bool>> FilterStatusAndValidDay(int userGroupId,
            List<EntityStatusEnum> memberStatuses,
            DateTime? memberValidFromBefore,
            DateTime? memberValidFromAfter,
            DateTime? memberValidToBefore,
            DateTime? memberValidToAfter)
        {
            return u => u.UGMembers.Any(j => j.UserGroupId == userGroupId
                    && (j.validFrom == null || (
                                            (!memberValidFromBefore.HasValue || j.validFrom <= memberValidFromBefore) &&
                                            (!memberValidFromAfter.HasValue || j.validFrom >= memberValidFromAfter)))
                    && (j.ValidTo == null || (
                                            (!memberValidToBefore.HasValue || j.ValidTo <= memberValidToBefore) &&
                                            (!memberValidToAfter.HasValue || j.ValidTo >= memberValidToAfter)))
                    && (memberStatuses.Contains(EntityStatusEnum.All) || memberStatuses.Contains((EntityStatusEnum)j.EntityStatusId)));
        }
        public UserQueryBuilder FilterByStatusIds(List<EntityStatusEnum> statusIds)
        {
            if (!statusIds.IsNotNullOrEmpty())
            {
                _query = _query.Where(p => p.EntityStatusId.Value == (int)EntityStatusEnum.Active || p.EntityStatusId.Value == (int)EntityStatusEnum.New);
            }
            else if (!statusIds.Contains(EntityStatusEnum.All))
            {
                if (statusIds.Count == 1)
                {
                    var firsValue = (int)statusIds[0];
                    _query = _query.Where(p => p.EntityStatusId == firsValue);
                }
                else
                {
                    _query = _query.Where(p => statusIds.Contains((EntityStatusEnum)p.EntityStatusId.Value));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByArchetypeIds(List<ArchetypeEnum> archetypeIds)
        {
            if (archetypeIds.IsNotNullOrEmpty())
            {
                if (archetypeIds.Count == 1)
                {
                    int value = (int)archetypeIds[0];
                    _query = _query.Where(t => t.ArchetypeId == value);
                }
                else
                {
                    _query = _query.Where(p => archetypeIds.Contains((ArchetypeEnum)p.ArchetypeId.Value));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterBySsnList(List<string> ssnList)
        {
            if (ssnList.IsNotNullOrEmpty())
            {
                List<string> processedSSNList;
                if (_appSettings.EncryptSSN)
                {
                    processedSSNList = new List<string>();
                    foreach (var ssn in ssnList)
                    {
                        processedSSNList.Add(_userCryptoService.ComputeHashSsn(ssn));
                    }
                }
                else
                {
                    processedSSNList = ssnList;
                }
                if (processedSSNList.Count == 1)
                {
                    string value = processedSSNList[0];
                    _query = _query.Where(t => t.SSNHash == value);
                    //var s = _query.ToSql();
                }
                else
                {
                    var ss = string.Join(",", processedSSNList);
                    _query = _query.Where(p => processedSSNList.Contains(p.SSNHash));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByUserTypeExtIds(List<string> userTypeExtIds)
        {
            userTypeExtIds = userTypeExtIds.IsNotNullOrEmpty() ? userTypeExtIds.Where(t => t != null).ToList() : null;
            if (userTypeExtIds.IsNotNullOrEmpty())
            {
                if (userTypeExtIds.Count == 1)
                {
                    string value = userTypeExtIds[0];
                    _query = _query.Where(t => t.UT_Us.Any(x => x.UserType.ExtId == value));
                }
                else
                {
                    _query = _query.Where(p => p.UT_Us.Any(x => userTypeExtIds.Contains(x.UserType.ExtId)));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByUserTypeIds(List<int> userTypeIds)
        {
            if (userTypeIds.IsNotNullOrEmpty())
            {
                if (userTypeIds.Count == 1)
                {
                    var value = userTypeIds[0];
                    _query = _query.Where(t => t.UT_Us.Any(x => x.UserTypeId == value));
                }
                else
                {
                    _query = _query.Where(p => p.UT_Us.Any(x => userTypeIds.Contains(x.UserTypeId)));
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByUserNames(List<string> userNames)
        {
            if (userNames.IsNotNullOrEmpty())
            {
                if (userNames.Count == 1)
                {
                    string value = userNames[0];
                    _query = _query.Where(t => t.UserName == value);
                }
                else
                {
                    _query = _query.Where(p => userNames.Contains(p.UserName));
                }
            }
            return this;
        }

        public UserQueryBuilder FilterByOwnerId(int ownerId)
        {
            if (ownerId > 0)
            {
                _query = _query.Where(p => p.OwnerId == ownerId);
            }
            return this;
        }

        public UserQueryBuilder FilterByDate(DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter)
        {
            if (lastUpdatedBefore.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated <= lastUpdatedBefore);
            }

            if (lastUpdatedAfter.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated >= lastUpdatedAfter);
            }

            return this;
        }

        public UserQueryBuilder FilterByDate(
            DateTime? createdAfter, DateTime? createdBefore,
            DateTime? lastUpdatedAfter, DateTime? lastUpdatedBefore,
            DateTime? expirationDateAfter, DateTime? expirationDateBefore,
            DateTime? entityActiveDateAfter, DateTime? entityActiveDateBefore)
        {
            if (lastUpdatedBefore.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated <= lastUpdatedBefore);
            }

            if (lastUpdatedAfter.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated >= lastUpdatedAfter);
            }

            if (createdAfter.HasValue)
            {
                _query = _query.Where(t => t.Created >= createdAfter);
            }

            if (createdBefore.HasValue)
            {
                _query = _query.Where(t => t.Created <= createdBefore);
            }

            if (expirationDateAfter.HasValue)
            {
                _query = _query.Where(t => t.EntityExpirationDate >= expirationDateAfter);
            }

            if (expirationDateBefore.HasValue)
            {
                _query = _query.Where(t => t.EntityExpirationDate <= expirationDateBefore);
            }

            if (entityActiveDateAfter.HasValue)
            {
                _query = _query.Where(t => t.EntityActiveDate >= entityActiveDateAfter);
            }

            if (entityActiveDateBefore.HasValue)
            {
                _query = _query.Where(t => t.EntityActiveDate <= entityActiveDateBefore);
            }

            return this;
        }
        public UserQueryBuilder FilterByAges(List<AgeRange> ageRanges)
        {
            if (ageRanges.IsNotNullOrEmpty())
            {
                List<AgeRangePoints> agePointList = GetAgePointFromAgeRange(ageRanges);

                var searchPredicate = PredicateBuilder.New<UserEntity>();
                foreach (var item in agePointList)
                {
                    var currentYear = DateTime.Now.Year;
                    var to = item.End;
                    var from = item.Start;
                    var datetimeToYear = new DateTime(currentYear - to, 1, 1);
                    var datetimeFromYear = new DateTime(currentYear - from, 12, 31);
                    searchPredicate =
                    searchPredicate.Or(x =>
                    ((x.DateOfBirth.Value <= datetimeFromYear) && (x.DateOfBirth.Value >= datetimeToYear))
                    || (from == 0 && to == 0 && x.DateOfBirth == null));
                }

                _query = _query.AsExpandable().Where(searchPredicate);
            }
            return this;
        }

        public UserQueryBuilder FilterBySearchKey(string searchKey, bool enableSearchingSSN = true)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = searchKey.Trim();
                char space = ' ';
                var allKeys = searchKey.Split(space);
                if (allKeys.Length > 1)
                {
                    string term = string.Format("%{0}%", searchKey.Replace(' ', '%'));
                    _query = _query.Where(x => EF.Functions.Like(x.FirstName + x.LastName, term));
                }
                else
                {
                    if (enableSearchingSSN)
                    {
                        _query = _query.Where(x => x.FirstName.Contains(searchKey)
                                        || x.LastName.Contains(searchKey)
                                        || x.Email.Contains(searchKey)
                                        || x.Mobile.Contains(searchKey)
                                        || x.SSN.Contains(searchKey));
                    }
                    else
                    {
                        _query = _query.Where(x => x.FirstName.Contains(searchKey)
                                       || x.LastName.Contains(searchKey)
                                       || x.Email.Contains(searchKey)
                                       || x.UserName.Contains(searchKey));
                    }
                }
            }
            return this;
        }
        public UserQueryBuilder FilterByGenders(List<Gender> genders)
        {
            genders = genders == null ? new List<Gender>() : genders.Distinct().ToList();
            if (genders.IsNotNullOrEmpty())
            {
                if (genders.Count == 1)
                {
                    Gender value = genders[0];
                    if (value != Gender.Unknown)
                    {
                        _query = _query.Where(t => t.Gender == (int)value);
                    }
                    else
                    {
                        _query = _query.Where(t => t.Gender == null || t.Gender == (int)value);
                    }

                }
                else
                {
                    if (genders.Contains(Gender.Unknown))
                    {
                        _query = _query.Where(t => t.Gender == null || genders.Contains((Gender)t.Gender.Value));
                    }
                    else
                    {
                        _query = _query.Where(t => genders.Contains((Gender)t.Gender.Value));
                    }
                }
            }
            return this;
        }

        public UserQueryBuilder FilterByLoginServiceInfo(List<int> loginServiceIds = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null)
        {
            var hasFilterOnClaimValue = !loginServiceClaims.IsNullOrEmpty();
            var hasFilterOnClaimType = !loginServiceClaimTypes.IsNullOrEmpty();
            var hasFilterOnLoginServiceIds = !loginServiceIds.IsNullOrEmpty();

            if (!hasFilterOnClaimValue && !hasFilterOnClaimType && !hasFilterOnLoginServiceIds)
            {
                return this;
            }

            if (hasFilterOnClaimValue && !hasFilterOnLoginServiceIds && !hasFilterOnClaimType)
            {
                //This is popular filter in system, so we use native way to filter for best performance.


                if (loginServiceClaims.Count == 1)
                {
                    var claim = loginServiceClaims[0];
                    _query = _query.Where(q => q.LoginServiceUsers.Any(l => l.PrimaryClaimValue == claim));
                }
                else
                {
                    _query = _query.Where(q =>
                        q.LoginServiceUsers.Any(l => loginServiceClaims.Contains(l.PrimaryClaimValue)));

                }

                return this;
            }

            var paramIndex = 0;
            var whereExpression = new List<string>();
            var parametersValues = new List<object>();
            if (hasFilterOnLoginServiceIds)
            {
                if (loginServiceIds.Count == 1)
                {

                    whereExpression.Add($"l.LoginServiceId == @{paramIndex}");
                    parametersValues.Add(loginServiceIds[0]);

                }
                else
                {
                    whereExpression.Add($"@{paramIndex}.Contains(l.LoginServiceId)");
                    parametersValues.Add(loginServiceIds);

                }

                paramIndex++;

            }

            if (hasFilterOnClaimValue)
            {
                if (loginServiceClaims.Count == 1)
                {
                    whereExpression.Add($"l.PrimaryClaimValue == @{paramIndex}");
                    parametersValues.Add(loginServiceClaims[0]);
                }
                else if (loginServiceClaims.Count > 1)
                {
                    whereExpression.Add($"@{paramIndex}.Contains(l.PrimaryClaimValue)");
                    parametersValues.Add(loginServiceClaims);
                }

                paramIndex++;

            }

            if (hasFilterOnClaimType)
            {
                if (loginServiceClaimTypes.Count == 1)
                {
                    whereExpression.Add($"l.LoginService.PrimaryClaimType == @{paramIndex}");
                    parametersValues.Add(loginServiceClaimTypes[0]);
                }
                else
                {
                    whereExpression.Add($"@{paramIndex}.Contains(l.LoginService.PrimaryClaimType)");
                    parametersValues.Add(loginServiceClaimTypes);
                }

                paramIndex++;
            }


            if (whereExpression.Count > 0)
            {
                var where = $"LoginServiceUsers.Any(l=> {string.Join("&&", whereExpression)})";
                _query = _query.Where(where, parametersValues.ToArray());
            }


            return this;
        }

        public UserQueryBuilder FilterByDepartmentRoles(List<int> departmentRoles)
        {
            if (departmentRoles.IsNotNullOrEmpty())
            {
                _query = _query.Include(x => x.Department).ThenInclude(x => x.DT_Ds)
                               .Where(x => x.Department.DT_Ds.Any(a => departmentRoles.Contains(a.DepartmentTypeId)));
            }

            return this;
        }
        public UserQueryBuilder FilterByJsonValue(List<string> jsonDynamicData)
        {
            _query = _query.FilterByJsonValue(jsonDynamicData);
            return this;
        }

        public UserQueryBuilder IncludeDepartment(IncludeDepartmentOption includeDepartment)
        {

            switch (includeDepartment)
            {
                case IncludeDepartmentOption.Department:
                    _query = _query.Include(q => q.Department);
                    break;

                case IncludeDepartmentOption.DtDs:
                    _query = _query
                        .Include(q => q.Department)
                        .ThenInclude(x => x.DT_Ds);
                    break;

                case IncludeDepartmentOption.DepartmentType:
                    _query = _query
                        .Include(q => q.Department)
                        .ThenInclude(x => x.DT_Ds)
                        .ThenInclude(x => x.DepartmentType);
                    break;

                case IncludeDepartmentOption.LtDepartmentType:
                    _query = _query.Include(q => q.Department).ThenInclude(x => x.DT_Ds)
                        .ThenInclude(x => x.DepartmentType)
                        .ThenInclude(x => x.LT_DepartmentType);
                    break;
            }

            return this;
        }
        public UserQueryBuilder IncludeUserTypes(IncludeUserTypeOption includeUserType)
        {

            switch (includeUserType)
            {

                case IncludeUserTypeOption.UtUs:
                    _query = _query
                        .Include(q => q.UT_Us);
                    break;

                case IncludeUserTypeOption.UserType:
                    _query = _query
                        .Include(q => q.UT_Us)
                        .ThenInclude(x => x.UserType);
                    break;

                case IncludeUserTypeOption.LtUserType:
                    _query = _query
                        .Include(q => q.UT_Us)
                        .ThenInclude(x => x.UserType)
                        .ThenInclude(y => y.LT_UserType);
                    break;
            }

            return this;
        }

        public UserQueryBuilder IncludeUGMembers(IncludeUgMemberOption includeUGMember)
        {
            switch (includeUGMember)
            {
                case IncludeUgMemberOption.UgMember:
                    _query = _query.Include(q => q.UGMembers);
                    break;
                case IncludeUgMemberOption.UserGroup:
                    _query = _query.Include(q => q.UGMembers).ThenInclude(ug => ug.UserGroup);
                    break;
                case IncludeUgMemberOption.UserGroupUser:
                    _query = _query.Include(q => q.UGMembers)
                        .ThenInclude(ugm => ugm.UserGroup)
                        .ThenInclude(ug => ug.User);
                    break;
                case IncludeUgMemberOption.UgMemberUser:
                    _query = _query.Include(q => q.UGMembers).ThenInclude(ugm => ugm.User);
                    break;
                case IncludeUgMemberOption.UserGroup_UgMemberUser:
                    _query = _query.Include(q => q.UGMembers).ThenInclude(ugm => ugm.User);
                    _query = _query.Include(q => q.UGMembers).ThenInclude(ugm => ugm.UserGroup);
                    break;

            }

            return this;

        }

        private static ExpressionStarter<LoginServiceUserEntity> SearchLoginServiceUserWithClaimType(List<string> loginServiceClaimTypes, ExpressionStarter<LoginServiceUserEntity> searchLoginServiceUserPredicate)
        {
            if (loginServiceClaimTypes.IsNotNullOrEmpty())
            {
                if (loginServiceClaimTypes.Count == 1)
                {
                    var claimType = loginServiceClaimTypes[0];
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => claimType == lu.LoginService.PrimaryClaimType);
                }
                else
                {
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => loginServiceClaimTypes.Contains(lu.LoginService.PrimaryClaimType));
                }
            }
            return searchLoginServiceUserPredicate;
        }

        private static ExpressionStarter<LoginServiceUserEntity> SearchLoginServiceUserWithClaims(List<string> loginServiceClaims, ExpressionStarter<LoginServiceUserEntity> searchLoginServiceUserPredicate)
        {
            if (loginServiceClaims.IsNotNullOrEmpty())
            {
                if (loginServiceClaims.Count == 1)
                {
                    var claim = loginServiceClaims[0];
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => claim == lu.PrimaryClaimValue);
                }
                else
                {
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => loginServiceClaims.Contains(lu.PrimaryClaimValue));
                }
            }
            return searchLoginServiceUserPredicate;
        }

        private static ExpressionStarter<LoginServiceUserEntity> SearchLoginServiceUserWithLoginServiceIds(List<int> loginServiceIds, ExpressionStarter<LoginServiceUserEntity> searchLoginServiceUserPredicate)
        {
            if (loginServiceIds.IsNotNullOrEmpty())
            {
                if (loginServiceIds.Count == 1)
                {
                    var loginService = loginServiceIds[0];
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => loginService == lu.LoginServiceId);
                }
                else
                {
                    searchLoginServiceUserPredicate = searchLoginServiceUserPredicate.And(lu => loginServiceIds.Contains(lu.LoginServiceId));
                }
            }
            return searchLoginServiceUserPredicate;
        }

        private List<AgeRangePoints> GetAgePointFromAgeRange(List<AgeRange> ageRanges)
        {
            const int minAge = 0;
            const int maxAge = 200;
            const int ageStep = 9;
            const int underTwentyMaxAge = 19;
            List<AgeRangePoints> agePointList = new List<AgeRangePoints>();

            foreach (var age in ageRanges.Distinct())
            {
                switch (age)
                {
                    case AgeRange.UnderTwenty:
                        agePointList.Add(new AgeRangePoints { Start = minAge, End = underTwentyMaxAge });
                        break;
                    case AgeRange.Twenties:
                    case AgeRange.Thirties:
                    case AgeRange.Forties:
                        agePointList.Add(new AgeRangePoints { Start = (int)age, End = (int)age + ageStep });
                        break;
                    case AgeRange.FiftyAndGreater:
                        agePointList.Add(new AgeRangePoints { Start = (int)age, End = maxAge });
                        break;
                }
            }
            return agePointList;
        }

    }
    public static class Extensions
    {
        public static Expression<Func<T, bool>> ToExpression<T>(this ExpressionStarter<T> expr) => expr;
    }
}
