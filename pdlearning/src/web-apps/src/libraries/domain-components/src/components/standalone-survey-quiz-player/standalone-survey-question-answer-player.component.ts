import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {
  StandaloneSurveyQuestionAnswerModel,
  StandaloneSurveyQuestionAnswerService,
  StandaloneSurveyQuestionAnswerSingleValue,
  StandaloneSurveyQuestionModelSingleAnswerValue,
  StandaloneSurveyQuestionOption,
  StandaloneSurveyQuestionOptionType,
  StandaloneSurveyQuestionType,
  SurveyQuestionModel,
  SurveyWithQuestionsModel
} from '@opal20/domain-api';

import { StandaloneSurveyQuizPlayerPageService } from '../../services/standalone-survey-quiz-player-page.service';

@Component({
  selector: 'standalone-survey-question-answer-player',
  templateUrl: './standalone-survey-question-answer-player.component.html'
})
export class StandaloneSurveyQuestionAnswerPlayerComponent extends BaseComponent {
  public readonly questionOptionTypeEnum: typeof StandaloneSurveyQuestionOptionType = StandaloneSurveyQuestionOptionType;
  public readonly questionType: typeof StandaloneSurveyQuestionType = StandaloneSurveyQuestionType;
  public renderQuestionOptions: StandaloneSurveyQuestionOption[] | undefined;
  public displayAnswerResultToaster: boolean = false;
  public feedbackOnSelectedOption: string;
  public safeQuestionTitle: SafeHtml;

  @Input() public formData: SurveyWithQuestionsModel;
  @Input() public positionInForm: string | number = 1;
  public get optionCodeOrderList(): string[] | undefined {
    return this._optionCodeOrderList;
  }
  @Output('questionAnswerChange') public questionAnswerChangeEvent: EventEmitter<StandaloneSurveyQuestionAnswerModel> = new EventEmitter();
  @ViewChild('explantion', { static: false }) public explantion: ElementRef;
  @ViewChild('feedback', { static: false }) public feedback: ElementRef;

  public onShortTextQuestionAnswerChanged: (value: string) => void = createDebounceFn((value: string) => {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
  });

  public onLongTextQuestionAnswerChanged: (value: string) => void = createDebounceFn((value: string) => {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
  });

  public onDatePickerQuestionAnswerChanged: (value: Date) => void = createDebounceFn((value: Date) => {
    if (!value) {
      return;
    }
    this.updateFormQuestionAnswer(question => {
      question.answerValue = value.toDateString();
    });
  });
  protected _visualQuestionOptionAnswers: StandaloneSurveyQuestionModelSingleAnswerValue[];

  private _question: SurveyQuestionModel;
  private _optionCodeOrderList: string[] | undefined;
  private _questionAnswer: StandaloneSurveyQuestionAnswerModel;

  constructor(
    private mainQuizPlayerService: StandaloneSurveyQuizPlayerPageService,
    private sanitizer: DomSanitizer,
    protected formQuestionAnswerService: StandaloneSurveyQuestionAnswerService,
    moduleFacadeService: ModuleFacadeService
  ) {
    super(moduleFacadeService);
  }

  @Input()
  public set question(v: SurveyQuestionModel) {
    if (this._question && this._question.id === v.id) {
      return;
    }

    this._question = new SurveyQuestionModel(JSON.parse(JSON.stringify(v)));
    this.displayAnswerResultToaster = false;
    if (this._visualQuestionOptionAnswers && this._visualQuestionOptionAnswers.length) {
      this._visualQuestionOptionAnswers = [];
    }
  }
  public get question(): SurveyQuestionModel {
    return this._question;
  }

  @Input()
  public set questionAnswer(v: StandaloneSurveyQuestionAnswerModel) {
    if (!Utils.isDifferent(this._questionAnswer, v)) {
      return;
    }
    const prevQuestionAnswer = this._questionAnswer;
    this._questionAnswer = v;
    if (this.initiated) {
      if (prevQuestionAnswer.submittedDate === undefined && v.submittedDate !== undefined) {
        this.showAnswerResultToaster();
      }
    }
  }
  public get questionAnswer(): StandaloneSurveyQuestionAnswerModel {
    return this._questionAnswer;
  }

  @Input()
  public set optionCodeOrderList(v: string[] | undefined) {
    if (!Utils.isDifferent(this._optionCodeOrderList, v)) {
      return;
    }
    this._optionCodeOrderList = v;
    if (this.initiated) {
      this.renderQuestionOptions = this.getRenderQuestionOptions();
    }
  }

  public onDateRangePickerQuestionCorrectAnswerChanged(value: Date, isFromDate: boolean): void {
    if (!value) {
      return;
    }
    this.updateFormQuestionAnswer(question => {
      if (!(question.answerValue instanceof Array)) {
        question.answerValue = new Array();
      }
      if (isFromDate) {
        question.answerValue[0] = value.toDateString();
      } else {
        question.answerValue[1] = value.toDateString();
      }
    });
  }

  public getDateRangeAnswerValue(isFromDateAnswer: boolean): Date {
    const index = isFromDateAnswer ? 0 : 1;
    return this.questionAnswer.answerValue && this.questionAnswer.answerValue[index]
      ? new Date(this.questionAnswer.answerValue[index])
      : null;
  }

