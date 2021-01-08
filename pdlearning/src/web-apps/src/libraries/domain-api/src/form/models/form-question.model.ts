import { AnswerFeedbackDisplayOption, FormStatus, FormSurveyType, FormType, IFormModel, SqRatingType } from './form.model';
import { Guid, Utils } from '@opal20/infrastructure';
import { IOptionMedia, MediaType, QuestionOption } from './form-question-option.model';

import { FormSection } from '../../form-section/models/form-section';
import { FormStandaloneMode } from './form-standalone-mode.enum';
import { PublicUserInfo } from '../../share/models/user-info.model';
import { QuestionOptionType } from './question-option-type.model';
import { SafeHtml } from '@angular/platform-browser';
import { ValidationErrors } from '@angular/forms';

export type QuestionAnswerSingleValue = string | number | boolean;

export type QuestionAnswerValue = QuestionAnswerSingleValue | QuestionAnswerSingleValue[];

export type FormQuestionModelSingleAnswerValue = string | boolean | number;

export type FormQuestionModelAnswerValue = FormQuestionModelSingleAnswerValue | FormQuestionModelSingleAnswerValue[];

export enum FormQuestionModelValidationKey {
  questionOptionsAtLeast2 = 'questionOptionsAtLeast2',
  questionOptionsValueNotDuplicated = 'questionOptionsValueNotDuplicated',
  questionMustHaveCorrectAnswer = 'questionMustHaveCorrectAnswer',
  questionHasSomeTextOptions = 'questionHasSomeTextOptions',
  questionHasSomeBlankOptions = 'questionHasSomeTextOptions',
  questionMustHaveTitle = 'questionMustHaveTitle'
}

export enum QuestionType {
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
  Structure = 'Structure',
  FreeResponse = 'FreeResponse',
  Scale = 'Scale',
  Criteria = 'Criteria'
}

export const QUESTION_TYPE_LABEL = new Map<QuestionType, string>([
  [QuestionType.TrueFalse, 'True / False'],
  [QuestionType.SingleChoice, 'Radio Buttons'],
  [QuestionType.MultipleChoice, 'Checkboxes'],
  [QuestionType.ShortText, 'Free Text'],
  [QuestionType.FillInTheBlanks, 'Fill in The Blanks'],
  [QuestionType.DropDown, 'Drop Down'],
  [QuestionType.DatePicker, 'Date picker: one date'],
  [QuestionType.DateRangePicker, 'Date picker: date range'],
  [QuestionType.FreeResponse, 'Free Response']
]);

export interface IFormQuestionModel {
  id: string | undefined;
  formId: string;
  questionType: QuestionType;
  questionTitle: string;
  questionTitleSafeHtml?: SafeHtml;
  questionCorrectAnswer: QuestionAnswerValue | undefined;
  questionOptions: QuestionOption[] | undefined;
  priority: number;
  minorPriority: number;
  questionHint: string | undefined;
  answerExplanatoryNote: string | undefined;
  feedbackCorrectAnswer: string | undefined;
  feedbackWrongAnswer: string | undefined;
  questionLevel: number | undefined;
  randomizedOptions: boolean | undefined;
  score: number | undefined;
  isRequiredAnswer: boolean | undefined;
  parentId: string | undefined;
  createdDate: Date;
  changedDate: Date | undefined;
  isDeleted: boolean | undefined;
  isSurveyTemplateQuestion: boolean | undefined;
  nextQuestionId: string | undefined;
  formSectionId?: string;
  isScoreEnabled?: boolean;
  description?: string;
}

export class FormDataModel implements IFormModel {
  public id: string | undefined;
  public ownerId: string | undefined;
  public title: string = '';
  public type: FormType = FormType.Quiz;
  public surveyType?: FormSurveyType;
  public status: FormStatus = FormStatus.Draft;
  public inSecondTimeLimit: number | undefined;
  public randomizedQuestions: boolean = false;
  public maxAttempt: number | undefined;
  public passingMarkPercentage: number | undefined;
  public passingMarkScore: number | undefined;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public primaryApprovingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public originalObjectId: string;
  public owner: PublicUserInfo;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isSurveyTemplate: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public attemptToShowFeedback: number | undefined;
  public answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;
  public sqRatingType: SqRatingType | undefined;
  public startDate?: Date = new Date();
  public endDate?: Date = new Date();
  public archiveDate?: Date = new Date();
  public archivedBy: string;
  public archivedByUser: PublicUserInfo;
  public isStandalone: boolean = false;
  public standaloneMode: FormStandaloneMode = FormStandaloneMode.Restricted;
  public canUnpublishFormStandalone: boolean;
  public isArchived?: boolean;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;
  public formQuestions: FormQuestionModel[] = [];
  public formSections: FormSection[] = [];

