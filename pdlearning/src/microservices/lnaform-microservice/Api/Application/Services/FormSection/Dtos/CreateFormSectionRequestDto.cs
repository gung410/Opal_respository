using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.LnaForm.Application.Services
{
    public class CreateFormSectionRequestDto
    {
        public Guid? Id { get; set; }

        public Guid FormId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public SaveFormSectionsCommand BuildFormSectionCommand()
        {
            return new SaveFormSectionsCommand
            {
                Id = Id,
                FormId = FormId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted
            };
        }
    }
}
