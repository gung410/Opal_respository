using System;
using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.Models
{
    public class ValidateNominateLearnerResultModel
    {
        public Guid UserId { get; set; }

        public ValidateLearnerResultType ValidateResultCode { get; set; }
    }
}
