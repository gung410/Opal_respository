import { Guid, Utils } from '@opal20/infrastructure';
import { IStandaloneSurveyModel, StandaloneSurveySqRatingType, SurveyStatus } from './lna-form.model';
import { IStandaloneSurveyOptionMedia, StandaloneSurveyMediaType, StandaloneSurveyQuestionOption } from './form-question-option.model';

import { PublicUserInfo } from './../../share/models/user-info.model';
import { SafeHtml } from '@angular/platform-browser';
import { StandaloneSurveyQuestionOptionType } from './question-option-type.model';
import { SurveyParticipantMode } from './form-standalone-mode.enum';
import { SurveySection } from './form-section';
import { ValidationErrors } from '@angular/forms';

export type StandaloneSurveyQuestionAnswerSingleValue = string | number | boolean;

export type StandaloneSurveyQuestionAnswerValue = StandaloneSurveyQuestionAnswerSingleValue | StandaloneSurveyQuestionAnswerSingleValue[];

export type StandaloneSurveyQuestionModelSingleAnswerValue = string | boolean | number;

export type StandaloneSurveyQuestionModelAnswerValue =
  | StandaloneSurveyQuestionModelSingleAnswerValue
  | StandaloneSurveyQuestionModelSingleAnswerValue[];

export enum StandaloneSurveyQuestionModelValidationKey {
  questionOptionsAtLeast2 = 'questionOptionsAtLeast2',
  questionOptionsValueNotDuplicated = 'questionOptionsValueNotDuplicated',
  questionMustHaveCorrectAnswer = 'questionMustHaveCorrectAnswer',
  questionHasSomeTextOptions = 'questionHasSomeTextOptions',
  questionHasSomeBlankOptions = 'questionHasSomeTextOptions',
  questionMustHaveTitle = 'questionMustHaveTitle'
}

export enum StandaloneSurveyQuestionType {
  TrueFalse = 'TrueFalse',
  SingleChoice = 'SingleChoice',
  MultipleChoice = 'MultipleChoice',
  ShortText = 'ShortText',
  LongText = 'LongText',
  FillInTheBlanks = 'FillInTheBlanks',
  DropDown = 'DropDown',
  Section = 'Section',
  Smatrix = 'Smatrix',
  Note = 'Note',
  Qset = 'Qset',
  DatePicker = 'DatePicker',
  DateRangePicker = 'DateRangePicker',
  Structure = 'Structure'
}

export interface ISurveyQuestionModel {
  id: string | undefined;
  formId: string;
  questionType: StandaloneSurveyQuestionType;
  questionTitle: string;
  questionTitleSafeHtml?: SafeHtml;
  questionCorrectAnswer: StandaloneSurveyQuestionAnswerValue | undefined;
  questionOptions: StandaloneSurveyQuestionOption[] | undefined;
  priority: number;
  minorPriority: number;
  isRequiredAnswer: boolean | undefined;
  parentId: string | undefined;
  createdDate: Date;
  changedDate: Date | undefined;
  isDeleted: boolean | undefined;
  nextQuestionId: string | undefined;
  formSectionId?: string;
}

export class StandaloneSurveyDataModel implements IStandaloneSurveyModel {
  public id: string | undefined;
  public ownerId: string | undefined;
  public title: string = '';
  public status: SurveyStatus = SurveyStatus.Draft;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public originalObjectId: string;
  public owner: PublicUserInfo;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public sqRatingType: StandaloneSurveySqRatingType | undefined;
  public startDate?: Date = new Date();
  public endDate?: Date = new Date();
  public archiveDate?: Date = new Date();
  public archivedBy: string;
  public archivedByUser: PublicUserInfo;
  public isStandalone: boolean = false;
  public standaloneMode?: SurveyParticipantMode = SurveyParticipantMode.Restricted;
  public canUnpublishFormStandalone: boolean;
  public isArchived?: boolean;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;
  public formQuestions: SurveyQuestionModel[] = [];
  public formSections: SurveySection[] = [];

  constructor(data?: StandaloneSurveyDataModel) {
    if (data != null) {
      this.formQuestions = data.formQuestions.map(_ => new SurveyQuestionModel(_));
      this.formSections = data.formSections.map(_ => new SurveySection(_));
    }
  }
}

