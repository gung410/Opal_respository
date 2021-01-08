using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.SAM.Messages;
using Microservice.Analytics.Common.Helpers;
using Microservice.Analytics.Domain.Entities;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.SAM
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    public class UserCreatedOrUpdatedConsumer : ScopedOpalMessageConsumer<UserChangeMessage>
    {
        private readonly ILogger<UserCreatedOrUpdatedConsumer> _logger;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;
        private readonly IReadOnlyRepository<SAM_Department, string> _departmentRepository;

        public UserCreatedOrUpdatedConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserHistory> userHistoryRepository,
            IReadOnlyRepository<SAM_Department, string> departmentRepository)
        {
            _logger = loggerFactory.CreateLogger<UserCreatedOrUpdatedConsumer>();
            _userHistoryRepository = userHistoryRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task InternalHandleAsync(UserChangeMessage message)
        {
            if (message.UserData?.Identity?.ExtId == null)
            {
                _logger.LogError($"UserCreatedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var historyItemsOfUser = await _userHistoryRepository.GetAllListAsync(t => t.UserId == message.UserData.Identity.ExtId && t.ToDate == null);

            var now = Clock.Now;

            var newHistoryOfUser = await BuildEntityFromMessageAsync(message);
            if (historyItemsOfUser != null && historyItemsOfUser.Any())
            {
                var latestUserHistory = historyItemsOfUser.OrderByDescending(t => t.No).First();

                if (newHistoryOfUser.EntityStatusId == (int)AnalyticSAMStatus.Deactive)
                {
                    // we update ToDate for case user has been de-active, don't insert new record
                    foreach (var historyItem in historyItemsOfUser)
                    {
                        historyItem.ToDate = now;
                    }

                    await _userHistoryRepository.UpdateManyAsync(historyItemsOfUser);
                }
                else if (CheckDifferentHelper.HasDifferent(latestUserHistory, newHistoryOfUser, IgnoreFieldCheckDiffPredicate))
                {
                    // we set ToDate for case insert new record
                    foreach (var historyItem in historyItemsOfUser)
                    {
                        historyItem.ToDate = now;
                    }

                    await _userHistoryRepository.UpdateManyAsync(historyItemsOfUser);
                    await _userHistoryRepository.InsertAsync(newHistoryOfUser);
                }
                else
                {
                    // we update last login date for case the user has been updated LastLoginDate only, don't insert new record
                    latestUserHistory.LastLoginDate = message.UserData.JsonDynamicAttributes.LastLoginDate;
                    await _userHistoryRepository.UpdateAsync(latestUserHistory);
                }
            }
            else
            {
                await _userHistoryRepository.InsertAsync(newHistoryOfUser);
            }
        }

        private static bool IgnoreFieldCheckDiffPredicate(PropertyInfo x)
        {
            return x.Name != nameof(SAM_UserHistory.FromDate) &&
                   x.Name != nameof(SAM_UserHistory.ToDate) &&
                   x.Name != nameof(SAM_UserHistory.No) &&
                   x.Name != nameof(SAM_UserHistory.Id) &&
                   x.Name != nameof(SAM_UserHistory.LastLoginDate) &&
                   !typeof(IList).IsAssignableFrom(x.PropertyType);
        }

        private async Task<SAM_UserHistory> BuildEntityFromMessageAsync(UserChangeMessage message)
        {
            var now = Clock.Now;
            var department = await _departmentRepository.FirstOrDefaultAsync(t => t.ExtId == message.UserData.DepartmentId);
            var numOfHistory = await _userHistoryRepository.CountAsync(t => t.UserId == message.UserData.Identity.ExtId);

            DateTime? entityActiveDate = null;
            if (message.UserData.JsonDynamicAttributes.LastEntityStatusId != null &&
                message.UserData.JsonDynamicAttributes.LastEntityStatusId != (int)AnalyticSAMStatus.Active &&
                message.UserData.EntityStatus.StatusId == AnalyticSAMStatus.Active)
            {
                entityActiveDate = now;
            }

            return new SAM_UserHistory()
            {
                UserId = message.UserData.Identity.ExtId.Value,
                No = numOfHistory++,
                ArcheTypeId = ((int)message.UserData.Identity.Archetype).ToString(),
                FromDate = now,
                DepartmentId = department?.Id.ToString(),
                FirstName = message.UserData.FirstName,
                LastName = message.UserData.LastName,
                Email = message.UserData.EmailAddress,
                DateOfBirth = null,
                CountryCode = message.UserData.MobileCountryCode,
                Gender = message.UserData.Gender,
                Locked = message.UserData.EntityStatus.ExternallyMastered ? (short)1 : (short)0,
                EntityExpirationDate = message.UserData.EntityStatus.ExpirationDate,
                EntityActiveDate = entityActiveDate,
                IsDeleted = message.UserData.EntityStatus.Deleted,
                Roles = string.Join(',', message.UserData.SystemRoles?.Select(t => t.LocalizedData.FirstOrDefault(t => t.LanguageCode == "en-US")?.Fields.FirstOrDefault(t => t.Name == "Name")?.LocalizedText).ToList()),
                ServiceSchemeId = message.UserData.PersonnelGroups.FirstOrDefault()?.Identity.ExtId,
                DesignationId = message.UserData.JsonDynamicAttributes.Designation,
                FinishOnBoarding = message.UserData.JsonDynamicAttributes.FinishOnBoarding,
                SentWelcomeEmail = message.UserData.JsonDynamicAttributes.SentWelcomeEmail,
                SentWelcomeEmailDate = message.UserData.JsonDynamicAttributes.SentWelcomeEmailDate,
                EntityStatusId = (int)message.UserData.EntityStatus.StatusId,
                EntityStatusReasonId = (int)message.UserData.EntityStatus.StatusReasonId,
                StartedOnboardingDate = message.UserData.JsonDynamicAttributes.StartedOnboardingDate,
                FinishedOnboardingDate = message.UserData.JsonDynamicAttributes.FinishedOnboardingDate,
                LastLoginDate = message.UserData.JsonDynamicAttributes.LastLoginDate,
                FirstLoginDate = message.UserData.JsonDynamicAttributes.FirstLoginDate,
                NotificationPreference = string.Join(',', message.UserData.JsonDynamicAttributes.NotificationPreference),
                DateAppointedToService = message.UserData.JsonDynamicAttributes.DateAppointedToService,
                DateToCurrentScheme = message.UserData.JsonDynamicAttributes.DateToCurrentScheme,
                DateAppointedToTrainedGrade = message.UserData.JsonDynamicAttributes.DateAppointedToTrainedGrade,
                HoldsSupervisoryRole = message.UserData.JsonDynamicAttributes.HoldsSupervisoryRole,
                ExtId = message.UserData.Identity.Id.ToString(),
                UserExtId = message.UserData.Identity.ExtId.Value.ToString(),
                MOEStaff = message.UserData.EntityStatus.ExternallyMastered,
                DevelopmentRoleID = message.UserData.DevelopmentalRoles.FirstOrDefault()?.Identity.ExtId,
                DepartmentExtId = message.UserData.DepartmentId,
                Created = message.UserData.Created,
                LastUpdated = message.UserData.EntityStatus.LastUpdated
            };
        }
    }
}
