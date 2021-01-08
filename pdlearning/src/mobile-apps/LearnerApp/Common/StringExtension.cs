using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.UserOnBoarding;

namespace LearnerApp.Common
{
    public static class StringExtension
    {
        public static string GetDataInformationFromPrerequisiteCourses(List<string> datas, List<PrerequisiteCourse> preRequisiteCourses)
        {
            if (datas.IsNullOrEmpty() || preRequisiteCourses.IsNullOrEmpty())
            {
                return GlobalSettings.NotAvailable;
            }

            var listResult = datas.Join(
                preRequisiteCourses,
                data => data,
                preRequisite => preRequisite.Id,
                (post, course) => new { Text = course.CourseName }).ToList();

            var listStr = listResult.Select(list => list.Text).ToList();
            return GetInformationFromList(listStr);
        }

        public static string GetDataInformationFromDepartments(List<string> datas, List<Department> departments)
        {
            if (datas.IsNullOrEmpty() || departments.IsNullOrEmpty())
            {
                return GlobalSettings.NotAvailable;
            }

            var listResult = datas.Join(
                departments,
                data => data,
                preRequisite => preRequisite.Identity.Id.ToString(),
                (post, course) => new { Text = course.DepartmentName }).ToList();

            var listStr = listResult.Select(list => list.Text).ToList();
            return GetInformationFromList(listStr);
        }

        public static string GetDataInformationFromMetadatas(List<string> datas, List<MetadataTag> metadataTags)
        {
            if (datas.IsNullOrEmpty() || metadataTags.IsNullOrEmpty())
            {
                return GlobalSettings.NotAvailable;
            }

            var listResult = datas.Join(
                metadataTags,
                data => data,
                preRequisite => preRequisite.TagId,
                (post, meta) => new { Text = meta.DisplayText }).ToList();

            var listStr = listResult.Select(list => list.Text).ToList();
            return GetInformationFromList(listStr);
        }

        public static string GetInformation(string code, List<MetadataTag> metadataTags)
        {
            if (string.IsNullOrEmpty(code) || metadataTags.IsNullOrEmpty())
            {
                return GlobalSettings.NotAvailable;
            }

            var metadata = metadataTags.FirstOrDefault(meta => meta.TagId == code);
            return metadata == null ? GlobalSettings.NotAvailable : metadata.DisplayText;
        }

        public static string GetInformationFromList(List<string> listResult)
        {
            if (listResult.IsNullOrEmpty())
            {
                return GlobalSettings.NotAvailable;
            }

            var strBuilder = new StringBuilder();

            for (int index = 0; index < listResult.Count; index++)
            {
                string str = listResult[index];
                strBuilder.Append(str);

                if (index != listResult.Count - 1)
                {
                    strBuilder.Append(", ");
                }
            }

            return strBuilder.ToString();
        }

        public static string EscapeSinglePrime(this string html)
        {
            return string.IsNullOrEmpty(html) ? string.Empty : html.Replace(System.Environment.NewLine, string.Empty).Replace(@"\", @"\\").Replace("'", @"\'");
        }

        public static string RemoveTargetBlank(this string html)
        {
            return string.IsNullOrEmpty(html) ? string.Empty : Regex.Replace(html, "(<a.*?)target=\"_blank\"(.*?)>", "$1$2>");
        }

        public static string ReplaceExtensionUppercaseToLowercase(this string extension)
        {
            switch (extension)
            {
                case "PDF":
                    return "pdf";
                case "PNG":
                    return "png";
                case "JPEG":
                    return "jpeg";
                default:
                    return extension;
            }
        }
    }
}
