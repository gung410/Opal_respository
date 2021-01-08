using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeRegistrationWithdrawStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public WithdrawalStatus WithdrawalStatus { get; set; }

        public string Comment { get; set; }

        public bool IsManual { get; set; }
    }
}