  public isMultipleChoiceAnswerMatch(
    model: StandaloneSurveyQuestionAnswerModel,
    option: StandaloneSurveyQuestionAnswerSingleValue
  ): boolean {
    return StandaloneSurveyQuestionAnswerModel.isMultipleChoiceAnswerMatch(model, option);
  }

  public updateFormQuestionAnswer(updatefn: (data: StandaloneSurveyQuestionAnswerModel) => void): void {
    this.questionAnswer = Utils.clone(this.questionAnswer, p => {
      updatefn(p);
    });
    this.questionAnswerChangeEvent.emit(this.questionAnswer);
  }

  public onFillInTheBlankQuestionAnswerSingleValueFilledUp(
    value: StandaloneSurveyQuestionModelSingleAnswerValue,
    currentOptionIndex: number
  ): void {
    this.updateFormQuestionAnswer(p => {
      if (!this._visualQuestionOptionAnswers) {
        this._visualQuestionOptionAnswers = [];
      }
      if (p.answerValue === undefined) {
        p.answerValue = [];
      }
      if (!(p.answerValue instanceof Array)) {
        throw new Error('Question answer must be an array');
      }
      this._visualQuestionOptionAnswers[currentOptionIndex] = value;
      p.answerValue = this._visualQuestionOptionAnswers.filter(visualOptionAnswer => visualOptionAnswer !== null);
    });
  }

  public onTrueFalseQuestionOptionSelected(value: StandaloneSurveyQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
  }

  public onSingleChoiceQuestionOptionSelected(value: StandaloneSurveyQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
  }

  public onDropDownListSelected(value: StandaloneSurveyQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
  }

  public onMultipleChoiceQuestionOptionSelected(value: StandaloneSurveyQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      if (p.answerValue === undefined) {
        p.answerValue = [];
      }
      if (!(p.answerValue instanceof Array)) {
        throw new Error('Question answer must be an array');
      }

      if (p.answerValue.indexOf(<never>value) >= 0) {
        Utils.remove(p.answerValue, questionCorrectAnswerItem => questionCorrectAnswerItem === value);
      } else {
        p.answerValue.push(<never>value);
      }
    });
  }

  public getTrueFalseQuestionOptionLabel(optionValue: StandaloneSurveyQuestionModelSingleAnswerValue): string {
    return optionValue ? 'True' : 'False';
  }

  public get isFeedbackShowed(): boolean {
    // if (this.formData.form.type !== this.formType.Quiz || !this.feedbackOnSelectedOption) {
    //   return false;
    // }
    switch (this._question.questionType) {
      case StandaloneSurveyQuestionType.DropDown:
      case StandaloneSurveyQuestionType.TrueFalse:
      case StandaloneSurveyQuestionType.SingleChoice: {
        return true;
      }
      default: {
        return false;
      }
    }
  }

  public get isExplationNoteShowed(): boolean {
    return SurveyQuestionModel.questionTypeToShowExplationNote.includes(this._question.questionType);
  }

  public showAnswerResultToaster(): void {
    this.displayAnswerResultToaster = true;
    setTimeout(() => {
      if (this.displayAnswerResultToaster) {
        this.processScroll();
      }
      this.displayAnswerResultToaster = false;
    }, 3000);
  }

  public processScroll(): void {
    if (this.explantion) {
      this.explantion.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
      return;
    }
    if (this.feedback) {
      this.feedback.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
    }
  }

  public closeAnswerResultToaster(): void {
    this.displayAnswerResultToaster = false;
    this.processScroll();
  }

  public getMediaUrl(url: string): string {
    return url ? `${AppGlobal.environment.cloudfrontUrl}/${url}` : '';
  }

  protected onChanges(changes: SimpleChanges): void {
    if (changes.question && changes.question.currentValue !== changes.question.previousValue) {
      this.renderQuestionOptions = this.getRenderQuestionOptions();
      if (!changes.question.previousValue || this.question.questionTitle !== changes.question.previousValue.questionTitle) {
        this.mainQuizPlayerService.applyToPreparedPopulate(this.question.questionTitle).then(questionTitle =>
          this.mainQuizPlayerService
            .applyPopulatedFields(questionTitle)
            .then(newQuestionTitle => this.mainQuizPlayerService.applyToDisabledPopuplatedFields(newQuestionTitle))
            .then(newTitle => {
              this.safeQuestionTitle = this.sanitizer.bypassSecurityTrustHtml(newTitle);
            })
        );
      }
    }
  }

  private getRenderQuestionOptions(): StandaloneSurveyQuestionOption[] | undefined {
    if (!this.question.questionOptions || !this.optionCodeOrderList) {
      return this.question.questionOptions;
    }
    const questionOptionsDic = Utils.toDictionary(this.question.questionOptions, p => p.code);
    return this.optionCodeOrderList.map(code => questionOptionsDic[code]);
  }
}

function createDebounceFn(func: (...args: unknown[]) => void): (...args: unknown[]) => void {
  return Utils.debounce(func, 300);
}
