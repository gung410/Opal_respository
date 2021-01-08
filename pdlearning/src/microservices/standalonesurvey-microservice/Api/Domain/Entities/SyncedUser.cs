using Conexus.Opal.AccessControl.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SyncedUser : UserEntity
    {
        public string DepartmentName { get; set; }
    }
}
