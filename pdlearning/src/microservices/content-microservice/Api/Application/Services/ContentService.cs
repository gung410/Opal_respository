using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Queries;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Entities;
using Microservice.Content.Versioning.Services;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Services
{
    public class ContentService : ApplicationService, IContentService
    {
        private readonly IVersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IAccessControlContext _accessControlContext;
        private readonly IUserContext _userContext;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IOutboxQueue _outboxQueue;
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ContentService(
            IThunderCqrs thunderCqrs,
            IOutboxQueue outboxQueue,
            IAccessControlContext accessControlContext,
            IRepository<DigitalContent> digitalContentRepository,
            IVersionTrackingApplicationService versionTrackingApplicationService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
            _outboxQueue = outboxQueue;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessControlContext = accessControlContext;
            _userContext = accessControlContext.UserContext;
            _digitalContentRepository = digitalContentRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<DigitalContentModel> CreateDigitalContent(CreateDigitalContentRequest request, Guid userId)
        {
            var saveCommand = new SaveDigitalContentCommand
            {
                CreationRequest = request,
                UserId = userId,
                DepartmentId = _accessControlContext.GetUserDepartment(),
                Id = request.Id ?? Guid.NewGuid(),
                IsCreation = true,
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = saveCommand.Id,
                UserId = userId,
                ActionComment = string.Format("Created \"{0}\"", request.Title),
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
            });

            return await this._thunderCqrs.SendQuery(new GetDigitalContentByIdQuery { Id = saveCommand.Id, UserId = userId });
        }

        public async Task<DigitalContentModel> UpdateDigitalContent(UpdateDigitalContentRequest request, Guid userId)
        {
            var saveCommand = new SaveDigitalContentCommand
            {
                UpdateRequest = request,
                UserId = userId,
                IsSubmitForApproval = request.IsSubmitForApproval
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            if (!request.IsAutoSave && request.Status != DigitalContentStatus.ReadyToUse)
            {
                await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
                {
                    VersionSchemaType = VersionSchemaType.DigitalContent,
                    ObjectId = saveCommand.UpdateRequest.Id,
                    UserId = userId,
                    ActionComment = "Edited",
                    RevertObjectId = Guid.Empty,
                    CanRollback = false,
                    IncreaseMajorVersion = false,
                    IncreaseMinorVersion = true,
                });
            }

            return await this._thunderCqrs.SendQuery(new GetDigitalContentByIdQuery { Id = saveCommand.UpdateRequest.Id, UserId = userId });
        }

        public Task<PagedResultDto<SearchDigitalContentModel>> SearchDigitalContent(SearchDigitalContentRequest dto, Guid userId)
        {
            var searchQuery = new SearchDigitalContentQuery
            {
                Request = dto,
                UserId = userId
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }

        public Task<PagedResultDto<SearchDigitalContentModel>> GetPendingApprovalDigitalContents(GetPendingApprovalDigitalContentsRequest dto)
        {
            var searchQuery = new GetPendingApprovalDigitalContentsQuery
            {
                PagedInfo = dto.PagedInfo
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }

        public Task<DigitalContentModel> GetDigitalContentById(Guid contentId, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetDigitalContentByIdQuery
            {
                Id = contentId,
                UserId = userId
            });
        }

        public Task<List<DigitalContentModel>> GetListDigitalContentsByListIds(List<Guid> listIds)
        {
            return this._thunderCqrs.SendQuery(new GetListDigitalContentsByListIdQuery { ListIds = listIds });
        }

        public async Task DeleteDigitalContent(Guid contentId, Guid userId)
        {
            var deleteFromCommand = new DeleteDigitalContentCommand
            {
                Id = contentId,
                UserId = userId
            };

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = contentId,
                UserId = userId,
                ActionComment = "Deleted",
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = false,
            });

            await _thunderCqrs.SendCommand(deleteFromCommand);
        }

        public async Task RenameDigitalContent(RenameDigitalContentRequest request, Guid userId)
        {
            var renameDigitalContentCommand = new RenameDigitalContentCommand
            {
                Request = request,
                UserId = userId
            };

            await _thunderCqrs.SendCommand(renameDigitalContentCommand);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = request.Id,
                UserId = userId,
                ActionComment = string.Format("Renamed to \"{0}\"", request.Title),
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
            });
        }

        public async Task<DigitalContentModel> CloneDigitalContent(Guid contentId, Guid userId)
        {
            var cloneCommand = new CloneDigitalContentCommand
            {
                Id = contentId,
                NewId = Guid.NewGuid(),
                UserId = userId
            };

            await _thunderCqrs.SendCommand(cloneCommand);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = cloneCommand.NewId,
                UserId = userId,
                ActionComment = "Cloned item",
                RevertObjectId = Guid.Empty,
                CanRollback = false,
            });

            return await _thunderCqrs.SendQuery(new GetDigitalContentByIdQuery { Id = cloneCommand.NewId, UserId = userId });
        }

        public async Task ChangeApprovalStatus(ChangeApprovalStatusRequest request, Guid userId)
        {
            string actionComment = string.Empty;
            Guid digitalContentId = request.Id,
                revertObjectId = Guid.Empty;

            bool allowRollback = false,
                increaseMajorVersion = false,
                increaseMinorVersion = true;

            if (request.Status == DigitalContentStatus.Unpublished)
            {
                var cloneDigitalContentAsNewVersionCmd = new CloneDigitalContentAsNewVersionCommand
                {
                    Id = request.Id,
                    NewId = Guid.NewGuid(),
                    UserId = userId,
                    ParentId = request.Id,
                    Status = DigitalContentStatus.Unpublished
                };
                await _thunderCqrs.SendCommand(cloneDigitalContentAsNewVersionCmd);
                MarkDigitalContentAsArchivedCommand markDigitalContentAsArchivedCommand = new MarkDigitalContentAsArchivedCommand
                {
                    Id = request.Id,
                    UserId = userId
                };
                await _thunderCqrs.SendCommand(markDigitalContentAsArchivedCommand);

                // Clone metadata
                await _outboxQueue.QueueMessageAsync(
                    new CloneMetadataBody
                    {
                        CloneFromResouceId = digitalContentId,
                        CloneToResouceId = cloneDigitalContentAsNewVersionCmd.NewId,
                        UserId = userId
                    },
                    _userContext);
            }
            else
            {
                var cmd = new ChangeApprovalStatusCommand { ContentId = request.Id, Status = request.Status, Comment = request.Comment, UserId = userId };
                await _thunderCqrs.SendCommand(cmd);
            }

            switch (request.Status)
            {
                case DigitalContentStatus.Published:
                    allowRollback = true;
                    increaseMajorVersion = true;
                    revertObjectId = request.Id;
                    actionComment = "Published";
                    break;
                case DigitalContentStatus.Unpublished:
                    actionComment = "Unpublished";
                    increaseMinorVersion = false;
                    break;
                case DigitalContentStatus.Approved:
                    actionComment = "Approved";
                    increaseMinorVersion = false;
                    break;
                case DigitalContentStatus.Rejected:
                    actionComment = "Rejected";
                    increaseMinorVersion = false;
                    break;
                case DigitalContentStatus.PendingForApproval:
                    actionComment = "Submited for approval";
                    increaseMinorVersion = false;
                    break;
                case DigitalContentStatus.ReadyToUse:
                    actionComment = "Readied for use";
                    increaseMinorVersion = false;
                    break;
                case DigitalContentStatus.Archived:
                    actionComment = "Archived";
                    break;
                default:
                    actionComment = string.Format("Status changed to {0}", request.Status);
                    break;
            }

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = request.Id,
                UserId = userId,
                ActionComment = actionComment,
                RevertObjectId = revertObjectId,
                CanRollback = allowRollback,
                IncreaseMajorVersion = increaseMajorVersion,
                IncreaseMinorVersion = increaseMinorVersion
            });
        }

        public Task<DigitalContentExpiryInfoModel[]> GetExpiryInfoOfDigitalContents(Guid[] contentIds)
        {
            return _thunderCqrs.SendQuery(new GetExpiryInfoOfDigitalContentsQuery { ListDigitalContentId = contentIds });
        }

        public async Task MigrateContentNotification(List<Guid> listIds)
        {
            await this._thunderCqrs.SendCommand(new MigrateContentNotificationCommand { ListIds = listIds });
        }

        public async Task TransferCourseOwnership(TransferOwnershipRequest request)
        {
            await this._thunderCqrs.SendCommand(new TransferOwnershipCommand { Request = request });
        }

        public Task<DigitalContentModel> GetDigitalContentByVersionTrackingId(GetContentByVersionTrackingIdRequestDto request, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetContentByVersionTrackingIdQuery
            {
                VersionTrackingId = request.VersionTrackingId,
                UserId = userId
            });
        }

        public async Task<PagedResultDto<Guid>> MigrateSearchContentData(MigrateSearchContentDataRequest request)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                var query = _digitalContentRepository
                            .GetAll()
                            .Where(content => !content.IsDeleted)
                            .WhereIf(request.ContentIds != null && request.ContentIds.Any(), p => request.ContentIds.Contains(p.Id));

                var totalCount = await query.CountAsync();
                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var existedContent = await query
                    .Skip(request.SkipCount)
                    .Take(request.MaxResultCount)
                    .ToListAsync();

                await _thunderCqrs.SendEvents(existedContent.Select(p => new MigrateSearchContentDataEvent(new DigitalContentModel(p))));
                await uow.CompleteAsync();

                return new PagedResultDto<Guid>(totalCount, existedContent.Select(p => p.Id).ToList());
            }
        }
    }
}
