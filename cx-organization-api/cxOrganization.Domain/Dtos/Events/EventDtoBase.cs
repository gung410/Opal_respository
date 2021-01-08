using System;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Client.ConexusBase;

namespace cxEvent.Client
{
    [Serializable]
    public abstract class EventDtoBase : ConexusBaseDto
    {
        /// <summary>
        /// Represent to object of an event is point to (ex)
        /// </summary>
        public IdentityBaseDto ObjectIdentity { get; set; }
        /// <summary>
        /// User Identity: who did that?
        /// </summary>
        public IdentityBaseDto UserIdentity { get; set; }
        /// <summary>
        /// Department Identity: Where that happened?
        /// </summary>
        public IdentityBaseDto DepartmentIdentity { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// Json for the additional information
        /// </summary>
        public object AdditionalInformation { get; set; }
        /// <summary>
        /// Application name
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }
        /// <summary>
        /// Event type name.
        /// </summary>
        /// <value>
        /// Event type name.
        /// </value>
        [Required]
        public string EventTypeName { get; set; }
        /// <summary>
        /// The correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Group Identity
        /// </summary>
        public IdentityBaseDto GroupIdentity { get; set; }
        /// <summary>
        /// Who created the event
        /// </summary>
        public int? CreatedBy { get; set; }
    }
}
