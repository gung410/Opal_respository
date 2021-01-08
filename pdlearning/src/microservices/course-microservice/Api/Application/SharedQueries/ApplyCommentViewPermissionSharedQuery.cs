using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class ApplyCommentViewPermissionSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CommentViewPermission> _readCommentViewPermissionRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;

        public ApplyCommentViewPermissionSharedQuery(
            IReadOnlyRepository<CommentViewPermission> readCommentViewPermissionRepository,
            IReadOnlyRepository<CourseUser> readUserRepository)
        {
            _readCommentViewPermissionRepository = readCommentViewPermissionRepository;
            _readUserRepository = readUserRepository;
        }

        public async Task<IQueryable<Comment>> Execute(
            IQueryable<Comment> commentQuery,
            string actionType,
            Guid currentUser,
            List<string> currentUserRoles,
            CancellationToken cancellationToken)
        {
            var restrictedCommentByUserRoles = await _readCommentViewPermissionRepository
                .GetAll()
                .Where(x => currentUserRoles.Contains(x.CanViewRole))
                .WhereIf(!string.IsNullOrWhiteSpace(actionType), x => x.CommentAction == actionType)
                .Select(x => x.CommentByUserRole)
                .Distinct()
                .ToListAsync(cancellationToken);

            return restrictedCommentByUserRoles.Any()
                ? commentQuery
                    .Join(
                        _readUserRepository.GetAll(),
                        p => p.UserId,
                        x => x.Id,
                        (comment, user) => new { comment, user.UserSystemRoles })
                    .Where(x => x.comment.UserId == currentUser || x.UserSystemRoles.Select(p => p.Value).Any(a => restrictedCommentByUserRoles.Contains(a)))
                    .Select(x => x.comment)
                : commentQuery;
        }
    }
}
