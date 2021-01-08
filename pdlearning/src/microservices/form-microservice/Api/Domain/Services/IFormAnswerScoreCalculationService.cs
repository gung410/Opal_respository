using System.Collections.Generic;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Enums;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Domain.Services
{
    public interface IFormAnswerScoreCalculationService
    {
        double CalcFormAnswerScore(FormEntity form, ICollection<FormQuestion> questions, FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers);

        double CalcFormAnswerScorePercentage(FormEntity form, ICollection<FormQuestion> questions, FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers);

        double CalcScorePercentage(double score, ICollection<FormQuestion> questions);

        double CalculateQuestionAnswerValueScore(FormQuestion formQuestion, object formQuestionAnswerValue);

        double CalcMaxScore(ICollection<FormQuestion> questions);

        double CalculateManualQuestionAnswerScore(FormQuestion formQuestion, double score);

        FormAnswerPassingStatus CalcFormAnswerPassingStatus(FormAnswer formAnswer, FormEntity form);
    }
}
