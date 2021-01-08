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
    public class VideoCommentService : ApplicationService, IVideoCommentService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public VideoCommentService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public async Task<VideoCommentModel> CreateVideoComment(CreateVideoCommentRequest request, Guid userId)
        {
            var saveCommand = new SaveVideoCommentCommand
            {
                CreationRequest = request,
                UserId = userId,
                Id = Guid.Equals(request.Id, Guid.Empty) ? Guid.NewGuid() : request.Id.Value,
                IsCreation = true
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            return await this._thunderCqrs.SendQuery(new GetVideoCommentByIdQuery { Id = saveCommand.Id });
        }

        public async Task<VideoCommentModel> UpdateVideoComment(UpdateVideoCommentRequest request, Guid userId)
        {
            var saveCommand = new SaveVideoCommentCommand
            {
                UpdateRequest = request,
                UserId = userId,
                Id = request.Id,
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            return await this._thunderCqrs.SendQuery(new GetVideoCommentByIdQuery { Id = saveCommand.Id });
        }

        public async Task DeleteVideoComment(Guid id, Guid userId)
        {
            var deleteFromCommand = new DeleteVideoCommentCommand
            {
                Id = id,
                UserId = userId
            };

            await _thunderCqrs.SendCommand(deleteFromCommand);
        }

        public Task<PagedResultDto<VideoCommentModel>> SearchVideoComments(SearchVideoCommentRequest dto, Guid userId)
        {
            var searchQuery = new SearchVideoCommentQuery
            {
                Request = dto
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }
    }
}
