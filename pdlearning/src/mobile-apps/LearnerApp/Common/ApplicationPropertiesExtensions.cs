using System.Collections.Generic;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services.Identity;

namespace LearnerApp.Common
{
    public static class ApplicationPropertiesExtensions
    {
        public static void AddAccountProperties(this IDictionary<string, object> dictionary, IdentityModel accountProperties)
        {
            if (accountProperties == null)
            {
                return;
            }

            if (dictionary.ContainsKey("account-properties"))
            {
                dictionary["account-properties"] = accountProperties;
            }
            else
            {
                dictionary.Add("account-properties", accountProperties);
            }
        }

        public static IdentityModel GetAccountProperties(this IDictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("account-properties"))
            {
                return dictionary["account-properties"] as IdentityModel;
            }

            return null;
        }

        public static void AddMetadataTagging(this IDictionary<string, object> dictionary, List<MetadataTag> metaData)
        {
            if (metaData.IsNullOrEmpty())
            {
                return;
            }

            if (dictionary.ContainsKey("metadata-tagging"))
            {
                dictionary["metadata-tagging"] = metaData;
            }
            else
            {
                dictionary.Add("metadata-tagging", metaData);
            }
        }

        public static List<MetadataTag> GetMetadataTagging(this IDictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("metadata-tagging"))
            {
                return dictionary["metadata-tagging"] as List<MetadataTag>;
            }

            return null;
        }

        public static void AddMetadataSearching(this IDictionary<string, object> dictionary, List<PrerequisiteCourse> metaData)
        {
            if (metaData.IsNullOrEmpty())
            {
                return;
            }

            if (dictionary.ContainsKey("metadata-searching"))
            {
                dictionary["metadata-searching"] = metaData;
            }
            else
            {
                dictionary.Add("metadata-searching", metaData);
            }
        }

        public static List<PrerequisiteCourse> GetMetadataSearching(this IDictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("metadata-searching"))
            {
                return dictionary["metadata-searching"] as List<PrerequisiteCourse>;
            }

            return null;
        }

        public static void AddMetadataDepartment(this IDictionary<string, object> dictionary, List<Department> metaData)
        {
            if (metaData.IsNullOrEmpty())
            {
                return;
            }

            if (dictionary.ContainsKey("metadata-department"))
            {
                dictionary["metadata-department"] = metaData;
            }
            else
            {
                dictionary.Add("metadata-department", metaData);
            }
        }

        public static List<Department> GetMetadataDepartment(this IDictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("metadata-department"))
            {
                return dictionary["metadata-department"] as List<Department>;
            }

            return null;
        }

        public static void AddSiteInformation(this IDictionary<string, object> dictionary, string releaseDate)
        {
            if (string.IsNullOrEmpty(releaseDate))
            {
                return;
            }

            if (dictionary.ContainsKey("site-information"))
            {
                dictionary["site-information"] = releaseDate;
            }
            else
            {
                dictionary.Add("site-information", releaseDate);
            }
        }

        public static string GetSiteInformation(this IDictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("site-information"))
            {
                return dictionary["site-information"] as string;
            }

            return string.Empty;
        }
    }
}
