using System;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class CommentApplicationService : BaseApplicationService
    {
        public CommentApplicationService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<CommentModel> CreateComment(CreateCommentRequest request, Guid userId)
        {
            var saveCommand = new SaveCommentCommand
            {
                CreationRequest = request,
                UserId = userId,
                Id = request.Id ?? Guid.NewGuid(),
                IsCreation = true
            };
            await this.ThunderCqrs.SendCommand(saveCommand);

            return await this.ThunderCqrs.SendQuery(new GetCommentByIdQuery { Id = saveCommand.Id });
        }

        public Task<PagedResultDto<CommentModel>> SearchComments(SearchCommentRequest dto)
        {
            var searchQuery = new SearchCommentQuery
            {
                Request = dto
            };

            return this.ThunderCqrs.SendQuery(searchQuery);
        }
    }
}