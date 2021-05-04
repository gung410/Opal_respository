using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.Processor.Event;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Datahub.Processor.Base.ProcessorRegister;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Processor
{

    public abstract class SyncIdpUserInfoEventHandlerBase : ActionHandlerBase, IActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        protected SyncIdpUserInfoEventHandlerBase(ILogger<SyncIdpUserInfoEventHandlerBase> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract override string Action { get; }

        public override Func<dynamic, Task> Handler => async dynamicObject =>
        {
            await Task.Run(() =>
            {
                if (dynamicObject.Type != "event")
                    return;
                var messageStr = JsonConvert.SerializeObject(dynamicObject);
                IdpEvent message = JsonConvert.DeserializeObject<IdpEvent>(messageStr);
                try
                {
                    HandleEventAsync(message);
                }
                catch (Exception ex)
                {
                    int retry = 1;
                    if (ex.Message == MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED)
                    {
                        _logger.LogError($"ERROR_ENTITY_VERSION_INCORRECTED {message.Routing.EntityId} exception occured");
                        // retry 5 time if ERROR_ENTITY_VERSION_INCORRECTED occures
                        while (retry < 6)
                        {
                            _logger.LogError($"ERROR_ENTITY_VERSION_INCORRECTED {message.Routing.EntityId} retry process message {retry} time(s)");

                            try
                            {
                                HandleEventAsync(message);
                                _logger.LogError($"ERROR_ENTITY_VERSION_INCORRECTED {message.Routing.EntityId} retry process message success at {retry} time");
                                break;
                            }
                            catch (Exception retryEx)
                            {
                                if (retryEx.Message == MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED)
                                {
                                    Thread.Sleep(50);
                                }
                                else
                                {
                                    _logger.LogError(retryEx, $"ERROR_ENTITY_VERSION_INCORRECTED {message.Routing.EntityId} got {retryEx.Message} when retrying process user, skip retry processing" );
                                    break;
                                }
                            }
                            retry++;
                        }
                    }
                    else
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            });
        };

        protected abstract void SetUpdatingValueForUser(UserGenericDto user, IdpEvent eventData);
        private void HandleEventAsync(IdpEvent eventData)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var loginServiceUserRepo = scope.ServiceProvider.GetService<ILoginServiceUserRepository>();
                var userserviceFunc = scope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>();
                var user = loginServiceUserRepo.GetLoginServiceUsers(primaryClaimValues: new List<string> { eventData.Routing.EntityId }, userEntityStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All }).FirstOrDefault();
                if (user == null)
                {
                    _logger.LogWarning($"Sync 'User-Edit' operation aborted: User with external id {eventData.Routing.EntityId} is not mapped to our database");
                    return;
                }
                _logger.LogInformation($"Sync 'User-Edit' operation started. User external id: ({eventData.Routing.EntityId})");

                var workContext = scope.ServiceProvider.GetService<IAdvancedWorkContext>();
                workContext.IsEnableFiltercxToken = false;

                workContext.UserIdCXID = eventData.Payload?.Identity?.UserId;

                var needLoginServiceClaim = string.IsNullOrEmpty(workContext.UserIdCXID);

                var userService = userserviceFunc(ArchetypeEnum.Unknown);
                var userDb = userService.GetUsers<UserGenericDto>(userIds: new List<int> {user.UserId},
                        statusIds: new List<EntityStatusEnum> {EntityStatusEnum.All}, ignoreCheckReadUserAccess: true,
                        getLoginServiceClaims: needLoginServiceClaim)
                    .Items
                    .FirstOrDefault();

                workContext.IsEnableFiltercxToken = true;
                if (userDb == null)
                {
                    _logger.LogError($"User {user.UserId} is not found. Sync operation is aborted. User event {eventData.Routing.Action} updated: ({eventData.Routing.EntityId})");
                    return;
                }
                workContext.CurrentCustomerId = userDb.Identity.CustomerId;
                workContext.CurrentOwnerId = userDb.Identity.OwnerId;
                if (needLoginServiceClaim && userDb.LoginServiceClaims != null && userDb.LoginServiceClaims.Count > 1)
                {
                    workContext.UserIdCXID = userDb.LoginServiceClaims.First().Value;
                }

                var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                //hard code for now
                .ValidateDepartment(userDb.DepartmentId, ArchetypeEnum.Unknown)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

                SetUpdatingValueForUser(userDb, eventData);

                var userUpdate = userService.UpdateUser(validationSpecification, userDb);
                _logger.LogInformation($"Sync operation succeed. User event {eventData.Routing.Action} updated: ({eventData.Routing.EntityId}-{userUpdate.Identity.Id})");
            }
        }
    }
}
