using System;
using Microservice.LnaForm.Domain.Entities;

namespace Microservice.LnaForm.Application.Models
{
    public class FormQuestionAnswerStatisticsModel
    {
        public FormQuestionAnswerStatisticsModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public int AnswerCode { get; set; }

        public string AnswerValue { get; set; }

        public double AnswerCount { get; set; }

        public double AnswerPercentage { get; set; }
    }
}
