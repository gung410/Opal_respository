using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class QuestionBank : BaseEntity, ISoftDelete
    {
        public string Title { get; set; }

        public Guid? QuestionGroupId { get; set; }

        public string QuestionTitle { get; set; }

        public QuestionType QuestionType { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> QuestionOptions { get; set; }

        public string QuestionHint { get; set; }

        public string QuestionAnswerExplanatoryNote { get; set; }

        public string QuestionFeedbackCorrectAnswer { get; set; }

        public string QuestionFeedbackWrongAnswer { get; set; }

        public int? QuestionLevel { get; set; }

        public bool? RandomizedOptions { get; set; }

        public double? Score { get; set; }

        /// <summary>
        /// THIS COLUMN IS ONLY USED TO SHOW OPAL1 QUESTIONS DATA IN RELEASE 2.0
        /// WE MAY WILL IMPLEMENT THE APPROPRIATE SOLUTION LATER.
        /// </summary>
        public Guid? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsScoreEnabled { get; set; }
    }
}
