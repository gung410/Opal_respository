import { FormQuestionModel } from '../form/models/form-question.model';
import { FormType } from '../form/models/form.model';
import { Guid } from '@opal20/infrastructure';
import { QuestionBank } from '../question-bank/models/question-bank';

export class ModelMappingHelper {
  public static questionBankToFormQuestion(questionBank: QuestionBank, formType?: FormType): FormQuestionModel {
    const formQuestion = new FormQuestionModel();
    formQuestion.id = Guid.create().toString();
    formQuestion.questionLevel = questionBank.questionLevel;
    formQuestion.questionTitle = questionBank.questionTitle;
    formQuestion.questionType = questionBank.questionType;
    if (!formType || formType === FormType.Quiz) {
      formQuestion.answerExplanatoryNote = questionBank.answerExplanatoryNote;
      formQuestion.feedbackCorrectAnswer = questionBank.feedbackCorrectAnswer;
      formQuestion.feedbackWrongAnswer = questionBank.feedbackWrongAnswer;
      formQuestion.isRequiredAnswer = !!formType || null;
      formQuestion.questionCorrectAnswer = questionBank.questionCorrectAnswer;
      formQuestion.questionHint = questionBank.questionHint;
      formQuestion.randomizedOptions = questionBank.randomizedOptions;
      formQuestion.score = questionBank.score;
      formQuestion.isScoreEnabled = questionBank.isScoreEnabled;
    }
    formQuestion.questionOptions = questionBank.questionOptions.map(option => {
      option.feedback = !formType || formType === FormType.Quiz ? option.feedback : null;
      option.nextQuestionId = null;
      return option;
    });

    return formQuestion;
  }
}
