using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.RequestDtos.AnnouncementRequest;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class AnnouncementService : BaseApplicationService
    {
        public AnnouncementService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<PagedResultDto<AnnouncementTemplateModel>> SearchAnnouncementTemplates(SearchAnnouncementTemplateRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchAnnouncementTemplatesQuery
            {
                SearchText = request.SearchText,
                PageInfo = new PagedResultRequestDto
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                }
            });
        }

        public async Task<AnnouncementTemplateModel> SaveAnnouncementTemplate(SaveAnnouncementTemplateRequest request)
        {
            var saveCommand = new SaveAnnouncementTemplateCommand(request.Data);

            await ThunderCqrs.SendCommand(saveCommand);
            return await ThunderCqrs.SendQuery(new GetAnnouncementTemplateByIdQuery { Id = saveCommand.Id });
        }

        public Task DeleteAnnouncementTemplate(Guid announcementTemplateId)
        {
            return ThunderCqrs.SendCommand(new DeleteAnnouncementTemplateCommand { AnnouncementTemplateId = announcementTemplateId });
        }

        public async Task SendAnnouncement(SendAnnouncementRequest request, Guid currentUserId)
        {
            var saveCommand = new SaveAnnouncementCommand(request.Data, currentUserId);

            await ThunderCqrs.SendCommand(saveCommand);

            if (request.Data.SaveTemplate)
            {
                await ThunderCqrs.SendCommand(new SaveAnnouncementTemplateCommand
                {
                    Title = saveCommand.Title,
                    Message = saveCommand.Message,
                    IsCreate = true
                });
            }
        }

        public Task<PagedResultDto<AnnouncementModel>> SearchAnnouncement(SearchAnnouncementRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchAnnouncementQuery
            {
                ClassRunId = request.ClassRunId,
                CourseId = request.CourseId,
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
                    : null,
                PagedInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                }
            });
        }

        public async Task ChangeAnnouncementStatus(ChangeAnnouncementStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeAnnouncementStatusCommand()
            {
                Status = request.Status,
                ForAnnouncements = new SendAnnouncementCommandSearchCondition()
                {
                    Ids = request.Ids
                }
            });
        }

        public Task<SendAnnouncementEmailTemplateModel> GetSendAnnouncementDefaultTemplate(GetSendAnnouncementDefaultTemplateRequest request)
        {
            return ThunderCqrs.SendQuery(request.ToQuery());
        }

        public Task SendCoursePublicity(SendCoursePublicityRequest request)
        {
            return ThunderCqrs.SendCommand(request.ToCommand());
        }

        public Task SendCourseAnnoucementNomination(SendCourseNominationAnnoucementRequest request)
        {
            return ThunderCqrs.SendCommand(request.ToCommand());
        }

        public Task SendPlacementLetter(SendPlacementLetterRequest request)
        {
            return ThunderCqrs.SendCommand(new SendPlacementLetterCommand(request));
        }

        public Task SendOrderRefreshment(SendOrderRefreshmentRequest request)
        {
            return ThunderCqrs.SendCommand(request.ToCommand());
        }

        public Task<PreviewAnnouncementTemplateModel> PreviewAnnouncementTemplate(PreviewAnnouncementTemplateRequest request)
        {
            return ThunderCqrs.SendQuery(request.ToQuery());
        }
    }
}
