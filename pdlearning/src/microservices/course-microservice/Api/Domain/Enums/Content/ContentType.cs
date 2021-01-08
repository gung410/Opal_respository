using System.Diagnostics.CodeAnalysis;

namespace Microservice.Course.Domain.Enums
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public enum ContentType
    {
        Section,
        Lecture,
        Assignment
    }
}
