using System;
using System.ComponentModel.DataAnnotations;

namespace Microservice.Content.Versioning.Entities
{
    public interface IVersioningFields
    {
        /// <summary>
        /// The ID of previous version.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// The root object Id.
        /// </summary>
        public Guid OriginalObjectId { get; set; }

        /// <summary>
        /// Used to mark record as old version.
        /// </summary>
        [Required]
        public bool IsArchived { get; set; }
    }
}
