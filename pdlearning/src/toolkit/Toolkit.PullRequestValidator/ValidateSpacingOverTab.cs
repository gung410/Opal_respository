using System;
using System.Net;
using System.Threading.Tasks;
using Conexus.Toolkit.PullRequestValidator.Core;
using Conexus.Toolkit.PullRequestValidator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator
{
    /// <summary>
    /// This is a pull request validator to validate pull request whether it has tab character or not.
    /// </summary>
    public class ValidateSpacingOverTab : BasePullRequestValidator
    {
        [FunctionName("ValidateSpacingOverTab")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string token = req.Query["authToken"].ToString();
                var data = await ParseRequestBody(req);

                var azureRestClient = new AzureRestClient(log, data.Resource.Repository.Url, token);
                var pullRequestCommits = azureRestClient.GetPullRequestCommits(data.Resource.PullRequestId);
                var status = ComputeStatus(pullRequestCommits, azureRestClient, log);

                log.LogInformation($"Service Hook Received for PR {data.Resource.PullRequestId} with status {status}");

                azureRestClient.PostStatusOnPullRequest(data.Resource.PullRequestId, status);

                return new OkObjectResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new OkObjectResult(HttpStatusCode.InternalServerError);
            }
        }

        private static string ComputeStatus(PullRequestCommits pullRequestCommits, AzureRestClient azureRestClient, ILogger log)
        {
            string state = "succeeded";
            string description = "Spacing Validation Succeeded";
            foreach (var commit in pullRequestCommits.Value)
            {
                string commitId = commit.CommitId;
                var commitChange = azureRestClient.GetCommitChanges(commitId);
                string validationResult = CheckTabCharacterFromCommit(commitChange, azureRestClient, log);

                if (!string.IsNullOrEmpty(validationResult))
                {
                  state = "failed";
                  description = validationResult;
                  break;
                }
            }

            return JsonConvert.SerializeObject(
                new
                {
                    State = state,
                    Description = description,
                    Context = new
                    {
                        Name = "validate-spacing-over-tab",
                        Genre = "chk_thunder-toolkit"
                    }
                });
        }

        private static string CheckTabCharacterFromCommit(CommitChangesBody commitChange, AzureRestClient azureRestClient, ILogger log)
        {
            foreach (var change in commitChange.Changes)
            {
                if (change.Item.IsFolder || change.Item.Path.Contains(".sln"))
                {
                    continue;
                }

                string validationResult = azureRestClient.ValidateContentContainingTabCharacter(change.Item.Url, change.Item.Path);
                if (string.IsNullOrEmpty(validationResult))
                {
                    continue;
                }

                log.LogInformation(validationResult);
                var description = $"Pull request change file have tab character. {validationResult}";
                return description;
            }

            return string.Empty;
        }
    }
}
