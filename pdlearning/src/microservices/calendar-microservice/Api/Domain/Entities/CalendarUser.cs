using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Calendar.Domain.Entities
{
    public class CalendarUser : Entity
    {
        /// <summary>
        /// Use to map user ID from original database.
        /// </summary>
        [Column("UserID")]
        public int OriginalUserId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public Guid? PrimaryApprovalOfficerId { get; set; }

        public Guid? AlternativeApprovalOfficerId { get; set; }

        /// <summary>
        /// Use to map entity status ID from status.
        /// </summary>
        [Column("EntityStatusID")]
        public int Status { get; set; }

        public string FullName()
        {
            return (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty);
        }
    }
}