export class SurveyQuestionModel implements ISurveyQuestionModel {
  public static questionTypeToShowExplationNote: StandaloneSurveyQuestionType[] = [
    StandaloneSurveyQuestionType.DatePicker,
    StandaloneSurveyQuestionType.DateRangePicker,
    StandaloneSurveyQuestionType.FillInTheBlanks,
    StandaloneSurveyQuestionType.DropDown,
    StandaloneSurveyQuestionType.ShortText,
    StandaloneSurveyQuestionType.SingleChoice,
    StandaloneSurveyQuestionType.TrueFalse,
    StandaloneSurveyQuestionType.MultipleChoice
  ];

  public id: string | undefined;
  public formId: string = '';
  public questionType: StandaloneSurveyQuestionType = StandaloneSurveyQuestionType.ShortText;
  public questionTitle: string = '';
  public questionTitleSafeHtml: SafeHtml;
  public questionCorrectAnswer: StandaloneSurveyQuestionAnswerValue | undefined;
  public questionOptions: StandaloneSurveyQuestionOption[] | undefined;
  public priority: number = 0;
  public minorPriority: number | undefined;
  public isRequiredAnswer: boolean | undefined;
  public parentId: string | undefined;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public isDeleted: boolean | undefined;
  /**
   * The next question id for the question.
   */
  public nextQuestionId: string | undefined;
  public formSectionId?: string;

  public get combinePriorityTitle(): string {
    return `${this.priority} ${this.questionTitle}`.trim();
  }

  /**
   * Section, Smatrix, Qset and Note are not supported in R2 but the migrated data should include these questions on UI and ONLY for that
   * Because of that, these question types should not impact anything on the existing features!
   */
  public static removeUnsupportedQuestionTypes(formQuestions: SurveyQuestionModel[]): SurveyQuestionModel[] {
    return Utils.cloneDeep(
      formQuestions.filter(
        question =>
          question.questionType !== StandaloneSurveyQuestionType.Note &&
          question.questionType !== StandaloneSurveyQuestionType.Section &&
          question.questionType !== StandaloneSurveyQuestionType.Smatrix &&
          question.questionType !== StandaloneSurveyQuestionType.Qset &&
          question.questionType !== StandaloneSurveyQuestionType.Structure
      )
    );
  }

  public static Validate(model: SurveyQuestionModel): ValidationErrors | undefined {
    // if (formType === FormType.Survey) {
    return undefined;
    // }
  }

