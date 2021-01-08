using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Business.Exceptions;
using Communication.Business.Models;
using Communication.DataAccess.Notification;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Communication.Business.Services.Template
{
    public class CommunicationTemplateService : ICommunicationTemplateService
    {
        private readonly INotificationTemplateRepository _notificationTemplateRepository;
        private readonly INotificationTemplateLanguageRepository _notificationTemplateLanguageRepository;

        public CommunicationTemplateService(INotificationTemplateRepository notificationTemplateRepository,
            INotificationTemplateLanguageRepository notificationTemplateLanguageRepository)
        {
            this._notificationTemplateRepository = notificationTemplateRepository;
            this._notificationTemplateLanguageRepository = notificationTemplateLanguageRepository;
        }
        public async Task DeleteTemplate(string templateId)
        {
            await _notificationTemplateRepository.DeleteById(GetDbObjectId(templateId));
            var filter = new FilterDefinitionBuilder<NotificationTemplateLanguage>().Eq(x => x.NotificationTemplateId, GetDbObjectId(templateId));
            await _notificationTemplateLanguageRepository.DeleteByFilter(filter);
        }

        public async Task DeleteTemplateLanguage(string templateId, string templateLanguageId)
        {
            var template = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(templateId));
            if (template == null)
                throw new BadRequestException("Template not found", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            await _notificationTemplateLanguageRepository.DeleteById(GetDbObjectId(templateLanguageId));
        }

        public async Task<List<NotificationTempateModel>> GetTemplates(string id, string languageCode, string tag, string name)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var dataById = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(id));
                var templateLanguages = await _notificationTemplateLanguageRepository.GetAllAsync(x => x.NotificationTemplateId == GetDbObjectId(id)
                && (x.LanguageCode == languageCode || string.IsNullOrEmpty(languageCode)));
                return dataById.Select(x => new NotificationTempateModel
                {
                    Id = x.Id.ToString(),
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDateUtc = DateTime.UtcNow,
                    Name = x.Name,
                    Tag = x.Tag,
                    Version = x.Version,
                    TemplateLanguages = templateLanguages.Any() ? templateLanguages.Select(y => new NotificationTemplateLanguageModel
                    {
                        Id = y.Id.ToString(),
                        LanguageCode = y.LanguageCode,
                        NotificationTemplateId = id,
                        TemplateContent = y.TemplateContent,
                        TemplateSubject = y.TemplateSubject
                    }).ToList() : null
                }).ToList();
            }

            var data = await _notificationTemplateRepository.GetAllAsync(x => (string.IsNullOrEmpty(tag) || x.Tag == tag)
            && (string.IsNullOrEmpty(name) || x.Name == name));
            var ids = data.Select(x => x.Id).Distinct();
            var templateDatas = await _notificationTemplateLanguageRepository.GetAllAsync(x => ids.Contains(x.NotificationTemplateId)
            && (x.LanguageCode == languageCode || string.IsNullOrEmpty(languageCode)));
            List<NotificationTempateModel> result = new List<NotificationTempateModel>();
            foreach (var item in data)
            {
                var templateLanguages = templateDatas.Where(x => x.NotificationTemplateId == item.Id).ToList();
                result.Add(new NotificationTempateModel
                {
                    Id = item.Id.ToString(),
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDateUtc = DateTime.UtcNow,
                    Name = item.Name,
                    Tag = item.Tag,
                    Version = item.Version,
                    TemplateLanguages = templateLanguages.Any() ? templateLanguages.Select(y => new NotificationTemplateLanguageModel
                    {
                        Id = y.Id.ToString(),
                        LanguageCode = y.LanguageCode,
                        NotificationTemplateId = item.Id.ToString(),
                        TemplateContent = y.TemplateContent,
                        TemplateSubject = y.TemplateSubject
                    }).ToList() : null
                });
            }
            return result;
        }

        public async Task<NotificationTempateModel> InsertTemplate(NotificationTempateModel notificationTempateModel)
        {
            var data = new NotificationTemplate
            {
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDateUtc = DateTime.UtcNow,
                Name = notificationTempateModel.Name,
                Tag = notificationTempateModel.Tag,
                Version = notificationTempateModel.Version
            };
            await _notificationTemplateRepository.InsertOneAsync(data);
            List<NotificationTemplateLanguage> listLangs = null;
            if (notificationTempateModel.TemplateLanguages != null && notificationTempateModel.TemplateLanguages.Any())
            {
                listLangs = notificationTempateModel.TemplateLanguages.Select(x => new NotificationTemplateLanguage
                {
                    LanguageCode = x.LanguageCode,
                    NotificationTemplateId = data.Id,
                    TemplateContent = x.TemplateContent,
                    TemplateSubject = x.TemplateSubject
                }).ToList();
                await _notificationTemplateLanguageRepository.InsertManyAsync(listLangs);
            }


            return new NotificationTempateModel
            {
                Id = data.Id.ToString(),
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDateUtc = DateTime.UtcNow,
                Name = notificationTempateModel.Name,
                Tag = notificationTempateModel.Tag,
                Version = notificationTempateModel.Version,
                TemplateLanguages = listLangs != null ? listLangs.Select(x => new NotificationTemplateLanguageModel
                {
                    LanguageCode = x.LanguageCode,
                    NotificationTemplateId = data.Id.ToString(),
                    TemplateContent = x.TemplateContent,
                    TemplateSubject = x.TemplateSubject
                }).ToList() : null
            };
        }

        private ObjectId GetDbObjectId(string id)
        {
            try
            {
                var templateDbId = new ObjectId(id);
                return templateDbId;
            }
            catch (Exception)
            {
                throw new BadRequestException($"Invalid id: {id}", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            }
        }
        public async Task<NotificationTemplateLanguageModel> InsertTemplateLanguage(string templateId, NotificationTemplateLanguageModel notificationTemplateLanguageModel)
        {

            var template = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(notificationTemplateLanguageModel.NotificationTemplateId));
            if (template == null)
                throw new BadRequestException("Template not found", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            var data = new NotificationTemplateLanguage
            {
                LanguageCode = notificationTemplateLanguageModel.LanguageCode,
                NotificationTemplateId = new MongoDB.Bson.ObjectId(notificationTemplateLanguageModel.NotificationTemplateId),
                TemplateContent = notificationTemplateLanguageModel.TemplateContent,
                TemplateSubject = notificationTemplateLanguageModel.TemplateSubject
            };
            await _notificationTemplateLanguageRepository.InsertOneAsync(data);
            return new NotificationTemplateLanguageModel
            {
                Id = data.Id.ToString(),
                LanguageCode = data.LanguageCode,
                NotificationTemplateId = data.NotificationTemplateId.ToString(),
                TemplateContent = data.TemplateContent,
                TemplateSubject = data.TemplateSubject
            };
        }


        public async Task<NotificationTempateModel> UpdateTemplate(string id, NotificationTempateModel notificationTempateModel)
        {
            var template = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(id));
            if (template == null)
                throw new BadRequestException("Template not found", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            await _notificationTemplateRepository.Update(GetDbObjectId(id),
                new UpdateDefinitionBuilder<NotificationTemplate>()
                .Set("Tag", notificationTempateModel.Tag)
                .Set("Name", notificationTempateModel.Name)
                .Set("Version", notificationTempateModel.Version)
                );

            var templateDatas = await _notificationTemplateLanguageRepository.GetAllAsync(x => x.NotificationTemplateId == GetDbObjectId(id));
            if (notificationTempateModel.TemplateLanguages != null)
            {
                foreach (var item in notificationTempateModel.TemplateLanguages)
                {
                    var languageData = templateDatas.FirstOrDefault(x => x.Id.ToString() == item.Id);
                    if (languageData != null)
                    {
                        await _notificationTemplateLanguageRepository.Update(GetDbObjectId(item.Id),
                                new UpdateDefinitionBuilder<NotificationTemplateLanguage>()
                                .Set("TemplateContent", item.TemplateContent)
                                .Set("TemplateSubject", item.TemplateSubject)
                                .Set("LanguageCode", item.LanguageCode));
                    }
                    else
                    {
                        var data = new NotificationTemplateLanguage
                        {
                            LanguageCode = item.LanguageCode,
                            NotificationTemplateId = GetDbObjectId(item.NotificationTemplateId),
                            TemplateContent = item.TemplateContent,
                            TemplateSubject = item.TemplateSubject
                        };
                        await _notificationTemplateLanguageRepository.InsertOneAsync(data);
                    }
                }
                foreach (var item in templateDatas)
                {
                    var languageData = notificationTempateModel.TemplateLanguages.FirstOrDefault(x => GetDbObjectId(x.Id) == item.Id);
                    if (languageData == null)
                    {
                        await _notificationTemplateLanguageRepository.DeleteById(item.Id);
                    }
                }
            }
            return notificationTempateModel;
        }

        public async Task<NotificationTemplateLanguageModel> UpdateTemplateLanguage(string id, string notificationTemplateId, NotificationTemplateLanguageModel notificationTemplateLanguageModel)
        {
            var template = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(notificationTemplateId));
            if (template == null)
                throw new BadRequestException("Template not found", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            var notificationTemplateLanguages = await _notificationTemplateRepository.GetAllAsync(x => x.Id == GetDbObjectId(id));
            if (notificationTemplateLanguages == null)
                throw new BadRequestException("Template language not found", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            await _notificationTemplateLanguageRepository.Update(GetDbObjectId(id),
                    new UpdateDefinitionBuilder<NotificationTemplateLanguage>()
                    .Set("TemplateContent", notificationTemplateLanguageModel.TemplateContent)
                    .Set("TemplateSubject", notificationTemplateLanguageModel.TemplateSubject)
                    .Set("LanguageCode", notificationTemplateLanguageModel.LanguageCode)
                );
            return notificationTemplateLanguageModel;
        }
    }
}
