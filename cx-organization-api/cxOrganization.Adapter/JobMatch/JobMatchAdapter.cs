using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using cxOrganization.Adapter.JobMatch.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cxOrganization.Adapter.JobMatch
{
    public class JobMatchAdapter : IJobMatchAdapter
    {
        private static ILogger _logger;

        private readonly string _jobmatchApiBaseUrl;
        private readonly string _authenticationToken;
        private readonly HttpClient _httpClient;

        public JobMatchAdapter(string jobmatchApiBaseUrl, string authenticationToken, ILoggerFactory loggerFactory)
        {
            _jobmatchApiBaseUrl = (jobmatchApiBaseUrl ?? "").TrimEnd('/');
            _authenticationToken = authenticationToken;
            _httpClient = new HttpClient();
            _logger = loggerFactory.CreateLogger<JobMatchAdapter>();
        }

        private HttpRequestMessage InitRequest(HttpMethod method, string correlationId, string resourceStartWithSlash)
        {
            var request = new HttpRequestMessage(method, string.Format("{0}{1}", _jobmatchApiBaseUrl, resourceStartWithSlash));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Authorization", _authenticationToken);
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("Correlation-Id", correlationId);
            }
            return request;
        }

        public List<JobmatchesDto> GetJobmatchesFromRiasecLetters(string correlationId, string riasecLetters, List<ClassificationEnum> classifications, RiasecLetterCombinationEnum combination, string locale)
        {

            var resourceBuilder = new StringBuilder("/jobmatch?");
            resourceBuilder.AppendFormat("riasec={0}&combination={1}", riasecLetters, combination);

            if (classifications != null && classifications.Count > 0)
            {
                foreach (var classification in classifications)
                {
                    resourceBuilder.AppendFormat("&classifications={0}", classification);
                }
            }
            var request = InitRequest(HttpMethod.Get, correlationId, resourceBuilder.ToString());
            if (!string.IsNullOrEmpty(locale))
            {
                request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(locale));
            }

            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<List<JobmatchesDto>>(responseContent) ?? new List<JobmatchesDto>();
            }
            else
            {
                LogFailResponse(request, response);
            }

            return new List<JobmatchesDto>();
        }

        private void LogFailResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            
            var logMessageBuilder = new StringBuilder(request.RequestUri.GetLeftPart(System.UriPartial.Authority));
            logMessageBuilder.AppendFormat(" has responded status {0}-{1}", (int)response.StatusCode, response.StatusCode);
            logMessageBuilder.AppendFormat(" for request [{0}] {1} {2}.", GetRequestIdFromResponse(response), request.Method, request.RequestUri);
            logMessageBuilder.AppendFormat(" Response message: {0}.", response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if ((int)response.StatusCode >= 500)
            {
                _logger.LogError(logMessageBuilder.ToString());
            }
            else
            {
                _logger.LogWarning(logMessageBuilder.ToString());
            }
        }

        private string GetRequestIdFromResponse(HttpResponseMessage responseMessage)
        {
            IEnumerable<string> values;
            if(responseMessage.Headers.TryGetValues("Request-Id", out values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }
        public List<JobmatchDto> GetJobmatchesFromRiasecLettersByGroupLevel(string correlationId, JobmatchGroupLevel groupLevel, string riasecLetters, List<ClassificationEnum> classifications, RiasecLetterCombinationEnum combination, string locale)
        {

            var jobmatchesDtos = GetJobmatchesFromRiasecLetters(correlationId, riasecLetters, classifications, combination, locale);
            return ExtractJobmatchDtos(groupLevel, jobmatchesDtos);
        }
      
        private List<JobmatchDto> ExtractJobmatchDtos(JobmatchGroupLevel groupLevel, List<JobmatchesDto> jobmatchesDtos)
        {
            var jobmatchDtos = new List<JobmatchDto>();
            if (jobmatchDtos == null) return jobmatchDtos;

            foreach (var jobmatchesDto in jobmatchesDtos)
            {
                if (jobmatchesDto.MajorGroups == null) continue;

                foreach (var majorGroup in jobmatchesDto.MajorGroups)
                {
                    if (majorGroup.Jobmatches != null && (groupLevel == JobmatchGroupLevel.MajorGroup || groupLevel == JobmatchGroupLevel.All))
                    {
                        jobmatchDtos.AddRange(majorGroup.Jobmatches);
                        if (groupLevel == JobmatchGroupLevel.MajorGroup)
                            continue;
                    }

                    if (majorGroup.MinorGroups == null) continue;

                    foreach (var minorGroup in majorGroup.MinorGroups)
                    {
                        if (minorGroup.Jobmatches != null && (groupLevel == JobmatchGroupLevel.MinorGroup || groupLevel == JobmatchGroupLevel.All))
                        {
                            jobmatchDtos.AddRange(minorGroup.Jobmatches);
                            if (groupLevel == JobmatchGroupLevel.MinorGroup)
                                continue;
                        }

                        if (minorGroup.BroadOccupations == null) continue;

                        foreach (var broadOccupation in minorGroup.BroadOccupations)
                        {
                            if (broadOccupation.Jobmatches != null && (groupLevel == JobmatchGroupLevel.BroadOccupation || groupLevel == JobmatchGroupLevel.All))
                            {
                                jobmatchDtos.AddRange(broadOccupation.Jobmatches);
                                if (groupLevel == JobmatchGroupLevel.BroadOccupation)
                                    continue;
                            }

                            if (broadOccupation.DetailedOccupations == null) continue;

                            foreach (var detailedOccupations in broadOccupation.DetailedOccupations)
                            {
                                if (detailedOccupations.Jobmatches != null && (groupLevel == JobmatchGroupLevel.DetailedGroup || groupLevel == JobmatchGroupLevel.All))
                                {
                                    jobmatchDtos.AddRange(detailedOccupations.Jobmatches);

                                }
                            }
                        }
                    }
                }                 

            }
            return jobmatchDtos;
        }

      

    }
}

