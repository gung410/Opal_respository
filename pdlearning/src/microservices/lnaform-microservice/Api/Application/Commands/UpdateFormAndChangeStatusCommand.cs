using System;

namespace Microservice.LnaForm.Application.Commands
{
    public class UpdateFormAndChangeStatusCommand : SaveFormCommand
    {
        /// <summary>
        /// The ID will be ID of new record if the form status changed to Unpublised.
        /// </summary>
        public Guid NewFormID { get; set; }

        /// <summary>
        /// Set this field to true if need update to new version.
        /// </summary>
        public bool? IsUpdateToNewVersion { get; set; }
    }
}
