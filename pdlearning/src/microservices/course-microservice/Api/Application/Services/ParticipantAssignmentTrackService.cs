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
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class ParticipantAssignmentTrackService : BaseApplicationService
    {
        public ParticipantAssignmentTrackService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<ParticipantAssignmentTrackModel> GetParticipantAssignmentTrackById(Guid id)
        {
            return ThunderCqrs.SendQuery(new GetParticipantAssignmentTrackByIdQuery
            {
                Id = id
            });
        }

        public Task<List<ParticipantAssignmentTrackModel>> GetParticipantAssignmentTrackByIds(List<Guid> ids)
        {
            return ThunderCqrs.SendQuery(new GetParticipantAssignmentTrackByIdsQuery
            {
                Ids = ids
            });
        }

        public Task<PagedResultDto<ParticipantAssignmentTrackModel>> GetParticipantAssignmentTracks(GetParticipantAssignmentTracksRequest request)
        {
            return ThunderCqrs.SendQuery(new GetParticipantAssignmentTracksQuery
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId,
                AssignmentId = request.AssignmentId,
                ForCurrentUser = request.ForCurrentUser,
                IncludeQuizAssignmentFormAnswer = request.IncludeQuizAssignmentFormAnswer,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
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

        public async Task<IEnumerable<ParticipantAssignmentTrackModel>> AssignAssignment(AssignAssignmentRequest request)
        {
            await ThunderCqrs.SendCommand(new AssignAssignmentCommand
            {
                Registrations = request.Registrations?.Select(p => p.ToAssignAssignmentCommandRegistrationCommand()).ToList(),
                AssignmentId = request.AssignmentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            });

            var participantAssignmentTracks = (await ThunderCqrs.SendQuery(new GetParticipantAssignmentTracksQuery()
            {
                AssignmentId = request.AssignmentId,
                RegistrationIds = request.Registrations?.Select(x => x.RegistrationId)
            })).Items;

            await ThunderCqrs.SendCommand(new AssignAssessmentCommand
            {
                ParticipantAssignmentTrackIds = participantAssignmentTracks.Select(p => p.Id).ToList(),
                AssignmentId = request.AssignmentId
            });

            return participantAssignmentTracks;
        }

        public async Task<ParticipantAssignmentTrackModel> SaveAssignmentQuizAnswer(SaveAssignmentQuizAnswerRequest request)
        {
            await ThunderCqrs.SendCommand(request.ToCommand());

            return await ThunderCqrs.SendQuery(new GetSingleParticipantAssignmentTrackQuery
            {
                AssignmentId = request.AssignmentId,
                RegistrationId = request.RegistrationId
            });
        }

        public async Task<ParticipantAssignmentTrackModel> MarkScoreForQuizQuestionAnswer(MarkScoreForQuizQuestionAnswerRequest request)
        {
            await ThunderCqrs.SendCommand(new MarkScoreForQuizQuestionAnswerCommand
            {
                ParticipantAssignmentTrackId = request.ParticipantAssignmentTrackId,
                QuestionAnswerScores = request.MarkScoreForQuizQuestionAnswers.SelectList(p => p.ToCommand())
            });

            return await ThunderCqrs.SendQuery(new GetParticipantAssignmentTrackByIdQuery
            {
                Id = request.ParticipantAssignmentTrackId
            });
        }
    }
}