  constructor(data?: FormDataModel) {
    if (data != null) {
      this.formQuestions = data.formQuestions.map(_ => new FormQuestionModel(_));
      this.formSections = data.formSections.map(_ => new FormSection(_));
    }
  }
}

export class FormQuestionModel implements IFormQuestionModel {
  public static questionTypeToShowExplationNote: QuestionType[] = [
    QuestionType.DatePicker,
    QuestionType.DateRangePicker,
    QuestionType.FillInTheBlanks,
    QuestionType.DropDown,
    QuestionType.ShortText,
    QuestionType.SingleChoice,
    QuestionType.TrueFalse,
    QuestionType.MultipleChoice,
    QuestionType.FreeResponse
  ];

  public id: string | undefined;
  public formId: string = '';
  public questionType: QuestionType = QuestionType.ShortText;
  public questionTitle: string = '';
  public questionTitleSafeHtml: SafeHtml;
  public questionCorrectAnswer: QuestionAnswerValue | undefined;
  public questionOptions: QuestionOption[] | undefined;
  public priority: number = 0;
  public minorPriority: number | undefined;
  public questionHint: string | undefined;
  public answerExplanatoryNote: string | undefined;
  public feedbackCorrectAnswer: string | undefined;
  public feedbackWrongAnswer: string | undefined;
  public questionLevel: number | undefined;
  public randomizedOptions: boolean | undefined;
  public isRequiredAnswer: boolean | undefined;
  public score: number | undefined;
  public parentId: string | undefined;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public isDeleted: boolean | undefined;
  public isSurveyTemplateQuestion: boolean | undefined;
  public isScoreEnabled: boolean | undefined;
  public description: string | undefined;
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
  public static removeUnsupportedQuestionTypes(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    return Utils.cloneDeep(
      formQuestions.filter(
        question =>
          question.questionType !== QuestionType.Note &&
          question.questionType !== QuestionType.Section &&
          question.questionType !== QuestionType.Smatrix &&
          question.questionType !== QuestionType.Qset &&
          question.questionType !== QuestionType.Structure
      )
    );
  }

