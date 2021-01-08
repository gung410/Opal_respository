using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Infrastructure;
using Conexus.Opal.Microservice.Metadata.Constants;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Metadata.Dtos;
using Conexus.Opal.Microservice.Metadata.Entities;
using Microsoft.Extensions.Configuration;

namespace Conexus.Opal.Microservice.Metadata.DataSync
{
    public class MetadataSynchronizer : IMetadataSynchronizer
    {
        private readonly IMetadataDataProvider _metadataDataProvider;
        private readonly HttpService _httpService;
        private readonly IAuthenticationTokenService _authenticationTokenService;
        private readonly string _apiOriginUrl;

        public MetadataSynchronizer(
            IMetadataDataProvider metadataDataProvider,
            HttpService httpService,
            IAuthenticationTokenService authenticationTokenService,
            IConfiguration configuration)
        {
            _metadataDataProvider = metadataDataProvider;
            _httpService = httpService;
            _authenticationTokenService = authenticationTokenService;
            _apiOriginUrl = configuration[ConfigurationKeys.MetadataSynchronizerApiOriginUrl];
        }

        public async Task Sync<TMetadataTag>(bool force = false) where TMetadataTag : IMetadataTag
        {
            if (force == false && await _metadataDataProvider.HasAnyMetadataTags())
            {
                return;
            }

            var allMetadataTags = new List<MetadataTagSyncDto>();

            var serviceSchemes = await GetMetadataTags(MetadataTagGroupCodes.ServiceSchemes);
            SaveMetadataTags(serviceSchemes, MetadataTagGroupCodes.ServiceSchemes);
            allMetadataTags.AddRange(serviceSchemes.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var courseLevels = await GetMetadataTags(MetadataTagGroupCodes.CourseLevels);
            SaveMetadataTags(courseLevels, MetadataTagGroupCodes.CourseLevels);
            allMetadataTags.AddRange(courseLevels.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var pdoCategories = await GetMetadataTags(MetadataTagGroupCodes.PdoCategories);
            SaveMetadataTags(pdoCategories, MetadataTagGroupCodes.PdoCategories);
            allMetadataTags.AddRange(pdoCategories.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var pdoModes = await GetMetadataTags(MetadataTagGroupCodes.PdoModes);
            SaveMetadataTags(pdoModes, MetadataTagGroupCodes.PdoModes);
            allMetadataTags.AddRange(pdoModes.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var pdoTypes = await GetMetadataTags(MetadataTagGroupCodes.PdoTypes);
            SaveMetadataTags(pdoTypes, MetadataTagGroupCodes.PdoTypes);
            allMetadataTags.AddRange(pdoTypes.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var teachingSubjects = await GetMetadataTags(MetadataTagGroupCodes.TeachingSubjects);
            SaveMetadataTags(teachingSubjects, MetadataTagGroupCodes.TeachingSubjects);
            allMetadataTags.AddRange(teachingSubjects.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var coCurricularActivities = await GetMetadataTags(MetadataTagGroupCodes.CoCurricularActivities);
            SaveMetadataTags(coCurricularActivities, MetadataTagGroupCodes.CoCurricularActivities);
            allMetadataTags.AddRange(coCurricularActivities.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var pdoNatures = await GetMetadataTags(MetadataTagGroupCodes.PdoNatures);
            SaveMetadataTags(pdoNatures, MetadataTagGroupCodes.PdoNatures);
            allMetadataTags.AddRange(pdoNatures.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            var periodOfPdActivity = await GetMetadataTags(MetadataTagGroupCodes.PeriodOfPdActivity);
            SaveMetadataTags(periodOfPdActivity, MetadataTagGroupCodes.PeriodOfPdActivity);
            allMetadataTags.AddRange(periodOfPdActivity.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

            foreach (var serviceScheme in serviceSchemes)
            {
                var tracks = await GetMetadataTags(MetadataTagGroupCodes.Tracks, new List<string> { serviceScheme.Code });
                SaveMetadataTags(tracks, MetadataTagGroupCodes.Tracks, serviceScheme.Id);
                allMetadataTags.AddRange(tracks.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var coursesOfStudy = await GetMetadataTags(MetadataTagGroupCodes.CoursesOfStudy, new List<string> { serviceScheme.Code });
                SaveMetadataTags(coursesOfStudy, MetadataTagGroupCodes.CoursesOfStudy, serviceScheme.Id);
                allMetadataTags.AddRange(coursesOfStudy.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var teachingLevels = await GetMetadataTags(MetadataTagGroupCodes.TeachingLevels, new List<string> { serviceScheme.Code });
                SaveMetadataTags(teachingLevels, MetadataTagGroupCodes.TeachingLevels, serviceScheme.Id);
                allMetadataTags.AddRange(teachingLevels.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var jobFamilies = await GetMetadataTags(MetadataTagGroupCodes.JobFamilies, new List<string> { serviceScheme.Code });
                SaveMetadataTags(jobFamilies, MetadataTagGroupCodes.JobFamilies, serviceScheme.Id);
                allMetadataTags.AddRange(jobFamilies.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var developmentalRoles = await GetMetadataTags(MetadataTagGroupCodes.DevRoles, new List<string> { serviceScheme.Code });
                SaveMetadataTags(developmentalRoles, MetadataTagGroupCodes.DevRoles, serviceScheme.Id);
                allMetadataTags.AddRange(developmentalRoles.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var subjectAreasAndKeyWords = await GetMetadataTags(MetadataTagGroupCodes.PdoTaxonomy, new List<string> { serviceScheme.Code }, null, 10);
                SaveMetadataTags(subjectAreasAndKeyWords, MetadataTagGroupCodes.PdoTaxonomy, serviceScheme.Id);
                allMetadataTags.AddRange(subjectAreasAndKeyWords.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));

                var learningFrameworks = await GetMetadataTags(MetadataTagGroupCodes.LearningFxs, new List<string> { serviceScheme.Code }, new List<string> { "Learning Framework", "Learning Area", "Learning Dimension", "Learning Area Private", "Learning Sub Area", "Teacher Outcome" }, 10);
                SaveMetadataTags(learningFrameworks, MetadataTagGroupCodes.LearningFxs, serviceScheme.Id);
                allMetadataTags.AddRange(learningFrameworks.SelectMany(p => MetadataTagSyncDto.FlatTree(p)));
            }

            await _metadataDataProvider.DeleteNotExistedMetadataInListIds<TMetadataTag>(allMetadataTags.Select(p => p.Id));
        }

        public Task Sync(bool force = false)
        {
            return Sync<MetadataTag>(force);
        }

        private void SaveMetadataTags(IEnumerable<MetadataTagSyncDto> tags, string groupCode = null, Guid? parentTagId = null)
        {
            foreach (var item in tags)
            {
                _metadataDataProvider.SaveMetadataTag(item.ToEntity(groupCode, parentTagId));
                if (item.Childs != null && item.Childs.Any())
                {
                    SaveMetadataTags(item.Childs, null, item.Id);
                }
            }
        }

        private async Task<List<MetadataTagSyncDto>> GetMetadataTags(string metadataTagGroupCode, IEnumerable<string> parentCodes = null, IEnumerable<string> entryTypes = null, int? hierachyLevel = null)
        {
            var tokenResponse = await _authenticationTokenService.GetToken();
            var queryStringList = new List<string>();
            if (parentCodes != null)
            {
                queryStringList.AddRange(parentCodes.Select(p => $"relatedTo={p}"));
            }

            if (entryTypes != null)
            {
                queryStringList.AddRange(entryTypes.Select(p => $"entryTypes={p}"));
            }

            if (hierachyLevel != null)
            {
                queryStringList.Add($"hierachyLevel={hierachyLevel}");
            }

            queryStringList.Add("classifyItemType=false");

            var queryStringsUrlPart = queryStringList.Any() ? ("?" + string.Join("&", queryStringList)) : string.Empty;
            var result = await _httpService.GetAsync<IEnumerable<MetadataTagSyncDto>>($"{_apiOriginUrl}/catalogentries/explorer/{metadataTagGroupCode}{queryStringsUrlPart}", bearerToken: tokenResponse.AccessToken);
            return result.ToList();
        }
    }
}
