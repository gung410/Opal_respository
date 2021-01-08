using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class PreviewAnnouncementTemplateQueryHandler : BaseQueryHandler<PreviewAnnouncementTemplateQuery, PreviewAnnouncementTemplateModel>
    {
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly IReadOnlyRepository<CourseUser> _readCourseUserRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly SendPlacementLetterLogic _sendPlacementLetterLogic;

        public PreviewAnnouncementTemplateQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            IReadOnlyRepository<CourseUser> readCourseUserRepository,
            WebAppLinkBuilder webAppLinkBuilder,
            SendPlacementLetterLogic sendPlacementLetterLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _readCourseUserRepository = readCourseUserRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _sendPlacementLetterLogic = sendPlacementLetterLogic;
        }

        protected override async Task<PreviewAnnouncementTemplateModel> HandleAsync(
            PreviewAnnouncementTemplateQuery query,
            CancellationToken cancellationToken)
        {
            switch (query.AnnouncementType)
            {
                case AnnouncementType.PlacementLetter:
                    var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(query.ClassRunId);

                    var currentUserInfo = await _readCourseUserRepository.GetAsync(CurrentUserIdOrDefault);

                    return new PreviewAnnouncementTemplateModel()
                    {
                       Message = query.GetReplacedTagsMessage(
                           aggregatedClassRun.Course.CourseName,
                           query.UserNameTag,
                           aggregatedClassRun.Course.CourseCode,
                           currentUserInfo.FullName(),
                           currentUserInfo.Email,
                           _sendPlacementLetterLogic.RenderSessionTable(aggregatedClassRun.Sessions),
                           _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(aggregatedClassRun.Course.Id))
                    };
                default:
                    return null;
            }
        }
    }
}