  public static Validate(model: FormQuestionModel, formType: FormType): ValidationErrors | undefined {
    if (formType === FormType.Survey) {
      return undefined;
    }

    switch (model.questionType) {
      // Do not remove these question types, it was used for data migration from OPAL1 to OPAL2 R2.0
      case QuestionType.Qset:
      case QuestionType.Smatrix:
      case QuestionType.Section:
      case QuestionType.Note:
        return undefined;
      case QuestionType.MultipleChoice:
      case QuestionType.SingleChoice:
      case QuestionType.DropDown:
        if (model.questionOptions && model.questionOptions.length < 2) {
          return { [FormQuestionModelValidationKey.questionOptionsAtLeast2]: 'Must have at least 2 options' };
        }
        if (model.questionOptions && Utils.hasDuplicatedItems(model.questionOptions, p => p.value)) {
          return { [FormQuestionModelValidationKey.questionOptionsValueNotDuplicated]: 'All options must be unique' };
        }
        return model.hasCorrectAnswer() || formType !== FormType.Quiz
          ? undefined
          : { [FormQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      case QuestionType.FillInTheBlanks:
        if (!model.questionOptions.some(option => option.type === QuestionOptionType.Text)) {
          return {
            [FormQuestionModelValidationKey.questionHasSomeTextOptions]: 'Question must has at least a text option'
          };
        }
        if (!model.questionOptions.some(option => option.type === QuestionOptionType.Blank)) {
          return {
            [FormQuestionModelValidationKey.questionHasSomeBlankOptions]: 'Question must has at least a blank option'
          };
        }
        return undefined;
      case QuestionType.DateRangePicker:
        return model.questionCorrectAnswer instanceof Array &&
          model.questionCorrectAnswer.length === 2 &&
          !model.questionCorrectAnswer.includes('')
          ? undefined
          : { [FormQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      case QuestionType.LongText:
      case QuestionType.TrueFalse:
      case QuestionType.DatePicker:
        return model.hasCorrectAnswer()
          ? undefined
          : { [FormQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      case QuestionType.ShortText:
        return model.hasCorrectAnswer() || formType === FormType.Poll
          ? undefined
          : { [FormQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      default:
        break;
    }
  }

  public static CreateNewTrueFalseQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.TrueFalse;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.questionOptions = [new QuestionOption(1, true), new QuestionOption(2, false)];
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewSingleChoiceQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.SingleChoice;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.randomizedOptions = false;
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewMultipleChoiceQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.MultipleChoice;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.randomizedOptions = false;
    question.score = score;
    question.questionCorrectAnswer = [];
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewShortTextQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.ShortText;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewFreeResponseQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.FreeResponse;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewLongTextQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.LongText;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewFreeTextQuestion(formId: string, priority: number, minorPriority: number): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.LongText;
    question.questionTitle = '';
    question.createdDate = new Date();
    return question;
  }

  public static CreateNewFillInTheBlanksQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionOptions = [];
    question.questionCorrectAnswer = [];
    question.questionType = QuestionType.FillInTheBlanks;
    question.questionTitle = questionTitle;
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDropDownQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.DropDown;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDatePickerQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.DatePicker;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static CreateNewDateRangePickerQuestion(
    formId: string,
    priority: number,
    minorPriority: number,
    questionTitle: string,
    score: number | undefined
  ): FormQuestionModel {
    const question: FormQuestionModel = new FormQuestionModel();
    question.id = Guid.create().toString();
    question.formId = formId;
    question.priority = priority;
    question.minorPriority = minorPriority;
    question.questionType = QuestionType.DateRangePicker;
    question.questionTitle = questionTitle;
    question.questionOptions = [];
    question.createdDate = new Date();
    question.score = score;
    question.isRequiredAnswer = true;
    return question;
  }

  public static calcMaxScore(questions: FormQuestionModel[]): number {
    let result = 0;
    questions.forEach(p => {
      result += p.score != null ? p.score : 0;
    });
    return result;
  }

  constructor(data?: IFormQuestionModel) {
    if (data != null) {
      this.id = data.id;
      this.formId = data.formId;
      this.questionType = data.questionType;
      this.questionTitle = data.questionTitle;
      this.questionCorrectAnswer = data.questionCorrectAnswer;
      this.questionOptions = data.questionOptions;
      this.priority = data.priority;
      this.minorPriority = data.minorPriority;
      this.questionHint = data.questionHint;
      this.answerExplanatoryNote = data.answerExplanatoryNote;
      this.feedbackCorrectAnswer = data.feedbackCorrectAnswer;
      this.feedbackWrongAnswer = data.feedbackWrongAnswer;
      this.questionLevel = data.questionLevel;
      this.randomizedOptions = data.randomizedOptions;
      this.isRequiredAnswer = data.isRequiredAnswer;
      this.score = data.score;
      this.parentId = data.parentId;
      this.createdDate = data.createdDate;
      this.changedDate = data.changedDate;
      this.isDeleted = data.isDeleted;
      this.isSurveyTemplateQuestion = data.isSurveyTemplateQuestion;
      this.nextQuestionId = data.nextQuestionId;
      this.formSectionId = data.formSectionId;
      this.isScoreEnabled = data.isScoreEnabled;
      this.description = data.description;
    }
  }

  public getBlankOptions(): QuestionOption[] {
    return this.questionOptions.filter(questionOption => questionOption.type === QuestionOptionType.Blank);
  }

  public generateTitle(): void {
    if (!(this.questionType === QuestionType.FillInTheBlanks)) {
      return;
    }
    let questionTitleBuilder: string = '';
    for (const questionOption of this.questionOptions) {
      switch (questionOption.type) {
        case QuestionOptionType.Text: {
          questionTitleBuilder += questionOption.value + ' ';
          break;
        }
        case QuestionOptionType.Blank: {
          questionTitleBuilder += '---' + ' ';
          break;
        }
      }
    }
    this.questionTitle = questionTitleBuilder.trim();
  }

  public markQuestionAsNoRequireAnswer(): FormQuestionModel {
    this.isRequiredAnswer = false;
    return this;
  }

  public enableQuestionScore(): FormQuestionModel {
    this.isScoreEnabled = true;
    return this;
  }

  public updateQuestionOptionValue(value: QuestionAnswerSingleValue, optionIndex: number): void {
    const currentOption = this.questionOptions[optionIndex];
    if (currentOption.type !== QuestionOptionType.Text) {
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

  public addQuestionOptionMediaUrl(media: IOptionMedia, optionIndex: number): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions[optionIndex] = Utils.clone(questionOptions[optionIndex], questionOption => {
        switch (media.type) {
          case MediaType.Video:
            questionOption.videoUrl = media.src;
            questionOption.imageUrl = '';
            break;
          case MediaType.Image:
            questionOption.imageUrl = media.src;
            questionOption.videoUrl = '';
            break;
        }
      });
    });
  }

  public initBasicQuestionData(questionType: QuestionType, priority?: number): FormQuestionModel {
    this.id = Guid.create().toString();
    this.questionType = questionType;
    this.priority = priority || 0;
    this.questionOptions = [];
    return this;
  }

  public isOptionValueCorrect(optionValue: QuestionAnswerSingleValue): boolean {
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
    currentOptionValue: QuestionAnswerSingleValue,
    newOptionValue: QuestionAnswerSingleValue | undefined
  ): void {
    switch (this.questionType) {
      case QuestionType.MultipleChoice:
      case QuestionType.FillInTheBlanks: {
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

  public addNewMediaOption(addNewOptionValue: QuestionAnswerSingleValue, isCorrect: boolean, media: IOptionMedia): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      switch (media.type) {
        case MediaType.Video:
          questionOptions.push(new QuestionOption(questionOptions.length + 1, addNewOptionValue, null, null, null, media.src, true));
          break;
        case MediaType.Image:
          questionOptions.push(new QuestionOption(questionOptions.length + 1, addNewOptionValue, null, null, media.src, null, true));
          break;
      }
    });
    if (isCorrect) {
      this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
    }
  }

  public addNewOption(addNewOptionValue: QuestionAnswerSingleValue, isCorrect: boolean): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(new QuestionOption(questionOptions.length + 1, addNewOptionValue));
    });
    if (isCorrect) {
      this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
    }
  }

  public addTextOption(addNewOptionValue: QuestionAnswerSingleValue): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(new QuestionOption(questionOptions.length + 1, addNewOptionValue, null, QuestionOptionType.Text));
    });
  }

