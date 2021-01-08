using System;
using System.Collections.Generic;
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
    public class SessionService : BaseApplicationService
    {
        public SessionService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public async Task<SessionModel> SaveSession(SaveSessionRequest request)
        {
            var command = request.Data.ToCommand(request.UpdatePreRecordClipOnly);
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetSessionByIdQuery { Id = command.SessionData.Id });
        }

        public async Task<SessionModel> ChangeLearningMethod(ChangeLearningMethodRequest request)
        {
            var command = request.ToChangeLearningMethodCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetSessionByIdQuery { Id = command.Id });
        }

        public Task<PagedResultDto<SessionModel>> GetSessionsByClassRunId(GetSessionsBySessionIdRequest request)
        {
            return ThunderCqrs.SendQuery(new GetSessionsByClassRunIdQuery
            {
                ClassRunId = request.ClassRunId,
                SearchType = request.SearchType,
                PageInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                }
            });
        }

        public Task<SessionModel> GetSessionById(Guid id)
        {
            return ThunderCqrs.SendQuery(new GetSessionByIdQuery { Id = id });
        }

        public Task<IEnumerable<SessionModel>> GetSessionsByIds(IEnumerable<Guid> sessionIds)
        {
            return ThunderCqrs.SendQuery(new GetSessionsByIdsQuery { SessionIds = sessionIds });
        }

        public Task<IEnumerable<SessionModel>> GetSessionsByClassRunIds(IEnumerable<Guid> classRunIds)
        {
            return ThunderCqrs.SendQuery(new GetSessionsByClassRunIdsQuery { ClassRunIds = classRunIds });
        }

        public async Task<SessionModel> GetSessionCodeById(Guid sessionId)
        {
            var session = await ThunderCqrs.SendQuery(new GetSessionCodeByIdQuery { SessionId = sessionId });

            if (string.IsNullOrEmpty(session.SessionCode))
            {
                await ThunderCqrs.SendCommand(new CreateSessionCodeCommand { SessionId = sessionId });
                session = await ThunderCqrs.SendQuery(new GetSessionCodeByIdQuery { SessionId = sessionId });
            }

            return session;
        }

        public Task<bool> CheckExistedSessionField(CheckExistedSessionFieldRequest request)
        {
            return ThunderCqrs.SendQuery(new CheckExistedSessionFieldQuery
            {
                SessionDate = request.SessionDate,
                SessionId = request.SessionId,
                ClassRunId = request.ClassRunId
            });
        }

        public async Task DeleteSession(Guid id)
        {
            await this.ThunderCqrs.SendCommand(new DeleteSessionCommand { Id = id });
        }

        public Task<List<UpcomingSessionModel>> GetUpcomingSessionByClassRunIds(IEnumerable<Guid> classRunIds)
        {
            return ThunderCqrs.SendQuery(new GetUpcomingSessionByClassRunIdsQuery
            {
                ClassRunIds = classRunIds
            });
        }

        public Task<int> GetMaxMinutesCanJoinWebinarEarly()
        {
            return ThunderCqrs.SendQuery(new GetMaxMinutesCanJoinWebinarEarlyQuery());
        }
    }
}
