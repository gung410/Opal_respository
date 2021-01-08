using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class DeleteFormCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }
    }
}
