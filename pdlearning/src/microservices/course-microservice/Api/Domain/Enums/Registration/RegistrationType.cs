namespace Microservice.Course.Domain.Enums
{
    public enum RegistrationType
    {
        /// <summary>
        /// EF core cannot insert enum start from first item. Then we must be added None.
        /// </summary>
        None,

        /// <summary>
        /// Manual is when learner register a course manually.
        /// </summary>
        Manual,
        Application,
        Nominated,
        AddedByCA
    }
}
