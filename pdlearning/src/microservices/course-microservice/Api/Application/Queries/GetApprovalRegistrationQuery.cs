using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetApprovalRegistrationQuery : BaseThunderQuery<PagedResultDto<RegistrationModel>>
    {
        public ApprovalRegistrationFilterType FilterType { get; set; }

        public Guid? CourseId { get; set; }

        [CanBeNull]
        public List<Guid> ClassRunIds { get; set; }

        [CanBeNull]
        public List<Guid> LearnerIds { get; set; }

        [CanBeNull]
        public List<RegistrationStatus> RegistrationStatuses { get; set; }

        [CanBeNull]
        public List<WithdrawalStatus> WithdrawalStatuses { get; set; }

        [CanBeNull]
        public List<ClassRunChangeStatus> ClassRunChangeStatuses { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public DateTime? WithdrawalStartDate { get; set; }

        public DateTime? WithdrawalEndDate { get; set; }

        public DateTime? ClassRunChangeRequestedStartDate { get; set; }

        public DateTime? ClassRunChangeRequestedEndDate { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
