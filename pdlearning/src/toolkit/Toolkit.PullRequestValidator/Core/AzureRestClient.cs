using System;
using System.Text;
using Conexus.Toolkit.PullRequestValidator.Models;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Conexus.Toolkit.PullRequestValidator.Core
{
    public class AzureRestClient
    {
        private const string ApiVersion = "api-version=5.1-preview.1";
        private const string HeaderJsonValue = "application/json";

        // TODO: Move to configuration file.
        private const string DefaultPAT = "vqhcgglduvl4j7y47p2purblrzwlfsy5gu6xngco2u3xqyn7osma";

        private readonly RestClient _client;
        private readonly ILogger _logger;
        private readonly string _personalAccessToken;

        public AzureRestClient(ILogger logger, Uri repositoryUrl, string token)
        {
            if (repositoryUrl == null)
            {
                throw new ArgumentNullException(nameof(repositoryUrl));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            _personalAccessToken = string.IsNullOrEmpty(token) ? GenerateAuthorizationHeader(DefaultPAT) : GenerateAuthorizationHeader(token);
            _client = new RestClient(repositoryUrl);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ExecutePost(RestRequest request, string requestBody)
        {
            var restClient = _client;

            request.Method = Method.POST;

            request.AddHeader("Content-Type", HeaderJsonValue);
            request.AddHeader("Authorization", _personalAccessToken);
            request.AddParameter(HeaderJsonValue, requestBody, ParameterType.RequestBody);

            var response = restClient.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response";
                throw new PullRequestValidatorException(message, response.ErrorException);
            }
        }

        public T ExecuteGet<T>(RestRequest request) where T : class, new()
        {
            request.Method = Method.GET;

            request.AddHeader("Content-Type", HeaderJsonValue);
            request.AddHeader("Authorization", _personalAccessToken);
            var response = _client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response";
                throw new PullRequestValidatorException(message, response.ErrorException);
            }

            return response.Data;
        }

        public void ExecutePatch(RestRequest request, string requestBody)
        {
            var restClient = _client;

            request.Method = Method.PATCH;

            request.AddHeader("Content-Type", HeaderJsonValue);
            request.AddHeader("Authorization", _personalAccessToken);
            request.AddParameter(HeaderJsonValue, requestBody, ParameterType.RequestBody);

            var response = restClient.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response";
                throw new PullRequestValidatorException(message, response.ErrorException);
            }
        }

        public PullRequestCommits GetPullRequestCommits(int pullRequestId)
        {
            _logger.LogInformation($"Begin to get commits from pullRequets {pullRequestId}");
            var pullRequestCommitsApi = $"pullRequests/{pullRequestId}/commits?{ApiVersion}";
            var request = new RestRequest(pullRequestCommitsApi);
            var data = ExecuteGet<PullRequestCommits>(request);
            return data;
        }

        public CommitChangesBody GetCommitChanges(string commitId)
        {
            _logger.LogInformation($"Begin to get commit changes from {commitId}");
            var commitChangeApi = $"commits/{commitId}/changes?{ApiVersion}";

            var request = new RestRequest(commitChangeApi);
            var data = ExecuteGet<CommitChangesBody>(request);
            return data;
        }

        public string ValidateContentContainingTabCharacter(Uri contentUrl, string path)
        {
            _logger.LogInformation($"Begin to get commit changes content from path file {path}");
            var request = new RestRequest(contentUrl)
            {
                Method = Method.GET
            };
            request.AddHeader("Accept", "text/plain");
            request.AddHeader("Authorization", _personalAccessToken);

            var response = _client.Execute(request);
            var type = response.ContentType;
            if (type.Contains("text/plain"))
            {
                var result = response.Content;
                if (result.Contains("\t"))
                {
                    return $"File path {path} contains tab character.";
                }
            }

            return string.Empty;
        }

        public void PostStatusOnPullRequest(int pullRequestId, string status)
        {
            string statusPRUrl = $"pullrequests/{pullRequestId}/statuses?{ApiVersion}";

            var request = new RestRequest(statusPRUrl);
            ExecutePost(request, status);
        }

        public void UpdatePullRequestTitle(int pullRequestId, string title)
        {
            var pullRequestApi = $"pullRequests/{pullRequestId}?{ApiVersion}";
            var request = new RestRequest(pullRequestApi);

            ExecutePatch(request, Newtonsoft.Json.JsonConvert.SerializeObject(new { Title = title }));
        }

        public string GenerateAuthorizationHeader(string pat)
        {
            string convertPAT = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
            return $"Basic {convertPAT}";
        }
    }

    public class PullRequestValidatorException : Exception
    {
        public PullRequestValidatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
