using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteMyOutstandingTaskLogic : BaseBusinessLogic<MyOutstandingTask>, IWriteMyOutstandingTaskLogic
    {
        private readonly IReadOnlyRepository<Form> _readFormRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<MyClassRun> _readMyClassRunRepository;
        private readonly IReadOnlyRepository<FormParticipant> _readFormParticipantRepository;
        private readonly IReadOnlyRepository<MyOutstandingTask> _readMyOutstandingTaskRepository;

        public WriteMyOutstandingTaskLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<Form> readFormRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<MyClassRun> readMyClassRunRepository,
            IReadOnlyRepository<FormParticipant> readFormParticipantRepository,
            IReadOnlyRepository<MyOutstandingTask> readMyOutstandingTaskRepository,
            IWriteOnlyRepository<MyOutstandingTask> writeMyOutstandingTaskRepository)
            : base(thunderCqrs, writeMyOutstandingTaskRepository)
        {
            _readFormRepository = readFormRepository;
            _readClassRunRepository = readClassRunRepository;
            _readMyClassRunRepository = readMyClassRunRepository;
            _readFormParticipantRepository = readFormParticipantRepository;
            _readMyOutstandingTaskRepository = readMyOutstandingTaskRepository;
        }

        public Task InsertMicroLearningTask(MyCourse myCourse)
        {
            return InsertOutstandingTask(new OutstandingTask(myCourse));
        }

        public Task InsertDigitalContentTask(MyDigitalContent myDigitalContent)
        {
            return InsertOutstandingTask(new OutstandingTask(myDigitalContent));
        }

        public Task DeleteCourseTask(MyClassRun myClassRun)
        {
            return DeleteOutstandingTask(new OutstandingTask(myClassRun));
        }

        public Task DeleteMicroLearningTask(MyCourse myCourse)
        {
            return DeleteOutstandingTask(new OutstandingTask(myCourse));
        }

        public Task DeleteDigitalContentTask(MyDigitalContent myDigitalContent)
        {
            return DeleteOutstandingTask(new OutstandingTask(myDigitalContent));
        }

        public Task DeleteAssignmentTask(MyAssignment myAssignment)
        {
            return DeleteOutstandingTask(new OutstandingTask(myAssignment));
        }

        public Task DeleteStandaloneFormTask(FormParticipant formParticipant)
        {
            return DeleteOutstandingTask(new OutstandingTask(formParticipant));
        }

        public async Task InsertCourseTask(MyClassRun myClassRun)
        {
            var endDate = await _readClassRunRepository
                .GetAll()
                .Where(p => p.Id == myClassRun.ClassRunId)
                .Select(p => p.EndDateTime)
                .FirstOrDefaultAsync();

            var courseTask = new OutstandingTask(myClassRun)
                .WithDueDate(endDate);

            await InsertOutstandingTask(courseTask);
        }

        public Task InsertAssignmentTask(MyAssignment myAssignment)
        {
            var assignmentTask = new OutstandingTask(myAssignment)
                .WithDueDate(myAssignment.EndDate);

            return InsertOutstandingTask(assignmentTask);
        }

        public async Task InsertManyStandaloneFormTask(Guid formOriginalObjectId)
        {
            var participants = await _readFormParticipantRepository
                .GetAll()
                .Where(p => p.FormOriginalObjectId == formOriginalObjectId)
                .Where(p => p.Status != FormParticipantStatus.Completed)
                .Select(p => new
                {
                    p.Id,
                    p.UserId
                })
                .ToListAsync();

            if (!participants.Any())
            {
                return;
            }

            var endDate = await _readFormRepository
                .GetAll()
                .Where(p => p.Status == FormStatus.Published)
                .Where(p => p.IsStandalone.Value)
                .Where(p => !p.IsArchived)
                .Where(p => p.OriginalObjectId == formOriginalObjectId)
                .Select(p => p.EndDate)
                .FirstOrDefaultAsync();

            var myOutstandingTasks = new List<MyOutstandingTask>();

            foreach (var participant in participants)
            {
                var myOutstandingTask = new MyOutstandingTask
                {
                    Id = Guid.NewGuid(),
                    UserId = participant.UserId,
                    ItemId = participant.Id,
                    ItemType = OutstandingTaskType.StandaloneForm,
                    Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.StandaloneForm)
                };

                myOutstandingTask.WithDueDate(endDate);

                myOutstandingTasks.Add(myOutstandingTask);
            }

            await WriteRepository.InsertManyAsync(myOutstandingTasks);
        }

        public async Task DeleteManyStandaloneFormTask(Guid formOriginalObjectId)
        {
            var participants = await _readFormParticipantRepository
                .GetAll()
                .Where(p => p.FormOriginalObjectId == formOriginalObjectId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!participants.Any())
            {
                return;
            }

            var myOutstandingTasks = await _readMyOutstandingTaskRepository
                .GetAll()
                .Where(p => p.ItemType == OutstandingTaskType.StandaloneForm)
                .Where(p => participants.Contains(p.ItemId))
                .ToListAsync();

            if (!myOutstandingTasks.Any())
            {
                return;
            }

            await WriteRepository.DeleteManyAsync(myOutstandingTasks);
        }

        public async Task DeleteManyCourseTask(ClassRun classRun)
        {
            var myOutstandingTasks = await GetByClassRunId(classRun.Id);

            if (!myOutstandingTasks.Any())
            {
                return;
            }

            await WriteRepository.DeleteManyAsync(myOutstandingTasks);
        }

        public async Task UpdateDueDateOfCourseTask(ClassRun classRun, DateTime? endDateTime)
        {
            var myOutstandingTasks = await GetByClassRunId(classRun.Id);

            if (!myOutstandingTasks.Any())
            {
                return;
            }

            myOutstandingTasks.ForEach(p =>
            {
                p.DueDate = endDateTime;
            });

            await WriteRepository.UpdateManyAsync(myOutstandingTasks);
        }

        private async Task<List<MyOutstandingTask>> GetByClassRunId(Guid classRunId)
        {
            var registrationIds = await _readMyClassRunRepository
                .GetAll()
                .Where(MyClassRun.FilterParticipantExpr())
                .Where(p => p.ClassRunId == classRunId)
                .Select(p => p.RegistrationId)
                .ToListAsync();

            if (!registrationIds.Any())
            {
                return new List<MyOutstandingTask>();
            }

            return await _readMyOutstandingTaskRepository
                .GetAll()
                .Where(p => registrationIds.Contains(p.ItemId))
                .Where(p => p.ItemType == OutstandingTaskType.Course)
                .ToListAsync();
        }

        private async Task InsertOutstandingTask(OutstandingTask outstandingTask)
        {
            var existingMyOutstandingTask = await _readMyOutstandingTaskRepository
                .GetAll()
                .Where(p => p.UserId == outstandingTask.UserId)
                .Where(p => p.ItemType == outstandingTask.ItemType)
                .Where(p => p.ItemId == outstandingTask.ItemId)
                .FirstOrDefaultAsync();

            if (existingMyOutstandingTask != null)
            {
                return;
            }

            var myOutstandingTask = new MyOutstandingTask
            {
                Id = Guid.NewGuid(),
                UserId = outstandingTask.UserId,
                ItemId = outstandingTask.ItemId,
                ItemType = outstandingTask.ItemType,
                Priority = outstandingTask.Priority
            };

            myOutstandingTask.WithDueDate(outstandingTask.DueDate);

            await WriteRepository.InsertAsync(myOutstandingTask);
        }

        private async Task DeleteOutstandingTask(OutstandingTask outstandingTask)
        {
            var existingMyOutstandingTask = await _readMyOutstandingTaskRepository
                .GetAll()
                .Where(p => p.UserId == outstandingTask.UserId)
                .Where(p => p.ItemType == outstandingTask.ItemType)
                .Where(p => p.ItemId == outstandingTask.ItemId)
                .FirstOrDefaultAsync();

            if (existingMyOutstandingTask == null)
            {
                return;
            }

            await WriteRepository.DeleteAsync(existingMyOutstandingTask);
        }

        protected class OutstandingTask
        {
            public OutstandingTask(MyCourse myCourse)
            {
                ItemId = myCourse.Id;
                UserId = myCourse.UserId;
                ItemType = OutstandingTaskType.Microlearning;
                Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.Microlearning);
            }

            public OutstandingTask(MyClassRun myClassRun)
            {
                ItemId = myClassRun.Id;
                UserId = myClassRun.UserId;
                ItemType = OutstandingTaskType.Course;
                Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.Course);
            }

            public OutstandingTask(MyAssignment myAssignment)
            {
                ItemId = myAssignment.Id;
                UserId = myAssignment.UserId;
                ItemType = OutstandingTaskType.Assignment;
                Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.Assignment);
            }

            public OutstandingTask(MyDigitalContent myDigitalContent)
            {
                ItemId = myDigitalContent.Id;
                UserId = myDigitalContent.UserId;
                ItemType = OutstandingTaskType.DigitalContent;
                Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.DigitalContent);
            }

            public OutstandingTask(FormParticipant formParticipant)
            {
                ItemId = formParticipant.Id;
                UserId = formParticipant.UserId;
                ItemType = OutstandingTaskType.StandaloneForm;
                Priority = TaskPriorityMapper.MapFromTaskType(OutstandingTaskType.StandaloneForm);
            }

            public Guid UserId { get; private set; }

            public Guid ItemId { get; private set; }

            public OutstandingTaskType ItemType { get; private set; }

            public int Priority { get; private set; }

            public DateTime? DueDate { get; private set; }

            public OutstandingTask WithDueDate(DateTime? dueDate)
            {
                if (dueDate != null)
                {
                    DueDate = dueDate;
                }

                return this;
            }
        }
    }
}
