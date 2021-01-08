using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.PDPM;

namespace LearnerApp.Common
{
    public class PDPlanMethod
    {
        public static async Task<string> AddCourseToPDPlan(string courseId, string userId, Func<PDO, Task<ApiResponse<PDOResponse>>> pdpmApi)
        {
            string learningOpportunityUri = $"{GlobalSettings.LearningOpportunityUri}:{courseId}";

            string isoDate = DateTime.UtcNow.ToString("o");

            var pdo = new PDO
            {
                ObjectiveInfo = new ObjectiveInfo
                {
                    Identity = new Identity
                    {
                        ExtId = userId
                    }
                },
                Answer = new Answer
                {
                    LearningOpportunity = new LearningOpportunity
                    {
                        Uri = learningOpportunityUri
                    },
                    AddedToIdpDate = isoDate
                },
                AdditionalProperties = new AdditionalProperties
                {
                    LearningOpportunityUri = learningOpportunityUri
                },
                Timestamp = isoDate
            };

            var pdoResponse = await pdpmApi(pdo);

            var resultId = string.Empty;

            if (!pdoResponse.HasEmptyResult())
            {
                resultId = pdoResponse.Payload?.ResultIdentity?.ExtId;
            }

            return resultId;
        }

        public static async Task DeactivateCourseInPDPlan(string resultId, Func<PDORemove, Task<ApiResponse<List<PDORemoveResponse>>>> pdpmApi)
        {
            PDORemove pDORemove = new PDORemove
            {
                Identities = new List<PDORemoveIdentity>
                    {
                        new PDORemoveIdentity
                        {
                            ExtId = resultId
                        }
                    }
            };

            await pdpmApi(pDORemove);
        }
    }
}
