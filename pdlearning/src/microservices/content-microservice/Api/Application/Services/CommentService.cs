using System;
using System.Threading.Tasks;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Queries;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Services
{
    public class CommentService : ApplicationService, ICommentService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public CommentService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
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
            await this._thunderCqrs.SendCommand(saveCommand);

            return await this._thunderCqrs.SendQuery(new GetCommentByIdQuery { Id = saveCommand.Id });
        }

        public Task<PagedResultDto<CommentModel>> SearchComments(SearchCommentRequest dto)
        {
            var searchQuery = new SearchCommentQuery
            {
                Request = dto
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }
    }
}
