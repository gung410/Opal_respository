import { QuestionAnswerValue, QuestionType } from '../../form/models/form-question.model';

import { QuestionBank } from '../models/question-bank';
import { QuestionOption } from '../../form/models/form-question-option.model';

export interface ISaveQuestionBankRequest {
  id?: string;
  title: string;
  questionGroupName: string;
  questionTitle: string;
  questionType: QuestionType;
  questionCorrectAnswer: QuestionAnswerValue;
  questionOptions: QuestionOption[];
  questionHint: string | null;
  answerExplanatoryNote: string | undefined;
  feedbackCorrectAnswer: string | undefined;
  feedbackWrongAnswer: string | undefined;
  questionLevel: number | undefined;
  randomizedOptions: boolean | undefined;
  score: number | undefined;
  isDeleted: boolean;
  isScoreEnabled?: boolean;
}

export class SaveQuestionBankRequest {
  public id?: string;
  public title: string;
  public questionGroupName: string;
  public questionTitle: string;
  public questionType: QuestionType;
  public questionCorrectAnswer: QuestionAnswerValue;
  public questionOptions: QuestionOption[];
  public questionHint: string | null;
  public answerExplanatoryNote: string | undefined;
  public feedbackCorrectAnswer: string | undefined;
  public feedbackWrongAnswer: string | undefined;
  public questionLevel: number | undefined;
  public randomizedOptions: boolean | undefined;
  public score: number | undefined;
  public isDeleted: boolean;
  public isScoreEnabled?: boolean;

  constructor(data: QuestionBank) {
    if (data === undefined) {
      return;
    }

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
    this.isDeleted = data.isDeleted;
    this.isScoreEnabled = data.isScoreEnabled;
  }
}
