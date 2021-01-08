using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetCommentNotSeenQueryHandler : BaseQueryHandler<GetCommentNotSeenQuery, IEnumerable<SeenCommentModel>>
    {
        private readonly IReadOnlyRepository<Comment> _readCommentRepository;
        private readonly IReadOnlyRepository<CommentTrack> _readCommentTrackRepository;
        private readonly ApplyCommentViewPermissionSharedQuery _applyCommentViewPermissionSharedQuery;

        public GetCommentNotSeenQueryHandler(
            IReadOnlyRepository<Comment> readCommentRepository,
            IReadOnlyRepository<CommentTrack> readCommentTrackRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            ApplyCommentViewPermissionSharedQuery applyCommentViewPermissionSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCommentRepository = readCommentRepository;
            _readCommentTrackRepository = readCommentTrackRepository;
            _applyCommentViewPermissionSharedQuery = applyCommentViewPermissionSharedQuery;
        }

        protected override async Task<IEnumerable<SeenCommentModel>> HandleAsync(GetCommentNotSeenQuery query, CancellationToken cancellationToken)
        {
            var commentDbQuery = _readCommentRepository.GetAll().Where(x => query.ObjectIds.Contains(x.ObjectId));

            switch (query.EntityCommentType)
            {
                case EntityCommentType.Course:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.CoursePrefix}%"));
                    break;
                case EntityCommentType.ClassRun:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.ClassRunPrefix}%"));
                    break;
                case EntityCommentType.CourseContent:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.CourseContentPrefix}%"));
                    break;
                case EntityCommentType.ClassRunContent:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.ClassRunContentPrefix}%"));
                    break;
                case EntityCommentType.Registration:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.RegistrationPrefix}%"));
                    break;
                case EntityCommentType.ParticipantAssignmentTrackQuizAnswer:
                    commentDbQuery = commentDbQuery.Where(c => EF.Functions.Like(c.Action, $"{CommentActionConstant.ParticipantAssignmentTrackQuizAnswerPrefix}%"));
                    break;
            }

            commentDbQuery =
               await _applyCommentViewPermissionSharedQuery.Execute(commentDbQuery, string.Empty, CurrentUserIdOrDefault, CurrentUserRoles, cancellationToken);

            var commentTrackDbQuery = _readCommentTrackRepository.GetAll()
                .Where(x => x.UserId == CurrentUserId);

            var unseenCommentQuery = from c in commentDbQuery
                          join ct in commentTrackDbQuery on c.Id equals ct.CommentId into ps
                          from p in ps.DefaultIfEmpty()
                          where p == null
                          select c;

            var unseenComment = await unseenCommentQuery.ToListAsync(cancellationToken);

            var unseenCommentDict = unseenComment.GroupBy(x => x.ObjectId).ToDictionary(x => x.Key);

            return query.ObjectIds.Select(x =>
            {
                if (unseenCommentDict.ContainsKey(x))
                {
                    return new SeenCommentModel
                    {
                        ObjectId = x,
                        CommentNotSeenIds = unseenCommentDict[x].Select(p => p.Id)
                    };
                }

                return new SeenCommentModel
                {
                    ObjectId = x,
                    CommentNotSeenIds = new List<Guid>()
                };
            });
        }
    }
}
