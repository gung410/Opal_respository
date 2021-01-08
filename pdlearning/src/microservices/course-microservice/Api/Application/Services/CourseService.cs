using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class CourseService : BaseApplicationService
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public CourseService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWork,
            IReadOnlyRepository<CourseEntity> readCourseRepository) : base(thunderCqrs, unitOfWork)
        {
            _readCourseRepository = readCourseRepository;
        }

        public Task<PagedResultDto<CourseModel>> SearchCourses(SearchCoursesRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchCoursesQuery
            {
                SearchText = request.SearchText,
                SearchType = request.SearchType,
                CheckCourseContent = request.CheckCourseContent,
                PageInfo = new PagedResultRequestDto
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                },
                CoursePlanningCycleId = request.CoursePlanningCycleId,
                Filter = request.Filter != null
                    ? new CommonFilter
                    {
                        ContainFilters = request.Filter.ContainFilters?
                            .Select(p => new ContainFilter { Field = p.Field, Values = p.Values, NotContain = p.NotContain })
                            .ToList(),
                        FromToFilters = request.Filter.FromToFilters?
                            .Select(p => new FromToFilter
                            {
                                Field = p.Field,
                                FromValue = p.FromValue,
                                ToValue = p.ToValue,
                                EqualFrom = p.EqualFrom,
                                EqualTo = p.EqualTo
                            })
                            .ToList()
                    }
                    : null
            });
        }

        public Task<PagedResultDto<UserModel>> SearchCourseUsers(SearchCourseUsersRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchUsersQuery
            {
                SearchText = request.SearchText,
                CanApplyForCourse = request.ForCourse != null ? new Queries.SearchUsersQueryForCourseInfo
                {
                    CourseId = request.ForCourse.CourseId,
                    FollowCourseTargetParticipant = request.ForCourse.FollowCourseTargetParticipant
                }
                : null,
                PageInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                }
            });
        }

        public async Task<CourseModel> SaveCourse(SaveCourseRequest request)
        {
            var saveCommand = request.Data.ToCommand();

            await ThunderCqrs.SendCommand(saveCommand);

            await ThunderCqrs.SendCommand(new ChangeCourseStatusCommand
            {
                Status = saveCommand.Status,
                Ids = new List<Guid> { saveCommand.Id },
                Comment = request.Data.CourseApprovalComment
            });

            if (!string.IsNullOrWhiteSpace(request.Data.CourseApprovalComment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Course,
                    StatusEnum = saveCommand.Status,
                    Content = request.Data.CourseApprovalComment,
                    ObjectId = saveCommand.Id,
                    IsCreate = true
                });
            }

            return await ThunderCqrs.SendQuery(new GetCourseByIdQuery { Id = saveCommand.Id });
        }

        public Task<CourseModel> GetCourseDetailById(Guid courseId)
        {
            return ThunderCqrs.SendQuery(new GetCourseByIdQuery { Id = courseId });
        }

        public Task<IEnumerable<CourseModel>> GetCoursesByCourseCodes(GetCoursesByCourseCodesRequest request)
        {
            return ThunderCqrs.SendQuery(new GetCoursesByCourseCodesQuery { CourseCodes = request.CourseCodes });
        }

        public Task<List<CourseModel>> GetListCoursesByListIds(List<Guid> listIds)
        {
            return ThunderCqrs.SendQuery(new GetListCoursesByListIdQuery { ListIds = listIds });
        }

        public Task DeleteCourse(Guid courseId)
        {
            return ThunderCqrs.SendCommand(new DeleteCourseCommand { CourseId = courseId });
        }

        public async Task<CourseModel> CloneCourse(CloneCourseRequest request)
        {
            Guid newGuid = Guid.NewGuid();
            await ThunderCqrs.SendCommand(new CloneCourseCommand { Id = request.Id, NewId = newGuid, FromCoursePlanning = request.FromCoursePlanning });
            return await ThunderCqrs.SendQuery(new GetCourseByIdQuery { Id = newGuid });
        }

        public async Task ChangeCourseStatus(ChangeCourseStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeCourseStatusCommand
            {
                Status = request.Status,
                Ids = request.Ids,
                Comment = request.Comment
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Course,
                    StatusEnum = request.Status,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task<PagedResultDto<Guid>> MigrateCourseNotification(MigrateCourseNotificationRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var query = _readCourseRepository
                    .GetAll()
                    .WhereIf(request.CourseIds != null && request.CourseIds.Any(), p => request.CourseIds.Contains(p.Id));

                var totalCount = await query.CountAsync();
                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var results = await query.Skip(request.SkipCount).Take(request.MaxResultCount).ToListAsync();

                await ThunderCqrs.SendEvents(results.Select(p => new CourseChangeEvent(p, CourseChangeType.Updated, true)));
                return new PagedResultDto<Guid>(totalCount, results.Select(p => p.Id).ToList());
            });
        }

        public Task<bool> CheckExistedCourseField(CheckExistedCourseFieldRequest request)
        {
            return ThunderCqrs.SendQuery(new CheckExistedCourseFieldQuery
            {
                CourseCode = request.CourseCode,
                ExternalCode = request.ExternalCode,
                CourseId = request.CourseId
            });
        }

        public Task<bool> CheckCourseEndDateValidWithClassEndDate(CheckCourseEndDateValidWithClassEndDateRequest request)
        {
            return ThunderCqrs.SendQuery(new CheckCourseEndDateValidWithClassEndDateQuery
            {
                CourseId = request.CourseId,
                EndDate = request.EndDate
            });
        }

        public Task TransferCourseOwnership(TransferCourseOwnershipRequest request)
        {
            return ThunderCqrs.SendCommand(new TransferCourseOwnershipCommand
            {
                CourseId = request.CourseId,
                NewOwnerId = request.NewOwnerId
            });
        }

        public async Task ArchiveCourse(ArchiveCourseRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeCourseStatusCommand
            {
                Status = CourseStatus.Archived,
                Ids = request.Ids,
                Comment = string.Empty
            });
        }
    }
}
