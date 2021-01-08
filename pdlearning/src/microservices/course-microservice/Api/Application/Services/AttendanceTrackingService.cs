using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class AttendanceTrackingService : BaseApplicationService
    {
        public AttendanceTrackingService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public async Task<PagedResultDto<AttendanceTrackingModel>> GetAttendanceTrackingBySessionId(GetAttendanceTrackingBySessionIdRequest request)
        {
            await ThunderCqrs.SendCommand(new InitAttendanceTrackingForSessionCommand { SessionId = request.SessionId });
            return await ThunderCqrs.SendQuery(new GetAttendanceTrackingBySessionIdQuery
            {
                Id = request.SessionId,
                PageInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                },
                SearchText = request.SearchText,
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

        public Task<AttendanceTrackingModel> GetAttendanceTrackingById(Guid id)
        {
            return ThunderCqrs.SendQuery(new GetAttendanceTrackingByIdQuery { Id = id });
        }

        public Task<List<AttendanceTrackingModel>> GetUserAttendanceTrackingByClassRunId(Guid classRunId)
        {
            return ThunderCqrs.SendQuery(new GetUserAttendanceTrackingsByClassRunIdQuery
            {
                ClassRunId = classRunId
            });
        }

        public Task<List<AttendanceRatioOfPresentInfo>> GetAttendanceRatioOfPresents(GetAttendanceRatioOfPresentsRequest request)
        {
            return ThunderCqrs.SendQuery(new GetAttendanceRatioOfPresentsQuery
            {
                ClassRunId = request.ClassRunId,
                RegistrationIds = request.RegistrationIds
            });
        }

        public Task ChangeReasonForAbsence(ChangeAttendanceTrackingReasonForAbsenceRequest request)
        {
            return ThunderCqrs.SendCommand(new ChangeAttendanceTrackingReasonForAbsenceCommand
            {
                SessionId = request.SessionId,
                Reason = request.Reason,
                Attachment = request.Attachment,
                UserId = request.UserId
            });
        }

        public Task ChangeStatus(ChangeAttendanceTrackingStatusRequest request)
        {
            return ThunderCqrs.SendCommand(new ChangeAttendancesStatusCommand
            {
                SessionId = request.SessionId,
                AttendanceTrackingIds = request.Ids,
                Status = request.Status
            });
        }

        public async Task<AttendanceTrackingModel> UpdateTakeAttendaceCodeScanned(LearnerTakeAttendanceRequest request)
        {
            await ThunderCqrs.SendCommand(new UpdateTakeAttendaceCodeScannedCommand
            {
                SessionId = request.SessionId,
                SessionCode = request.SessionCode
            });

            return await ThunderCqrs.SendQuery(new GetAttendanceTrackingByUserAndSessionCodeQuery { SessionCode = request.SessionCode });
        }
    }
}
