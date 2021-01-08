using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ValidateECertificateTemplateLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public ValidateECertificateTemplateLogic(
           IReadOnlyRepository<CourseEntity> readCourseRepository,
           IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
        }

        public async Task<Validation> ValidateCanModifyAsync(ECertificateTemplate eCertificateTemplate, CancellationToken cancellationToken = default)
        {
            var courseIdsUsingTemplate = await _readCourseRepository
                .GetAll()
                .Where(o => o.ECertificateTemplateId == eCertificateTemplate.Id)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            if (courseIdsUsingTemplate.Any())
            {
                return Validation.Invalid("Unable to edit/delete this E-Certificate Template because there is at least one course using this E-Certificate template.");
            }

            return Validation.Valid();
        }
    }
}
