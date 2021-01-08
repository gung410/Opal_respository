import { QuestionAnswerValue, QuestionType } from '../../form/models/form-question.model';

import { QuestionOption } from '../../form/models/form-question-option.model';

export interface IQuestionBank {
  id: string | undefined;
  title: string;
  questionGroupName: string;
  questionType: QuestionType;
  questionTitle: string;
  questionCorrectAnswer: QuestionAnswerValue | undefined;
  questionOptions: QuestionOption[] | undefined;

  questionHint: string | undefined;
  answerExplanatoryNote: string | undefined;
  feedbackCorrectAnswer: string | undefined;
  feedbackWrongAnswer: string | undefined;
  questionLevel: number | undefined;
  randomizedOptions: boolean | undefined;
  score: number | undefined;
  parentId: string | undefined;
  createdDate: Date;
  changedDate: Date | undefined;
  isDeleted: boolean | undefined;
  isScoreEnabled?: boolean;
}

export class QuestionBank implements IQuestionBank {
  public id: string | undefined;
  public title: string = '';
  public questionGroupName: string = '';
  public questionType: QuestionType = QuestionType.ShortText;
  public questionTitle: string = '';
  public questionCorrectAnswer: QuestionAnswerValue | undefined;
  public questionOptions: QuestionOption[] | undefined;
  public questionHint: string | undefined;
  public answerExplanatoryNote: string | undefined;
  public feedbackCorrectAnswer: string | undefined;
  public feedbackWrongAnswer: string | undefined;
  public questionLevel: number | undefined;
  public randomizedOptions: boolean | undefined;
  public score: number | undefined;
  public parentId: string | undefined;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public isDeleted: boolean | undefined;
  public isScoreEnabled?: boolean | undefined;

  constructor(data?: IQuestionBank) {
    if (data != null) {
      this.id = data.id;
      this.title = data.title;
      this.questionGroupName = data.questionGroupName;
      this.questionType = data.questionType;
      this.questionTitle = data.questionTitle;
      this.questionCorrectAnswer = data.questionCorrectAnswer;
      this.questionOptions = data.questionOptions;
      this.questionHint = data.questionHint;
      this.answerExplanatoryNote = data.answerExplanatoryNote;
      this.feedbackCorrectAnswer = data.feedbackCorrectAnswer;
      this.feedbackWrongAnswer = data.feedbackWrongAnswer;
      this.questionLevel = data.questionLevel;
      this.randomizedOptions = data.randomizedOptions;
      this.score = data.score;
      this.parentId = data.parentId;
      this.createdDate = data.createdDate;
      this.changedDate = data.changedDate;
      this.isDeleted = data.isDeleted;
      this.isScoreEnabled = data.isScoreEnabled;
    }
  }
}