  public static CreateNewTrueFalseQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.TrueFalse;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.questionOptions = [new StandaloneSurveyQuestionOption(1, true), new StandaloneSurveyQuestionOption(2, false)];
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewSingleChoiceQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.SingleChoice;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewMultipleChoiceQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.MultipleChoice;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.questionCorrectAnswer = [];
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewShortTextQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.ShortText;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewLongTextQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.LongText;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewFreeTextQuestion(formId: string, priority: number, minorPriority: number): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.LongText;
    question.questionTitle = '';
    question.createdDate = new Date();
    return question;
  }

  public static CreateNewFillInTheBlanksQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionOptions = [];
    question.questionCorrectAnswer = [];
    question.questionType = StandaloneSurveyQuestionType.FillInTheBlanks;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDropDownQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.DropDown;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDatePickerQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.DatePicker;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDateRangePickerQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string
  ): SurveyQuestionModel {
    const question: SurveyQuestionModel = new SurveyQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = StandaloneSurveyQuestionType.DateRangePicker;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.isRequiredAnswer = true;
    return question;
  }

  constructor(data?: ISurveyQuestionModel) {
    if (data != null) {
      this.id = data.id;
      this.formId = data.formId;
      this.questionType = data.questionType;
      this.questionTitle = data.questionTitle;
      this.questionCorrectAnswer = data.questionCorrectAnswer;
      this.questionOptions = data.questionOptions;
      this.priority = data.priority;
      this.minorPriority = data.minorPriority;
      this.isRequiredAnswer = data.isRequiredAnswer;
      this.parentId = data.parentId;
      this.createdDate = data.createdDate;
      this.changedDate = data.changedDate;
      this.isDeleted = data.isDeleted;
      this.nextQuestionId = data.nextQuestionId;
      this.formSectionId = data.formSectionId;
    }
  }

  public getBlankOptions(): StandaloneSurveyQuestionOption[] {
    return this.questionOptions.filter(questionOption => questionOption.type === StandaloneSurveyQuestionOptionType.Blank);
  }

  public generateTitle(): void {
    if (!(this.questionType === StandaloneSurveyQuestionType.FillInTheBlanks)) {
      return;
    }
    let questionTitleBuilder: string = '';
    for (const questionOption of this.questionOptions) {
      switch (questionOption.type) {
        case StandaloneSurveyQuestionOptionType.Text: {
          questionTitleBuilder += questionOption.value + ' ';
          break;
        }
        case StandaloneSurveyQuestionOptionType.Blank: {
          questionTitleBuilder += '---' + ' ';
          break;
        }
      }
    }
    this.questionTitle = questionTitleBuilder.trim();
  }

  public markQuestionAsNoRequireAnswer(): SurveyQuestionModel {
    this.isRequiredAnswer = false;
    return this;
  }

  public updateQuestionOptionValue(value: StandaloneSurveyQuestionAnswerSingleValue, optionIndex: number): void {
    const currentOption = this.questionOptions[optionIndex];
    if (currentOption.type !== StandaloneSurveyQuestionOptionType.Text) {
      const currentOptionValue = currentOption.value;
      const generatedValue = this.generateValueForImageOption(optionIndex + 1);
      const val = currentOptionValue || generatedValue;
      const isCurrentOptionValueCorrect = this.isOptionValueCorrect(val);
      if (isCurrentOptionValueCorrect) {
        if (
          !value &&
          this.questionCorrectAnswer instanceof Array &&
          this.questionCorrectAnswer.indexOf(generatedValue) < 0 &&
          (currentOption.imageUrl || currentOption.videoUrl)
        ) {
          this.updateQuestionCorrectAnswer(currentOptionValue, generatedValue);
        } else {
          this.updateQuestionCorrectAnswer(currentOptionValue, value);
        }
      }
    }
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions[optionIndex] = Utils.clone(questionOptions[optionIndex], questionOption => {
        questionOption.value = value;
      });
    });
  }

  public addQuestionOptionMediaUrl(media: IStandaloneSurveyOptionMedia, optionIndex: number): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions[optionIndex] = Utils.clone(questionOptions[optionIndex], questionOption => {
        switch (media.type) {
          case StandaloneSurveyMediaType.Video:
            questionOption.videoUrl = media.src;
            questionOption.imageUrl = '';
            break;
          case StandaloneSurveyMediaType.Image:
            questionOption.imageUrl = media.src;
            questionOption.videoUrl = '';
            break;
        }
      });
    });
  }

  public isOptionValueCorrect(optionValue: StandaloneSurveyQuestionAnswerSingleValue): boolean {
    if (Utils.isNullOrUndefined(this.questionCorrectAnswer)) {
      return false;
    }
    if (this.questionCorrectAnswer === optionValue) {
      return true;
    }
    if (this.questionCorrectAnswer instanceof Array && this.questionCorrectAnswer.indexOf(optionValue) >= 0) {
      return true;
    }

    return false;
  }

  public updateQuestionCorrectAnswer(
    currentOptionValue: StandaloneSurveyQuestionAnswerSingleValue,
    newOptionValue: StandaloneSurveyQuestionAnswerSingleValue | undefined
  ): void {
    switch (this.questionType) {
      case StandaloneSurveyQuestionType.MultipleChoice:
      case StandaloneSurveyQuestionType.FillInTheBlanks: {
        if (!(this.questionCorrectAnswer instanceof Array)) {
          throw new Error('Question Correct answer must be an array');
        }
        const currentOptionValueIndex = this.questionCorrectAnswer.indexOf(currentOptionValue);
        if (currentOptionValueIndex < 0) {
          this.questionCorrectAnswer = Utils.clone(this.questionCorrectAnswer, p => {
            p.push(newOptionValue);
          });
          break;
        }
        this.questionCorrectAnswer = Utils.clone(this.questionCorrectAnswer, p => {
          if (newOptionValue) {
            p[currentOptionValueIndex] = newOptionValue;
          } else {
            p.splice(currentOptionValueIndex, 1);
          }
        });
        break;
      }
      default: {
        this.questionCorrectAnswer = newOptionValue;
        break;
      }
    }
  }

  public addNewMediaOption(
    addNewOptionValue: StandaloneSurveyQuestionAnswerSingleValue,
    isCorrect: boolean,
    media: IStandaloneSurveyOptionMedia
  ): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      switch (media.type) {
        case StandaloneSurveyMediaType.Video:
          questionOptions.push(
            new StandaloneSurveyQuestionOption(questionOptions.length + 1, addNewOptionValue, null, null, media.src, true)
          );
          break;
        case StandaloneSurveyMediaType.Image:
          questionOptions.push(
            new StandaloneSurveyQuestionOption(questionOptions.length + 1, addNewOptionValue, null, media.src, null, true)
          );
          break;
      }
    });
    if (isCorrect) {
      this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
    }
  }

  public addNewOption(addNewOptionValue: StandaloneSurveyQuestionAnswerSingleValue, isCorrect: boolean): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(new StandaloneSurveyQuestionOption(questionOptions.length + 1, addNewOptionValue));
    });
    if (isCorrect) {
      this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
    }
  }

  public addTextOption(addNewOptionValue: StandaloneSurveyQuestionAnswerSingleValue): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(
        new StandaloneSurveyQuestionOption(questionOptions.length + 1, addNewOptionValue, null, StandaloneSurveyQuestionOptionType.Text)
      );
    });
  }

  public addBlankOption(addNewOptionValue: StandaloneSurveyQuestionAnswerSingleValue, optionFeedback: string | null): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(
        new StandaloneSurveyQuestionOption(questionOptions.length + 1, addNewOptionValue, StandaloneSurveyQuestionOptionType.Blank)
      );
    });
    this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
  }

  public removeOption(optionIndex: number): void {
    const currentOptionValue = this.questionOptions[optionIndex].value;
    const isCurrentOptionValueCorrect = this.isOptionValueCorrect(currentOptionValue);
    if (isCurrentOptionValueCorrect) {
      this.updateQuestionCorrectAnswer(currentOptionValue, undefined);
    }
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.splice(optionIndex, 1);
      questionOptions.forEach((p, i) => {
        questionOptions[i] = Utils.clone(questionOptions[i], questionOption => {
          questionOption.code = i;
        });
      });
    });
  }

  public hasCorrectAnswer(): boolean {
    if (this.questionType === StandaloneSurveyQuestionType.ShortText) {
      return !Utils.isNullOrEmpty(this.questionCorrectAnswer);
    }
    return (
      !Utils.isNullOrEmpty(this.questionCorrectAnswer) &&
      (!(this.questionCorrectAnswer instanceof Array) || this.questionCorrectAnswer.length > 0)
    );
  }

  public getCorrectAnswerLenght(): number {
    return (this.questionCorrectAnswer as string[]).length;
  }

  public isAnswerCorrect(answerValue: StandaloneSurveyQuestionModelAnswerValue): boolean {
    if (Utils.isNullOrUndefined(this.questionCorrectAnswer) || Utils.isNullOrUndefined(answerValue)) {
      return false;
    }
    if (this.questionCorrectAnswer === answerValue) {
      return true;
    }
    if (
      (typeof this.questionCorrectAnswer && typeof answerValue) === 'string' &&
      this.questionCorrectAnswer
        .toString()
        .trim()
        .toLowerCase() ===
        answerValue
          .toString()
          .trim()
          .toLowerCase()
    ) {
      return true;
    }
    if (
      this.questionType === StandaloneSurveyQuestionType.FillInTheBlanks ||
      this.questionType === StandaloneSurveyQuestionType.DateRangePicker
    ) {
      const trimmedCorrectAnswer = (this.questionCorrectAnswer as string[]).map(answer => answer.trim());
      const trimmedAnswer = (answerValue as string[]).map(answer => answer.trim());
      return JSON.stringify(trimmedCorrectAnswer).toLowerCase() === JSON.stringify(trimmedAnswer).toLowerCase();
    }
    if (this.questionType === StandaloneSurveyQuestionType.MultipleChoice) {
      const questionCorrectAnswer = <StandaloneSurveyQuestionModelSingleAnswerValue[]>this.questionCorrectAnswer;
      const castedAnswerValue = <StandaloneSurveyQuestionModelSingleAnswerValue[]>answerValue;
      return Utils.includesAll(questionCorrectAnswer, castedAnswerValue) && questionCorrectAnswer.length === castedAnswerValue.length;
    }
    return false;
  }

  public generateValueForImageOption(priority: number): string {
    return this.questionType.toString() + priority;
  }
}
