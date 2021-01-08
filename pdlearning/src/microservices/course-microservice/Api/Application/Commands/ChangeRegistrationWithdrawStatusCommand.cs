using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeRegistrationWithdrawStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public WithdrawalStatus WithdrawalStatus { get; set; }

        public bool IsManual { get; set; }
    }
}
