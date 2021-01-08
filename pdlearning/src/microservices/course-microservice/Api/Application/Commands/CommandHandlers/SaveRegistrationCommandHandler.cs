using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveRegistrationCommandHandler : BaseCommandHandler<SaveRegistrationCommand>
    {
        private readonly ProcessRegistrationLogic _processRegistration;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly RegistrationCudLogic _registrationCudLogic;

        public SaveRegistrationCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            ProcessRegistrationLogic processRegistration,
            SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            WebAppLinkBuilder webAppLinkBuilder) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _processRegistration = processRegistration;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _registrationCudLogic = registrationCudLogic;
        }

        protected override async Task HandleAsync(SaveRegistrationCommand command, CancellationToken cancellationToken)
        {
            var newRegistrations = command.Registrations
                .Select(registration => new Registration
                {
                    Id = registration.Id,
                    UserId = CurrentUserIdOrDefault,
                    ApprovingOfficer = command.ApprovingOfficer,
                    AlternativeApprovingOfficer = command.AlternativeApprovingOfficer,
                    ClassRunId = registration.ClassRunId,
                    CourseId = registration.CourseId,
                    Status = RegistrationStatus.PendingConfirmation,
                    RegistrationDate = Clock.Now,
                    RegistrationType = command.RegistrationType,
                    CreatedBy = CurrentUserIdOrDefault,
                    CreatedDate = Clock.Now
                })
                .ToList();
            await _processRegistration.RegisterNewForNotAddedByCA(newRegistrations, CurrentUserIdOrDefault, cancellationToken);

            await _registrationCudLogic.InsertMany(newRegistrations, cancellationToken);

            await SendNotificationWhenLearnerRegisterClassrun(newRegistrations.Where(p => p.Status == RegistrationStatus.PendingConfirmation).ToList());
        }

        protected override Task<Validation<SaveRegistrationCommand>> ValidateCommand(SaveRegistrationCommand command)
        {
            return Task.FromResult(Validation.ValidIf(
                command,
                command.ApprovingOfficer != Guid.Empty || (command.AlternativeApprovingOfficer.HasValue && command.AlternativeApprovingOfficer != Guid.Empty),
                "There isn’t any AO assigned to you. Therefore, you cannot apply a course. Please contact your administrator for further support."));
        }

        private async Task SendNotificationWhenLearnerRegisterClassrun(
          List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerRegistrationNotifyApproverEvent(
                    registration.UserId,
                    new LearnerRegistrationNotifyApproverPayload
                    {
                        LearnerName = user.FullName(),
                        LearnerEmail = user.Email,
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetClassRegistrationLinkForCAMModule()
                    },
                    user.ApprovingOfficerIds));
        }
    }
}
