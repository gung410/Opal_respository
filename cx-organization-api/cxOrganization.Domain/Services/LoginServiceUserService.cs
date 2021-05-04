using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Services
{

    public class LoginServiceUserService : ILoginServiceUserService
    {
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IAdvancedWorkContext _workContext;
        private readonly ILoginServiceUserRepository _loginServiceUserRepository;
        private readonly ILoginServiceRepository _loginServiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginServiceUserService> _logger;

        public LoginServiceUserService(OrganizationDbContext organizationDbContext,
            IAdvancedWorkContext workContext,
            ILoginServiceUserRepository loginServiceUserRepository,
            ILoginServiceRepository loginServiceRepository,
            IUserRepository userRepository, ILogger<LoginServiceUserService> logger)
        {
            _organizationDbContext = organizationDbContext;
            _loginServiceUserRepository = loginServiceUserRepository;
            _workContext = workContext;
            _loginServiceRepository = loginServiceRepository;
            _userRepository = userRepository;
            _logger = logger;
        }


        public LoginServiceUserDto Insert(LoginServiceUserDto insertingLoginServiceUserDto)
        {
            var userEnity = GetUserEntity(insertingLoginServiceUserDto.UserIdentity, insertingLoginServiceUserDto.AcceptedUserEntityStatuses);
            var loginServiceEnity = GetLoginServiceEntity(insertingLoginServiceUserDto.LoginServiceIdentity);

            var existingLoginServiceUser = _loginServiceUserRepository.GetLoginServiceUser(userEnity.UserId, loginServiceEnity.LoginServiceID);
            if (existingLoginServiceUser != null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                    string.Format("Login service claim already exists for login service id {0} and user id {1}", loginServiceEnity.LoginServiceID, userEnity.UserId));

            }
            var insertingLoginServiceEntity = new LoginServiceUserEntity()
            {
                UserId = userEnity.UserId,
                LoginServiceId = loginServiceEnity.LoginServiceID,
                PrimaryClaimValue = insertingLoginServiceUserDto.ClaimValue ?? string.Empty,
                Created = insertingLoginServiceUserDto.Created ?? DateTime.Now
            };

            LoginServiceUserEntity insertedLoginServiceEntity;

            insertedLoginServiceEntity = _loginServiceUserRepository.Insert(insertingLoginServiceEntity);
            _organizationDbContext.SaveChanges();
            insertedLoginServiceEntity.User = userEnity;
            insertedLoginServiceEntity.LoginService = loginServiceEnity;

            return MapToLoginServiceUserDtoFromEntity(insertedLoginServiceEntity);
        }

        public LoginServiceUserDto InsertOrUpdate(LoginServiceUserDto insertingLoginServiceUserDto)
        {
            var userEnity = GetUserEntity(insertingLoginServiceUserDto.UserIdentity, insertingLoginServiceUserDto.AcceptedUserEntityStatuses);
            var loginServiceEnity = GetLoginServiceEntity(insertingLoginServiceUserDto.LoginServiceIdentity);

            var existingLoginServiceUser = _loginServiceUserRepository.GetLoginServiceUser(userEnity.UserId, loginServiceEnity.LoginServiceID);
            if (existingLoginServiceUser != null)
            {
                existingLoginServiceUser.PrimaryClaimValue = insertingLoginServiceUserDto.ClaimValue ?? string.Empty;

                LoginServiceUserEntity updatedLoginServiceEntity;

                updatedLoginServiceEntity = _loginServiceUserRepository.Update(existingLoginServiceUser);
                _organizationDbContext.SaveChanges();

                updatedLoginServiceEntity.User = userEnity;
                updatedLoginServiceEntity.LoginService = loginServiceEnity;
                return MapToLoginServiceUserDtoFromEntity(updatedLoginServiceEntity);
            }
            var insertingLoginServiceEntity = new LoginServiceUserEntity()
            {
                UserId = userEnity.UserId,
                LoginServiceId = loginServiceEnity.LoginServiceID,
                PrimaryClaimValue = insertingLoginServiceUserDto.ClaimValue ?? string.Empty,
                Created = insertingLoginServiceUserDto.Created ?? DateTime.Now
            };

            LoginServiceUserEntity insertedLoginServiceEntity;

            insertedLoginServiceEntity = _loginServiceUserRepository.Insert(insertingLoginServiceEntity);
            _organizationDbContext.SaveChanges();
            insertedLoginServiceEntity.User = userEnity;
            insertedLoginServiceEntity.LoginService = loginServiceEnity;

            return MapToLoginServiceUserDtoFromEntity(insertedLoginServiceEntity);
        }

        public LoginServiceUserDto Update(LoginServiceUserDto updatingLoginServiceUserDto)
        {
            var userEnity = GetUserEntity(updatingLoginServiceUserDto.UserIdentity, updatingLoginServiceUserDto.AcceptedUserEntityStatuses);
            var loginServiceEnity = GetLoginServiceEntity(updatingLoginServiceUserDto.LoginServiceIdentity);

            var existingLoginServiceUser = _loginServiceUserRepository.GetLoginServiceUser(userEnity.UserId, loginServiceEnity.LoginServiceID, forChanging: true);
            if (existingLoginServiceUser == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND,
                    string.Format("LoginServiceUser with login service id {0} and user id {1}", loginServiceEnity.LoginServiceID, userEnity.UserId));

            }

            existingLoginServiceUser.PrimaryClaimValue = updatingLoginServiceUserDto.ClaimValue ?? string.Empty;

            LoginServiceUserEntity updatedLoginServiceEntity;

            updatedLoginServiceEntity = _loginServiceUserRepository.Update(existingLoginServiceUser);
            _organizationDbContext.SaveChanges();

            updatedLoginServiceEntity.User = userEnity;
            updatedLoginServiceEntity.LoginService = loginServiceEnity;
            return MapToLoginServiceUserDtoFromEntity(updatedLoginServiceEntity);
        }
        public LoginServiceUserDto Delete(LoginServiceUserDto deletingLoginServiceUserDto)
        {
            var userEnity = GetUserEntity(deletingLoginServiceUserDto.UserIdentity, deletingLoginServiceUserDto.AcceptedUserEntityStatuses);
            var loginServiceEnity = GetLoginServiceEntity(deletingLoginServiceUserDto.LoginServiceIdentity);

            var existingLoginServiceUser = _loginServiceUserRepository.GetLoginServiceUser(userEnity.UserId, loginServiceEnity.LoginServiceID, forChanging: true);
            if (existingLoginServiceUser == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND,
                    string.Format("LoginServiceUser with login service id {0} and user id {1}", loginServiceEnity.LoginServiceID, userEnity.UserId));

            }


            _loginServiceUserRepository.Delete(existingLoginServiceUser);
            _organizationDbContext.SaveChanges();

            existingLoginServiceUser.User = userEnity;
            existingLoginServiceUser.LoginService = loginServiceEnity;
            return MapToLoginServiceUserDtoFromEntity(existingLoginServiceUser);
        }
        public void Delete(int loginServiceId, int userId)
        {
            var existingLoginServiceUser = _loginServiceUserRepository.GetLoginServiceUser(userId, loginServiceId, forChanging: true);
            if (existingLoginServiceUser == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND,
                    string.Format("LoginServiceUser with login service id {0} and user id {1}", loginServiceId, userId));

            }

            _loginServiceUserRepository.Delete(existingLoginServiceUser);
            _organizationDbContext.SaveChanges();

        }
        public List<LoginServiceUserDto> Get(List<int> userIds = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<int> loginServiceIds = null,
            List<string> primaryClaimTypes = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            List<int> siteIds = null,
            bool? includeLoginServiceHasNullSiteId = null,
            List<string> claimValues = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null
        )
        {
            var existingLoginServiceUsers = _loginServiceUserRepository.GetLoginServiceUsers(
                ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                userIds: userIds,
                userExtIds: userExtIds,
                userArchetypes: userArchetypes,
                loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes,
                userEntityStatuses: userEntityStatuses,
                siteIds: siteIds,
                includeLoginServiceHasNullSiteId: includeLoginServiceHasNullSiteId,
                claimValues: claimValues,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                includeUser: true,
                includeLoginService: true);

            return existingLoginServiceUsers.Select(MapToLoginServiceUserDtoFromEntity).ToList();
        }

        private LoginServiceUserDto MapToLoginServiceUserDtoFromEntity(LoginServiceUserEntity loginServiceEntity)
        {
            var userEnity = loginServiceEntity.User;
            var loginServiceEnity = loginServiceEntity.LoginService;
            return new LoginServiceUserDto
            {
                UserIdentity = new IdentityDto()
                {
                    Archetype = (ArchetypeEnum)(userEnity.ArchetypeId ?? 0),
                    Id = userEnity.UserId,
                    ExtId = userEnity.ExtId,
                    OwnerId = userEnity.OwnerId,
                    CustomerId = userEnity.CustomerId ?? _workContext.CurrentCustomerId
                },
                LoginServiceIdentity = new LoginServiceIdentityDto()
                {
                    Id = loginServiceEnity.LoginServiceID,
                    ExtId = loginServiceEnity.PrimaryClaimType,
                    SiteId = loginServiceEnity.SiteID

                },
                Created = loginServiceEntity.Created,
                ClaimValue = loginServiceEntity.PrimaryClaimValue
            };
        }

        private UserEntity GetUserEntity(IdentityDto userIdentityDto, List<EntityStatusEnum> acceptedEntityStatuses)
        {
            var customerId = userIdentityDto.CustomerId > 0 ? userIdentityDto.CustomerId : _workContext.CurrentCustomerId;
            var ownerId = userIdentityDto.OwnerId > 0 ? userIdentityDto.OwnerId : _workContext.CurrentOwnerId;
            var customerIds = new List<int> { customerId };
            var archetypeIds = new List<ArchetypeEnum> { userIdentityDto.Archetype };
            List<int> userIds = null;
            List<string> userExtIds = null;

            if (userIdentityDto.Id > 0)
            {
                userIds = new List<int> { (int)userIdentityDto.Id.Value };
            }
            if (!string.IsNullOrEmpty(userIdentityDto.ExtId))
            {
                userExtIds = new List<string> { userIdentityDto.ExtId };
            }
            if (userIds == null && userExtIds == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                    string.Format("Unable to get user with archetype '{0}' since missing identity ", userIdentityDto.Archetype));
            }

            if (acceptedEntityStatuses == null)
            {
                acceptedEntityStatuses = new List<EntityStatusEnum>
                {
                    EntityStatusEnum.Active, EntityStatusEnum.Pending,EntityStatusEnum.Deactive, EntityStatusEnum.PendingApproval1st,
                    EntityStatusEnum.PendingApproval2nd, EntityStatusEnum.PendingApproval3rd
                };
            }

            var userEntity = _userRepository.GetUsers(ownerId: ownerId, customerIds: customerIds,
                archetypeIds: archetypeIds, userIds: userIds, extIds: userExtIds,
                statusIds: acceptedEntityStatuses
            ).Items.FirstOrDefault();

            if (userEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, BuildUserNotFoundMessage(userIdentityDto, ownerId, customerId));
            }
            return userEntity;
        }


        private LoginServiceEntity GetLoginServiceEntity(LoginServiceIdentityDto loginServiceIdentityDto)
        {
            if (loginServiceIdentityDto == null)
            {
                _logger.LogWarning("Missing LoginService info, trying to find default LoginService.");
                var defaultLoginService = _loginServiceRepository.GetLoginServices().FirstOrDefault();
                if (defaultLoginService == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, "Default LoginService.");
                }
                else
                {
                    _logger.LogWarning($"Using default LoginService: {defaultLoginService.LoginServiceID}");
                }
                return defaultLoginService;
            }
            var loginServiceIds = loginServiceIdentityDto.Id > 0 ? new List<int> { (int)loginServiceIdentityDto.Id.Value } : null;

            //Since login service does not have extId, for now, we map it with primary claim value for identifying entity
            var primaryClaimTypes = !string.IsNullOrEmpty(loginServiceIdentityDto.ExtId) ? new List<string> { loginServiceIdentityDto.ExtId } : null;

            var siteIds = loginServiceIdentityDto.SiteId > 0 ? new List<int> { loginServiceIdentityDto.SiteId.Value } : null;
            var loginServiceEntity = _loginServiceRepository.GetLoginServices(loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes, siteIds: siteIds).FirstOrDefault();

            if (loginServiceEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, BuildLoginServiceNotFoundMessage(loginServiceIdentityDto));
            }
            return loginServiceEntity;
        }
        private static string BuildUserNotFoundMessage(IdentityDto userIdentityDto, int ownerId, int customerId)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("The active/pending user with archetype {0}", userIdentityDto.Archetype);
            if (userIdentityDto.Id > 0)
                messageBuilder.AppendFormat(", id '{0}'", userIdentityDto.Id);
            if (!string.IsNullOrEmpty(userIdentityDto.ExtId))
                messageBuilder.AppendFormat(", extId '{0}'", userIdentityDto.ExtId);
            messageBuilder.AppendFormat(" in customer {0}, owner {1}", ownerId, customerId);
            return messageBuilder.ToString();
        }

        private static string BuildLoginServiceNotFoundMessage(LoginServiceIdentityDto loginServiceIdentityDto)
        {
            var messageBuilder = new StringBuilder("Login service with ");
            if (loginServiceIdentityDto.Id > 0)
                messageBuilder.AppendFormat("id '{0}', ", loginServiceIdentityDto.Id);
            if (!string.IsNullOrEmpty(loginServiceIdentityDto.ExtId))
                messageBuilder.AppendFormat("primary claim type '{0}', ", loginServiceIdentityDto.ExtId);
            if (loginServiceIdentityDto.SiteId > 0)
                messageBuilder.AppendFormat("siteId {0}", loginServiceIdentityDto.SiteId);

            return string.Format("{0}.", messageBuilder.ToString().TrimEnd(' ', ','));
        }
    }
}
