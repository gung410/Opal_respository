using System.Collections.Generic;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Domain
{
    public interface IOwnQuestionEntity
    {
        QuestionType Question_Type { get; set; }

        string Question_Title { get; set; }

        object Question_CorrectAnswer { get; set; }

        IEnumerable<QuestionOption> Question_Options { get; set; }

        string Question_Hint { get; set; }

        string Question_AnswerExplanatoryNote { get; set; }

        string Question_FeedbackCorrectAnswer { get; set; }

        string Question_FeedbackWrongAnswer { get; set; }

        int? Question_Level { get; set; }
    }
}
