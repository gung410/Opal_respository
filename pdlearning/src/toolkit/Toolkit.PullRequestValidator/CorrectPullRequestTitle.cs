using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
    public class CorrectPullRequestTitle : BasePullRequestValidator
    {
        private static readonly Dictionary<Regex, string> _rulesTable = new Dictionary<Regex, string>
        {
            { new Regex("/microservices/learner.*"),  "LEARNER" },
            { new Regex("/modules/learner.*"),  "LEARNER" },
            { new Regex("/mobile-apps/.*"),  "MOBILE" },
            { new Regex("/microservices/content.*"),  "CCPM" },
            { new Regex("/microservices/form.*"),  "CCPM" },
            { new Regex("/microservices/tagging.*"),  "CCPM" },
            { new Regex("/modules/ccpm.*"),  "CCPM" },
            { new Regex("/microservices/course.*"),  "CAM-LMM" },
            { new Regex("/modules/cam.*"),  "CAM" },
            { new Regex("/modules/lmm.*"),  "LMM" },
            { new Regex(".*"),  "COMMON" }
        };

        private enum GroupType
        {
            Unknown,
            JiraTicket,
            Branch,
            Module
        }

        [FunctionName("CorrectPullRequestTitle")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string token = req.Query["authToken"].ToString();
                var data = await ParseRequestBody(req);

                var azureRestClient = new AzureRestClient(log, data.Resource.Repository.Url, token);
                string[] lstFileChangedPath = GetListFileChangedPath(data.Resource.LastMergeCommit.CommitId, azureRestClient);
                Dictionary<string, string> pathMapModuleNames = ConstructFilePathMapModuleName(lstFileChangedPath);
                var analyzeModuleScoreResult = AnalyzeModuleScore(pathMapModuleNames);
                string proposedModuleName = DecideCommitShouldBeModule(analyzeModuleScoreResult);

                string title = ExtractTitle(data);
                string correctedTitle = GetCorrectTitle(title, proposedModuleName);
                SendStatusAndUpdateTitle(azureRestClient, data.Resource.PullRequestId, title, correctedTitle, proposedModuleName);

                return new OkObjectResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new OkObjectResult(HttpStatusCode.InternalServerError);
            }
        }

        private void SendStatusAndUpdateTitle(
            AzureRestClient azureRestClient,
            int pullRequestId,
            string title,
            string correctedTitle,
            string proposedModuleName)
        {
            bool canChangeTitleAgain = correctedTitle != GetCorrectTitle(correctedTitle, proposedModuleName);
            bool hasFixme = correctedTitle.Contains("fix me");

            if (canChangeTitleAgain)
            {
                // WARNING: should not be here because it may have an issue in algorithm and cause a potential loop if call update title
                SendStatus(azureRestClient, pullRequestId, new
                {
                    State = "failed",
                    Description = "Title can't be validated.",
                    Context = new
                    {
                        Name = "pull-request-title-validate"
                    }
                });

                return;
            }

            if (hasFixme)
            {
                SendStatus(azureRestClient, pullRequestId, new
                {
                    State = "failed",
                    Description = "Title validation has failed.",
                    Context = new
                    {
                        Name = "pull-request-title-validate"
                    }
                });
                azureRestClient.UpdatePullRequestTitle(pullRequestId, correctedTitle);
            }
            else
            {
                SendStatus(azureRestClient, pullRequestId, new
                {
                    State = "succeeded",
                    Description = "Title is correct.",
                    Context = new
                    {
                        Name = "pull-request-title-validate"
                    }
                });

                // update only change
                if (title != correctedTitle)
                {
                    azureRestClient.UpdatePullRequestTitle(pullRequestId, correctedTitle);
                }
            }
        }

        private void SendStatus(AzureRestClient azureRestClient, int pullRequestId, object status)
        {
            azureRestClient.PostStatusOnPullRequest(pullRequestId, JsonConvert.SerializeObject(status));
        }

        private string DecideCommitShouldBeModule(Dictionary<string, int> analyzeModuleScoreResult)
        {
            // experimental!!
            // aggregate version of linq MaxBy (find module with max score)
            return analyzeModuleScoreResult.Aggregate((i, j) => i.Value > j.Value ? i : j).Key;
        }

        private Dictionary<string, int> AnalyzeModuleScore(Dictionary<string, string> pathMapModuleNames)
        {
            // experimental!!
            return pathMapModuleNames.GroupBy(
                    k => k.Value,
                    k => k)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, string> ConstructFilePathMapModuleName(string[] lstFileChangedPath)
        {
            var rulesTable = _rulesTable;
            return lstFileChangedPath.ToDictionary(
                p => p,
                p => rulesTable.FirstOrDefault(r => r.Key.IsMatch(p))
                    .Value);
        }

        private string[] GetListFileChangedPath(string commitId, AzureRestClient azureRestClient)
        {
            var commitChanges = azureRestClient.GetCommitChanges(commitId);

            return commitChanges.Changes.Where(c => !c.Item.IsFolder).Select(c => c.Item.Path).ToArray();
        }

        private string ExtractTitle(PullRequestInfo pri)
        {
            return pri.Resource.Title;
        }

        private string GetCorrectTitle(string title, string proposedModuleName)
        {
            // Refer: https://stackoverflow.com/questions/2403122/regular-expression-to-extract-text-between-square-brackets
            var regex = new Regex("\\[(.*?)\\]");
            var groups = regex.Matches(title);
            var predefinedGroups = groups.SelectMany(g => g.Captures.Select(c => c.Value)).ToArray();
            var classifiedGroups = ClassifyGroups(predefinedGroups);
            title = RemoveBracketGroupsInTitle(title, predefinedGroups);
            var prefix = BuildPreFix(classifiedGroups, proposedModuleName);

            return $"{prefix} {title}";
        }

        private string BuildPreFix((string group, GroupType type)[] predefinedGroups, string proposedModuleName)
        {
            return $"{(HasGroup(predefinedGroups, GroupType.JiraTicket) ? GetGroup(predefinedGroups, GroupType.JiraTicket) : "[fix me (jira ticket)]")}" +
                   $"{(HasGroup(predefinedGroups, GroupType.Branch) ? GetGroup(predefinedGroups, GroupType.Branch) : "[fix me (Branch)]")}" +
                   $"{(HasGroup(predefinedGroups, GroupType.Module) ? GetGroup(predefinedGroups, GroupType.Module) : "[" + proposedModuleName + "]")}";

            bool HasGroup((string group, GroupType type)[] groups, GroupType type)
            {
                return groups.Any(g => g.type == type);
            }

            string GetGroup((string group, GroupType type)[] groups, GroupType type)
            {
                return groups.FirstOrDefault(g => g.type == type).group;
            }
        }

        private string RemoveBracketGroupsInTitle(string title, string[] groupBrackets)
        {
            string newTitle = title;
            foreach (var groupBracket in groupBrackets)
            {
                newTitle = newTitle.Replace(groupBracket, string.Empty);
            }

            return newTitle.Trim();
        }

        private (string group, GroupType type)[] ClassifyGroups(string[] groupBrackets)
        {
            return groupBrackets.Select(g =>
            {
                var token = g.Replace("[", string.Empty).Replace("]", string.Empty);
                if (IsJiraTicket(token))
                {
                    return (g, GroupType.JiraTicket);
                }

                if (IsBranch(token))
                {
                    return (g, GroupType.Branch);
                }

                if (IsModule(token))
                {
                    return (g, GroupType.Module);
                }

                return (g, GroupType.Unknown);
            }).ToArray();

            bool IsModule(string token)
            {
                var upperToken = token.ToUpper();
                switch (upperToken)
                {
                    case "LEARNER":
                    case "CAM":
                    case "LMM":
                    case "CAM-LMM":
                    case "CCPM":
                    case "COMMON":
                        return true;
                    default: return false;
                }
            }

            bool IsBranch(string token)
            {
                // match 1.0, 2.1, 2.3, 11.3, 13.14 but not 12.2a 2.2b
                return Regex.IsMatch(token, "^\\d+\\.\\d+$", RegexOptions.IgnoreCase);
            }

            bool IsJiraTicket(string token)
            {
                return Regex.IsMatch(token, "^(OP|OPX)-\\d+$", RegexOptions.IgnoreCase) || token.ToUpper() == "NONE";
            }
        }
    }
}
