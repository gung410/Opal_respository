using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atlassian.Jira;
using Conexus.Toolkit.PullRequestValidator.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator
{
    /// <summary>
    /// This is a pull request validator to validate pull request whether it has a JIRA ticket or not.
    /// </summary>
    public class ValidateJiraTicket : BasePullRequestValidator
    {
        [FunctionName("ValidateJiraTicket")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string token = req.Query["authToken"].ToString();
                string jiraUserName = req.Query["jiraUserName"].ToString();
                string jiraPassword = req.Query["jiraPassword"].ToString();
                var data = await ParseRequestBody(req);

                var azureRestClient = new AzureRestClient(log, data.Resource.Repository.Url, token);
                var status = await ComputeStatus(data.Resource.Title, jiraUserName, jiraPassword);
                azureRestClient.PostStatusOnPullRequest(data.Resource.PullRequestId, status);

                return new OkObjectResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new OkObjectResult(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<string> ComputeStatus(string pullRequestTitle, string jiraUserName, string jiraPassword)
        {
            var jiraClient = Jira.CreateRestClient("https://cxtech.atlassian.net/", jiraUserName, jiraPassword);
            var jiraIssueKey = ExtractJiraIssueFromPrTitle(pullRequestTitle);

            if ("None".Equals(jiraIssueKey, StringComparison.OrdinalIgnoreCase))
            {
                return JsonConvert.SerializeObject(
                    new
                    {
                        State = "succeeded",
                        Description = "No need Jira Issue link.",
                        Context = new
                        {
                            Name = "jira-issue-linked"
                        }
                    });
            }

            try
            {
                var jiraIssue = await jiraClient.Issues.GetIssueAsync(jiraIssueKey);
                var jiraIssueUrl = $"https://cxtech.atlassian.net/browse/{jiraIssueKey}";

                if (jiraIssue != null && !string.IsNullOrEmpty(jiraIssue.JiraIdentifier))
                {
                    return JsonConvert.SerializeObject(
                        new
                        {
                            State = "succeeded",
                            Description = $"JiraIssueKey: {jiraIssueKey}",
                            Context = new
                            {
                                Name = "jira-issue-linked"
                            },
                            TargetUrl = jiraIssueUrl
                        });
                }
            }
            catch (Exception ex)
            {
                return FailResponse(ex.Message);
            }

            return FailResponse();
        }

        private string ExtractJiraIssueFromPrTitle(string pullRequestTitle)
        {
            if (string.IsNullOrEmpty(pullRequestTitle))
            {
                return string.Empty;
            }

            var regex = new Regex(@"[^\[\]]+");
            var matchedValue = regex.Match(pullRequestTitle);

            return matchedValue.Success ? matchedValue.Value : string.Empty;
        }

        private string FailResponse(string error = null)
        {
            return JsonConvert.SerializeObject(
                new
                {
                    State = "failed",
                    Description = $"Jira Issue Not Found with error {error}",
                    Context = new
                    {
                        Name = "jira-issue-linked"
                    }
                });
        }
    }
}
