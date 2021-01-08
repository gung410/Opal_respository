using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessCoursePdAreaThemeCodeLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<MetadataTag> _readMetaDataTagRepository;

        public ProcessCoursePdAreaThemeCodeLogic(
            IReadOnlyRepository<MetadataTag> readMetaDataTagRepository,
            IUserContext userContext) : base(userContext)
        {
            _readMetaDataTagRepository = readMetaDataTagRepository;
        }

        public async Task ForSingleCourse(CourseEntity courseEntity, CancellationToken cancellationToken)
        {
            await ForCourses(new List<CourseEntity>() { courseEntity }, cancellationToken);
        }

        public async Task ForCourses(List<CourseEntity> courses, CancellationToken cancellationToken)
        {
            var validCourses = courses.Where(p => !string.IsNullOrEmpty(p.PDAreaThemeId)).ToList();
            if (!validCourses.Any())
            {
                return;
            }

            var coursesPdAreaThemeIds = validCourses.Select(p => p.PDAreaThemeId).ToList();
            var coursesPdAreaThemeCodeDic = await GetCodingSchemaByPdAreaThemeIds(coursesPdAreaThemeIds, cancellationToken);
            validCourses.ForEach(p =>
            {
                p.PDAreaThemeCode = coursesPdAreaThemeCodeDic.GetValueOrDefault(new Guid(p.PDAreaThemeId));

                EnsureBusinessLogicValid(Validation.ValidIf(
                    p.PDAreaThemeCode != null,
                    "The selected PDAreaTheme is missing CodingScheme. Please select other PDAreaTheme."));
            });
        }

        private async Task<Dictionary<Guid, string>> GetCodingSchemaByPdAreaThemeIds(List<string> pdAreaThemeIds, CancellationToken cancellationToken)
        {
            var metdataIds = pdAreaThemeIds.Select(p => new Guid(p));
            return await _readMetaDataTagRepository.GetAll()
                .Where(p => metdataIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.CodingScheme, cancellationToken);
        }
    }
}
