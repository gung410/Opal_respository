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
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchCommentQueryHandler : BaseQueryHandler<SearchCommentQuery, PagedResultDto<CommentModel>>
    {
        private readonly IReadOnlyRepository<Comment> _readCommentRepository;
        private readonly ApplyCommentViewPermissionSharedQuery _applyCommentViewPermissionSharedQuery;

        public SearchCommentQueryHandler(
            IReadOnlyRepository<Comment> readCommentRepository,
            ApplyCommentViewPermissionSharedQuery applyCommentViewPermissionSharedQuery,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCommentRepository = readCommentRepository;
            _applyCommentViewPermissionSharedQuery = applyCommentViewPermissionSharedQuery;
        }

        protected override async Task<PagedResultDto<CommentModel>> HandleAsync(SearchCommentQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readCommentRepository.GetAll()
                .Where(c => c.ObjectId == query.ObjectId);

            switch (query.EntityCommentType)
            {
                case EntityCommentType.Course:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.CoursePrefix}-{query.ActionType}%"));
                    break;
                case EntityCommentType.ClassRun:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.ClassRunPrefix}-{query.ActionType}%"));
                    break;
                case EntityCommentType.CourseContent:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.CourseContentPrefix}-{query.ActionType}%"));
                    break;
                case EntityCommentType.ClassRunContent:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.ClassRunContentPrefix}-{query.ActionType}%"));
                    break;
                case EntityCommentType.Registration:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.RegistrationPrefix}-{query.ActionType}%"));
                    break;
                case EntityCommentType.ParticipantAssignmentTrackQuizAnswer:
                    dbQuery = dbQuery.Where(c =>
                        EF.Functions.Like(c.Action, $"{CommentActionConstant.ParticipantAssignmentTrackQuizAnswerPrefix}%"));
                    break;
            }

            dbQuery = await _applyCommentViewPermissionSharedQuery.Execute(dbQuery, query.ActionType, CurrentUserIdOrDefault, CurrentUserRoles, cancellationToken);
            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PagedInfo);

            var entities = await dbQuery.Select(p => new CommentModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<CommentModel>(totalCount, entities);
        }
    }
}