  public addBlankOption(addNewOptionValue: QuestionAnswerSingleValue, optionFeedback: string | null): void {
    this.questionOptions = Utils.clone(this.questionOptions, questionOptions => {
      questionOptions.push(new QuestionOption(questionOptions.length + 1, addNewOptionValue, optionFeedback, QuestionOptionType.Blank));
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
    if (this.questionType === QuestionType.ShortText) {
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

  public isAnswerCorrect(answerValue: FormQuestionModelAnswerValue): boolean {
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
    if (this.questionType === QuestionType.FillInTheBlanks || this.questionType === QuestionType.DateRangePicker) {
      const trimmedCorrectAnswer = (this.questionCorrectAnswer as string[]).map(answer => answer.trim());
      const trimmedAnswer = (answerValue as string[]).map(answer => answer.trim());
      return JSON.stringify(trimmedCorrectAnswer).toLowerCase() === JSON.stringify(trimmedAnswer).toLowerCase();
    }
    if (this.questionType === QuestionType.MultipleChoice) {
      const questionCorrectAnswer = <FormQuestionModelSingleAnswerValue[]>this.questionCorrectAnswer;
      const castedAnswerValue = <FormQuestionModelSingleAnswerValue[]>answerValue;
      return Utils.includesAll(questionCorrectAnswer, castedAnswerValue) && questionCorrectAnswer.length === castedAnswerValue.length;
    }
    return false;
  }

  public generateValueForImageOption(priority: number): string {
    return this.questionType.toString() + priority;
  }
}
