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
    public class CommentService : BaseApplicationService
    {
        public CommentService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public async Task<CommentModel> CreateComment(CreateCommentRequest request)
        {
            var saveCommand = new SaveCommentCommand
            {
                EntityCommentType = request.EntityCommentType,
                Id = Guid.NewGuid(),
                Content = request.Content,
                ObjectId = request.ObjectId,
                IsCreate = true,
                Notification = request.CommentNotification
            };
            await ThunderCqrs.SendCommand(saveCommand);

            return await ThunderCqrs.SendQuery(new GetCommentByIdQuery { Id = saveCommand.Id.Value });
        }

        public async Task<PagedResultDto<CommentModel>> SearchComments(SearchCommentRequest request)
        {
            var searchQuery = new SearchCommentQuery
            {
                ObjectId = request.ObjectId,
                EntityCommentType = request.EntityCommentType,
                ActionType = request.ActionType,
                PagedInfo = request.PagedInfo
            };

            var result = await ThunderCqrs.SendQuery(searchQuery);

            var saveCommentTrackCommand = new SaveCommentTrackCommand
            {
                CommentIds = result.Items.Select(x => x.Id)
            };

            await ThunderCqrs.SendCommand(saveCommentTrackCommand);

            return result;
        }

        public Task<IEnumerable<SeenCommentModel>> GetCommentNotSeen(GetCommentNotSeenRequest request)
        {
            return ThunderCqrs.SendQuery(
                new GetCommentNotSeenQuery
                {
                    ObjectIds = request.ObjectIds,
                    EntityCommentType = request.EntityCommentType
                });
        }
    }
}
