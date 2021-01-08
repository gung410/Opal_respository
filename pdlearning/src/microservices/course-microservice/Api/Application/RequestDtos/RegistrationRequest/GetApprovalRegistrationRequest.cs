using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetApprovalRegistrationRequest : PagedResultRequestDto
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
    }
}
