using Communication.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Business.Services.Template
{
    public interface ICommunicationTemplateService
    {
        Task<NotificationTempateModel> InsertTemplate(NotificationTempateModel notificationTempateModel);
        Task<NotificationTemplateLanguageModel> InsertTemplateLanguage(string templateId, NotificationTemplateLanguageModel notificationTempateModel);
        Task<NotificationTempateModel> UpdateTemplate(string id, NotificationTempateModel notificationTempateModel);
        Task<NotificationTemplateLanguageModel> UpdateTemplateLanguage(string id, string templateLanguageId, NotificationTemplateLanguageModel notificationTemplateLanguageModel);
        Task DeleteTemplate(string templateId);
        Task DeleteTemplateLanguage(string templateId, string templateLanguageId);
        Task<List<NotificationTempateModel>> GetTemplates(string id, string languageCode, string tag, string name);
    }
}
