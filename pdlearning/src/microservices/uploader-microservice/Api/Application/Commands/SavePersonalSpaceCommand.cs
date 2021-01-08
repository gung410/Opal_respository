using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Commands
{
    public class SavePersonalSpaceCommand : BaseThunderCommand
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public double TotalSpace { get; set; }

        public double TotalUsed { get; set; }

        public bool IsCreation { get; set; }

        public bool IsStorageUnlimited { get; set; }
    }
}
