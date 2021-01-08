using System.Collections.Generic;
using LearnerApp.Models.PDPM;

namespace LearnerApp.Models
{
    public class UserManagement
    {
        public JsonDynamicAttributes JsonDynamicAttributes { get; set; }

        public List<GroupUserManagement> Groups { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public int DepartmentId { get; set; }

        public Identity Identity { get; set; }
    }
}
