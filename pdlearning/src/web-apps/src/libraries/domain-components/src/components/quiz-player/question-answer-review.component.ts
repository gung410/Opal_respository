import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  FormAnswerModel,
  FormQuestionAnswerModel,
  FormQuestionModel,
  FormQuestionModelSingleAnswerValue,
  FormType,
  QuestionAnswerSingleValue,
  QuestionAnswerValue,
  QuestionOptionType,
  QuestionType
} from '@opal20/domain-api';

@Component({
  selector: 'question-answer-review',
  templateUrl: './question-answer-review.component.html'
})
export class QuestionAnswerReviewComponent extends BaseComponent {
  @Input('question') public question: FormQuestionModel;
  @Input('priority') public priority: number;
  @Input('minorPriority') public minorPriority: number;
  @Input() set formAnswer(formAnswer: FormAnswerModel) {
    this._formAnswer = formAnswer;
    this.questionAnswer = formAnswer.questionAnswers.find(x => x.formQuestionId === this.question.id);
  }
  @Input() public formDataType: FormType;
  public questionAnswer: FormQuestionAnswerModel;

  public readonly QUESTIONTYPE = QuestionType;
  public readonly QUESTIONOPTIONTYPEENUM: typeof QuestionOptionType = QuestionOptionType;

  get formAnswer(): FormAnswerModel {
    return this._formAnswer;
  }
  public questionIdToFormAnswerDic: Dictionary<FormQuestionAnswerModel | undefined> = {};
  private _formAnswer: FormAnswerModel;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get isQuiz(): boolean {
    return this.formDataType === FormType.Quiz;
  }

  public getMediaUrl(meidaUrl: string): string {
    return meidaUrl ? `${AppGlobal.environment.cloudfrontUrl}/${meidaUrl}` : '';
  }

  public getTrueFalseQuestionOptionLabel(optionValue: FormQuestionModelSingleAnswerValue): string {
    return optionValue ? 'True' : 'False';
  }

  public isMultipleChoiceAnswerMatch(option: QuestionAnswerSingleValue): boolean {
    return (
      this.questionAnswer.answerValue !== undefined &&
      this.questionAnswer.answerValue instanceof Array &&
      this.questionAnswer.answerValue.indexOf(<never>option) >= 0
    );
  }

  public isCheckOptionValueCorrect(option: FormQuestionModel, value: QuestionAnswerSingleValue): boolean {
    return this.isQuiz ? option.isOptionValueCorrect(value) : null;
  }

  public isCheckAnswerCorrect(answerCorrectValue: QuestionAnswerValue): boolean | null {
    return this.isQuiz
      ? this.questionAnswer.answerValue && answerCorrectValue && this.questionAnswer.answerValue === answerCorrectValue
      : null;
  }

  public isCheckSingleOptionAnswerCorrect(option: FormQuestionModel): boolean {
    return this.isQuiz ? option.isAnswerCorrect(this.questionAnswer.answerValue) : null;
  }

  public getDatePickerAnswer(): Date {
    const dateAnswer = (this.questionAnswer.answerValue as unknown) as Date;
    return new Date(dateAnswer);
  }

  public getDateRangePickerAnswer(index: number): Date {
    return new Date(this.questionAnswer.answerValue[index]);
  }

  // For survey and poll
  public isHasAnswerValue(questionValue: string): boolean {
    return !this.isQuiz && this.questionAnswer.answerValue === questionValue;
  }

  public canEditScore(): boolean {
    return (
      this.formDataType === FormType.Quiz &&
      this.formAnswer.isCompleted &&
      (this.question.questionType === QuestionType.FillInTheBlanks ||
        this.question.questionType === QuestionType.ShortText ||
        this.question.questionType === QuestionType.FreeResponse)
    );
  }

  public onScoreChange(event: number): void {
    this.questionAnswer.markedScore = event;
  }
}
