using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Business.Common;
using cxOrganization.Business.Exceptions;
using cxOrganization.Business.Extensions;
using cxOrganization.Client;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateUserService<TUser> : IDeactivateUserService<TUser> where TUser : UserDtoBase
    {
        private readonly ILogger _logger;

        private readonly IAdvancedWorkContext _workContext;
        /// <summary>
        /// User service to handle specific archetype user
        /// </summary>
        private readonly IUserService _userArchetypeService;


        /// <summary>
        /// User service to handle generic user
        /// </summary>
        private readonly IUserService _userService;

        private readonly ArchetypeEnum _userArchetype;


        private readonly DeactivateUserConfig _deactivateUserConfig;

        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IUGMemberService _ugMemberService;

        public DeactivateUserService(IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IUserService> userServiceFunc,
            OrganizationDbContext organizationUnitOfWork,
            IUserService userService,
            ILoginServiceUserService loginServiceUserService,
            IUGMemberService ugMemberService,
            ILoggerFactory loggerFactory,
            IOptions<DeactivateUserConfig> deactivateUserConfigOption)
        {
            _userArchetype = DomainDefinition.GetArchetype<TUser>();
            _workContext = workContext;
            _userArchetypeService = userServiceFunc(_userArchetype);
            _userService = userService;
            _organizationUnitOfWork = organizationUnitOfWork;
            _loginServiceUserService = loginServiceUserService;
            _deactivateUserConfig = deactivateUserConfigOption.Value;
            _ugMemberService = ugMemberService;
            _logger = loggerFactory.CreateLogger<DeactivateUserService<TUser>>();
        }

        public DeactivateUsersResultDto Deactivate(DeactivateUsersDto deactivateUsersDto)
        {
            ValidateDeactivateUserDto(deactivateUsersDto);

            IdentityDto deactivatedByIdentity = null;

            if (!deactivateUsersDto.SelfDeactivate)
            {
                deactivatedByIdentity = GetValidUpdatedIdentity(deactivateUsersDto);
            }

            var deactivateUserResults = DeactivateInternal(deactivateUsersDto.Identities, deactivateUsersDto.DeactivateLoginService, deactivateUsersDto.DeactivateMembership, deactivatedByIdentity, deactivateUsersDto.AcceptedEntityStatuses);

            return new DeactivateUsersResultDto()
            {
                UserResults = deactivateUserResults,
                UpdatedByIdentity = deactivatedByIdentity
            };
        }

        private List<DeactivateUserResultDto> DeactivateInternal(List<IdentityWithClaimDto> deactivatingIdenities, bool deactivateLoginService, bool deactivateMembership,
            IdentityDto deactivatedByIdentity, List<EntityStatusEnum> entityStatuses)
        {
            var deactivateUserResults = new List<DeactivateUserResultDto>();
            var acceptedStatusesForDeactivating = entityStatuses.IsNullOrEmpty()
                ? _deactivateUserConfig.AcceptedStatusesForDeactivating
                : entityStatuses;

            foreach (var identityWithClaimDto in deactivatingIdenities)
            {
                var existingUser = GetExistingUserArchetypeDto<TUser>(identityWithClaimDto, acceptedStatusesForDeactivating);
                DeactivateUserDetailResult detailResult;
                if (existingUser != null)
                {
                    identityWithClaimDto.Id = existingUser.Identity.Id;
                    identityWithClaimDto.ExtId = existingUser.Identity.ExtId;
                    detailResult = ExecuteDeactivateUser(deactivateLoginService, deactivateMembership, existingUser,
                        deactivatedByIdentity ?? existingUser.Identity, acceptedStatusesForDeactivating);
                }
                else
                {
                    detailResult = DeactivateUserDetailResult
                            .Create(MessageStatus.CreateNotFound(string.Format("{0} is not found", _userArchetype)),
                                MessageStatus.CreateNoContent(string.Format("Not performed due to {0} is not found", _userArchetype)),
                                MessageStatus.CreateNoContent(string.Format("Not performed due to {0} is not found", _userArchetype)));
                }

                var result = new DeactivateUserResultDto()
                {
                    Identity = identityWithClaimDto,
                    DetailtResult = detailResult
                };
                deactivateUserResults.Add(result);
            }
            return deactivateUserResults;
        }

        private DeactivateUserDetailResult ExecuteDeactivateUser(bool deactivateLoginService, bool deactiveMembership, TUser existingUser,
            IdentityDto deactivatedByIdentity, List<EntityStatusEnum> statusesForGettingToUpdate)
        {
            MessageStatus deactivateLoginServiceResult = null;
            MessageStatus deactivateMembershipResult = null;
            if (deactivateLoginService)
            {
                deactivateLoginServiceResult = DeactivateLoginService(existingUser);
            }

            if (deactiveMembership)
            {
                deactivateMembershipResult = DeactivateMembership(existingUser, statusesForGettingToUpdate, deactivatedByIdentity);
            }

            var deactivateUserObjectResult = DeactivateUserObject(existingUser, deactivatedByIdentity);

            return DeactivateUserDetailResult.Create(deactivateUserObjectResult, deactivateLoginServiceResult, deactivateMembershipResult);

        }

        private MessageStatus DeactivateMembership(TUser existingUser, List<EntityStatusEnum> statusesForGettingToUpdate, IdentityDto deactivatedByIdentity)
        {

            try
            {
                var existingMemberships = _ugMemberService.GetMemberships(_workContext.CurrentOwnerId, customerIds: new List<int> { existingUser.Identity.CustomerId },
                    userIds: new List<int> { (int)existingUser.Identity.Id }, ugMemberStatuses: statusesForGettingToUpdate);

                if (existingMemberships.Count > 0)
                {
                    foreach (var existingMembership in existingMemberships)
                    {
                        SetMembershipDeactivated(existingMembership, deactivatedByIdentity);
                    }
                    _ugMemberService.Update(existingMemberships);

                    return MessageStatus.CreateSuccess(string.Format("Deactivated {0} membership(s)", existingMemberships.Count));
                }
                return MessageStatus.CreateNoContent(string.Format("{0} has no membership", _userArchetype));

                //TODO: handle more membership with other data here
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return MessageStatus.CreateError("Error occurs when deactivating membership");
            }
        }

        private MessageStatus DeactivateUserObject(TUser existingUser, IdentityDto deactivatedByIdentity)
        {
            try
            {
                AnonymizeUser(existingUser);

                SetEmployeeToDeactivated(existingUser, deactivatedByIdentity);

                var hierarchyValidation = GetHierarchyDepartmentValidationSpecification(existingUser);

                _userArchetypeService.UpdateUser(hierarchyValidation, existingUser);

                return MessageStatus.CreateSuccess();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return MessageStatus.CreateError("Error occurs when deactivating user object");
            }

        }

        private MessageStatus DeactivateLoginService(TUser existingUser)
        {
            try
            {
                var loginServiceOfUsers = _loginServiceUserService.Get(userIds: new List<int> { (int)existingUser.Identity.Id });
                if (loginServiceOfUsers.Count > 0)
                {
                    foreach (var loginServiceOfUser in loginServiceOfUsers)
                    {
                        _loginServiceUserService.Delete((int)loginServiceOfUser.LoginServiceIdentity.Id, (int)loginServiceOfUser.UserIdentity.Id);
                    }
                    return MessageStatus.CreateSuccess(string.Format("Deactivated {0} login service(s)", loginServiceOfUsers.Count));
                }
                return MessageStatus.CreateNoContent(string.Format("{0} has no login service", _userArchetype));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return MessageStatus.CreateError(string.Format("Error occurs when deactivating login service of {0}", _userArchetype));
            }

        }

        protected virtual HierarchyDepartmentValidationSpecification GetHierarchyDepartmentValidationSpecification(TUser user)
        {
            return (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(user.GetParentDepartmentId(), ArchetypeEnum.Unknown)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();
        }
        private void AnonymizeUser(TUser user)
        {
            var dummyEmail = GenerateDummyEmail(user.Identity.ExtId);
            user.FirstName = "";
            user.LastName = "";
            user.MobileNumber = null;
            user.SSN = "";
            user.EmailAddress = dummyEmail;
        }
        private string GenerateDummyEmail(string userExtId)
        {
            if (string.IsNullOrEmpty(userExtId)) userExtId = Guid.NewGuid().ToString();
            return string.Format("{0}@dummy.net", userExtId.Replace("-", ".").Replace("@", "."));
        }
        private static void SetEmployeeToDeactivated(TUser user, IdentityDto deactivatedByIdentity)
        {
            user.ForceLoginAgain = true;
            SetEntityStatusToDeactivated(user, deactivatedByIdentity);

        }

        private static void SetMembershipDeactivated(MembershipDto membership, IdentityDto deactivatedByIdentity)
        {
            membership.ValidTo = DateTime.Now;
            SetEntityStatusToDeactivated(membership, deactivatedByIdentity);
        }
        private static void SetEntityStatusToDeactivated(ConexusBaseDto dto, IdentityDto deactivatedByIdentity)
        {
            dto.EntityStatus.StatusId = EntityStatusEnum.Deactive;
            dto.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Deactive_SynchronizedFromSource;
            dto.EntityStatus.LastUpdatedBy = (int)(deactivatedByIdentity.Id);
            dto.EntityStatus.LastUpdated = DateTime.Now;
        }
        private IdentityDto GetValidUpdatedIdentity(DeactivateUsersDto deactivateUsersDto)
        {
            var updatedIdentityStatusDto = GetExistingIdentityStatusDto(deactivateUsersDto.UpdatedByIdentity);
            if (updatedIdentityStatusDto == null)
            {
                throw new NotFoundException(string.Format("{0} is not found", deactivateUsersDto.UpdatedByIdentity));
            }
            if (_deactivateUserConfig.DeactivateUserByRoles.IsNotNullOrEmpty())
            {
                var existingRoleMemberships = _userService.GetUserMemberships((int)updatedIdentityStatusDto.Identity.Id.Value,
                    updatedIdentityStatusDto.Identity.Archetype,
                    membershipsArchetypeIds: new List<ArchetypeEnum> { ArchetypeEnum.Role },
                    membershipExtIds: _deactivateUserConfig.DeactivateUserByRoles);

                if (existingRoleMemberships.Count == 0)
                {
                    throw new InvalidException(string.Format("{0} is required to have role in list '{1}' to execute deactivating {2}",
                        updatedIdentityStatusDto.ToStringInfo(), string.Join(",", _deactivateUserConfig.DeactivateUserByRoles), _userArchetype));
                }
            }
            return updatedIdentityStatusDto.Identity;
        }

        private IdentityStatusDto GetExistingIdentityStatusDto(IdentityWithClaimDto updatedIdentityWithClaimDto)
        {
            return GetExistingUser<IdentityStatusDto>(_userService, updatedIdentityWithClaimDto, null);
        }
        private T GetExistingUserArchetypeDto<T>(IdentityWithClaimDto updatedIdentityWithClaimDto,
            List<EntityStatusEnum> statusEnums) where T : UserDtoBase
        {
            return GetExistingUser<T>(_userArchetypeService, updatedIdentityWithClaimDto, statusEnums);
        }

        private T GetExistingUser<T>(IUserService userService,
            IdentityWithClaimDto updatedIdentityWithClaimDto,
            List<EntityStatusEnum> statusEnums) where T : ConexusBaseDto
        {

            if (updatedIdentityWithClaimDto.Id > 0)
            {
                return userService.GetUsers<T>(updatedIdentityWithClaimDto.OwnerId,
                        new List<int> { (int)updatedIdentityWithClaimDto.CustomerId },
                        userIds: new List<int> { (int)updatedIdentityWithClaimDto.Id.Value },
                        archetypeIds: new List<ArchetypeEnum> { updatedIdentityWithClaimDto.Archetype },
                        statusIds: statusEnums)
                    .Items.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(updatedIdentityWithClaimDto.ExtId))
            {
                return userService.GetUsers<T>(_workContext.CurrentOwnerId,
                          new List<int> { (int)updatedIdentityWithClaimDto.CustomerId },
                        extIds: new List<string> { updatedIdentityWithClaimDto.ExtId },
                        archetypeIds: new List<ArchetypeEnum> { updatedIdentityWithClaimDto.Archetype },
                        statusIds: statusEnums)
                    .Items.FirstOrDefault();
            }

            if (updatedIdentityWithClaimDto.Claims != null
                && updatedIdentityWithClaimDto.Claims.Count > 0)
            {
                return userService.GetUsers<T>(updatedIdentityWithClaimDto.OwnerId,
                        new List<int> { (int)updatedIdentityWithClaimDto.CustomerId },
                        loginServiceClaimTypes: updatedIdentityWithClaimDto.Claims.Keys.ToList(),
                        loginServiceClaims: updatedIdentityWithClaimDto.Claims.Values.ToList(),
                        archetypeIds: new List<ArchetypeEnum> { updatedIdentityWithClaimDto.Archetype },
                        statusIds: statusEnums)
                    .Items.FirstOrDefault();
            }
            return null;
        }

        private void ValidateDeactivateUserDto(DeactivateUsersDto deactivateUsersDto)
        {
            if (deactivateUsersDto.Identities.IsNullOrEmpty())
                throw new InvalidException("Identities is null or empty");

            if (deactivateUsersDto.SelfDeactivate != true
                && deactivateUsersDto.UpdatedByIdentity == null)
                throw new InvalidException("UpdatedByIdentity is required when this isn't self deactivating");

            if (deactivateUsersDto.Identities.Any(i => i.Archetype != _userArchetype))
            {
                throw new InvalidException(string.Format("Identities contain element that has invalid archetype. Expected archetype is {0}.", _userArchetype));
            }

        }
    }
}
