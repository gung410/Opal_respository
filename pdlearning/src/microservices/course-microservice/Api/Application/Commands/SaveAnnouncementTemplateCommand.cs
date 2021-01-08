using System;
using System.Text;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveAnnouncementTemplateCommand : BaseThunderCommand
    {
        public SaveAnnouncementTemplateCommand()
        {
        }

        public SaveAnnouncementTemplateCommand(SaveAnnouncementTemplateDto data)
        {
            Id = data.Id ?? Guid.NewGuid();
            IsCreate = !data.Id.HasValue;
            Title = data.Title;
            Message = Encoding.UTF8.GetString(Convert.FromBase64String(data.Message));
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsCreate { get; set; }
    }
}
