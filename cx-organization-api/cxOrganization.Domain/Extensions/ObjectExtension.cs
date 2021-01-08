using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using Newtonsoft.Json;

namespace cxOrganization.Domain.Extensions
{
    public static class ObjectExtension
    {
        public static T DeepClone<T>(this T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
        public static bool IsNullOrEmpty(this IDictionary dictionary)
        {
            return dictionary==null || dictionary.Count==0;
        }

        public static string GetFullName(this UserEntity userEntity)
        {
            return $"{userEntity.FirstName} {userEntity.LastName}".Trim(' ');
        }
        public static void SetCurrentDepartment(this IEnumerable<HierachyDepartmentIdentityDto> hierarchyDepartmentIdentities, int currentDepartmentId)
        {
            var currentHierarchyDepartment = hierarchyDepartmentIdentities?.FirstOrDefault(h => h.Identity.Id == currentDepartmentId);

            if (currentHierarchyDepartment != null)
            {
                currentHierarchyDepartment.IsCurrentDepartment = true;
            }
        }

        public static string ConvertToJsonValueDateTimeString(this DateTime dateTime)
        {
            var utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return JsonConvert.SerializeObject(utcDateTime).Trim('"');
        }
    }
}
