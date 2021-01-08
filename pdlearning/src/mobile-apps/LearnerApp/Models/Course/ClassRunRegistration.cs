using System.Collections.Generic;

namespace LearnerApp.Models.Course
{
    public class ClassRunRegistration
    {
        public string RegistrationType { get; set; } = "Manual";

        public string ApprovingOfficer { get; set; }

        public string AlternativeApprovingOfficer { get; set; }

        public List<Registration> Registrations { get; set; }
    }
}
