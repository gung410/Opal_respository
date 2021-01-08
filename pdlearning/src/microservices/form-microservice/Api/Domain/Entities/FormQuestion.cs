using System;
using System.Diagnostics.CodeAnalysis;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class FormQuestion : OwnQuestionEntity, ISoftDelete
    {
        public Guid FormId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public bool? ShowFeedBackAfterAnswer { get; set; }

        public bool? RandomizedOptions { get; set; }

        public double? Score { get; set; }

        /// <summary>
        /// THIS COLUMN IS ONLY USED TO SHOW OPAL1 QUESTIONS DATA IN RELEASE 2.0
        /// WE MAY WILL IMPLEMENT THE APPROPRIATE SOLUTION LATER.
        /// </summary>
        public Guid? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? FormSectionId { get; set; }

        public bool? IsShareIdentityQuestion { get; set; }

        public bool IsScoreEnabled { get; set; }
    }
}
