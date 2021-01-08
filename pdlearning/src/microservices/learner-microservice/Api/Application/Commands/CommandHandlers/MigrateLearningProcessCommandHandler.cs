using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
#pragma warning disable SA1402 // File may only contain a single type
    public class MigrateLearningProcessCommandHandler : BaseCommandHandler<MigrateLearningProcessCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<MyClassRun> _myClassRunRepository;
        private readonly IRepository<MyCourse> _myCourseRepository;

        public MigrateLearningProcessCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MyClassRun> myClassRunRepository,
            IThunderCqrs thunderCqrs,
            IRepository<MyCourse> myCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _thunderCqrs = thunderCqrs;
            _myClassRunRepository = myClassRunRepository;
            _myCourseRepository = myCourseRepository;
        }

        protected override async Task HandleAsync(MigrateLearningProcessCommand command, CancellationToken cancellationToken)
        {
            var batchSize = command.BatchSize;
            for (int i = 0; i < command.CourseIds.Count; i += command.BatchSize)
            {
                var courseIds = command.CourseIds.Take(batchSize).Skip(i).ToList();
                var selectionMyCourses = await _myCourseRepository
                    .GetAll()
                    .Where(p => courseIds.Contains(p.CourseId) && p.CourseType != LearningCourseType.Microlearning)
                    .Where(p => p.Status == MyCourseStatus.Completed || p.Status == MyCourseStatus.InProgress)
                    .Select(p => new SelectionMyCourse(p))
                    .ToListAsync(cancellationToken);

                batchSize += command.BatchSize;
                var myCourseIds = selectionMyCourses.Select(p => p.CourseId).ToList();
                var selectionMyClassRunsByMyCourseIds = await GetSelectionMyClassRunsByMyCourseIds(myCourseIds, cancellationToken);
                foreach (var selectionMyCourse in selectionMyCourses)
                {
                    var selectionMyClassRun = selectionMyClassRunsByMyCourseIds.FirstOrDefault(p => p.CourseId == selectionMyCourse.CourseId && p.UserId == selectionMyCourse.UserId);
                    await SendLearningProcessEvent(selectionMyCourse, selectionMyClassRun, cancellationToken);
                }
            }
        }

        private async Task SendLearningProcessEvent(SelectionMyCourse selectionMyCourse, SelectionMyClassRun selectionMyClassRun, CancellationToken cancellationToken)
        {
            if (selectionMyClassRun == null)
            {
                return;
            }

            var lecturesChange = new LearningProcessModel
            {
                LearningStatus = selectionMyCourse.Status,
                ProgressMeasure = selectionMyCourse.ProgressMeasure,
                RegistrationId = selectionMyClassRun.RegistrationId
            };

            await _thunderCqrs.SendEvent(new LearningProcessChangeEvent(lecturesChange, LearningProcessType.Updated), cancellationToken);
        }

        private Task<List<SelectionMyClassRun>> GetSelectionMyClassRunsByMyCourseIds(List<Guid> myCourseIds, CancellationToken cancellationToken)
        {
            return _myClassRunRepository.GetAll().Where(
                p => myCourseIds.Contains(p.CourseId) &&
                     (p.Status == RegistrationStatus.ConfirmedByCA ||
                      p.Status == RegistrationStatus.OfferConfirmed))
                .Select(p => new SelectionMyClassRun(p))
                .ToListAsync(cancellationToken);
        }
    }

    public class SelectionMyCourse
    {
        public SelectionMyCourse(MyCourse myCourse)
        {
            Status = myCourse.Status;
            ProgressMeasure = myCourse.ProgressMeasure;
            CourseId = myCourse.CourseId;
            UserId = myCourse.UserId;
        }

        public MyCourseStatus Status { get; set; }

        public double? ProgressMeasure { get; set; }

        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }
    }

    public class SelectionMyClassRun
    {
        public SelectionMyClassRun(MyClassRun myClassRun)
        {
            CourseId = myClassRun.CourseId;
            UserId = myClassRun.UserId;
            RegistrationId = myClassRun.RegistrationId;
        }

        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }

        public Guid RegistrationId { get; set; }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
