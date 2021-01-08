using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using cxOrganization.Adapter.JobChannel.Models;
using Newtonsoft.Json;

namespace cxOrganization.Adapter.JobChannel
{
    public class JobChannelAdapter : IJobChannelAdapter
    {
        private readonly string _jobChannelApiBaseUrl;
        private readonly string _serectKey;
        private readonly HttpClient _httpClient;
        public JobChannelAdapter(string jobChannelApiBaseUrl, string serectKey)
        {

            _jobChannelApiBaseUrl = (jobChannelApiBaseUrl ?? "").TrimEnd('/');
            _serectKey = serectKey;
            _httpClient=new HttpClient();

        }

        private HttpRequestMessage InitRequest(HttpMethod method, string resourceStartWithSlash, object content)
        {
            var request = new HttpRequestMessage(method, string.Format("{0}{1}", _jobChannelApiBaseUrl, resourceStartWithSlash))
            {
                Content = new StringContent(JsonConvert.SerializeObject(content),Encoding.UTF8,"application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("secretKey", _serectKey);
            return request;
        }

        private List<CvCompletenessStatus> GetCvCompletenessStatusesFromJobChannel(List<string> objectIds)
        {
            var request = InitRequest(HttpMethod.Post, "/api/candidates-b2b/cv-completeness", objectIds);
            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<List<CvCompletenessStatus>>(responseContent);
            }
            return new List<CvCompletenessStatus>();
        }

        public List<dynamic> GetCvCompletenessStatuses(List<string> objectIds)
        {
            var completenessStatues = new List<dynamic>();
            var rawCvCompelenessStatuses = GetCvCompletenessStatusesFromJobChannel(objectIds);
            foreach (var rawCvCompelenessStatus in rawCvCompelenessStatuses)
            {
                dynamic comletenessStatus = new ExpandoObject();
                comletenessStatus.ObjectId = rawCvCompelenessStatus.ObjectId;
                comletenessStatus.Completeness = rawCvCompelenessStatus.CV == null ? 0 : rawCvCompelenessStatus.CV.Completeness;
                completenessStatues.Add(comletenessStatus);
            }
            return completenessStatues;
        }
    }
}