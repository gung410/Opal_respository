using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    public class SharedQuestion : OwnQuestionEntity, ISoftDelete
    {
        public Guid OwnerId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
