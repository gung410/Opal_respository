using System.Diagnostics.CodeAnalysis;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class QuestionGroup : Entity
    {
        public string Name { get; set; }
    }
}
