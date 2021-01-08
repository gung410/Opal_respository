import { IStandaloneSurveyModel, StandaloneSurveyModel, SurveyStatus } from './lna-form.model';
import { StandaloneSurveyQuestionType, SurveyQuestionModel } from './form-question.model';

import { FormSection } from './../../form-section/models/form-section';
import { UpdateFormSectionRequest } from './../../form-section/dtos/update-form-section-request';
import { Guid } from '@opal20/infrastructure';
import { StandaloneSurveyQuestionOption } from './form-question-option.model';
import { StandaloneSurveyQuestionOptionType } from './question-option-type.model';

export interface IStandaloneSurveyPollExcelTemplate {
  'Form ID'?: number;
  'Form name'?: string;
  'Archive date'?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export interface IStandaloneSurveyQuestionExcelTemplate {
  'Form ID'?: number;
  'Question ID'?: number;
  'Question type'?: StandaloneSurveyImportQuestionType;
  'Question title': string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export interface IStandaloneSurveyQuestionAnswerExcelTemplate {
  'Question ID'?: number;
  'Question Answer'?: string;
  'Correct Answer'?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export enum StandaloneSurveyImportQuestionType {
  'True/False' = 'True/False',
  'Radio buttons' = 'Radio buttons',
  'Check box' = 'Check box',
  'Free text' = 'Free text',
  'Fill in the blanks' = 'Fill in the blanks',
  'Dropdown' = 'Dropdown',
  'Section' = 'Section',
  'Date picker: One date' = 'Date picker: One date',
  'Date picker: Date range' = 'Date picker: Date range'
}

export const STANDALONE_SURVEY_QUESTION_TYPE_DIC = new Map<StandaloneSurveyImportQuestionType, StandaloneSurveyQuestionType>([
  [StandaloneSurveyImportQuestionType['Date picker: Date range'], StandaloneSurveyQuestionType.DateRangePicker],
  [StandaloneSurveyImportQuestionType['Date picker: One date'], StandaloneSurveyQuestionType.DatePicker],
  [StandaloneSurveyImportQuestionType['Fill in the blanks'], StandaloneSurveyQuestionType.FillInTheBlanks],
  [StandaloneSurveyImportQuestionType['Check box'], StandaloneSurveyQuestionType.MultipleChoice],
  [StandaloneSurveyImportQuestionType['Free text'], StandaloneSurveyQuestionType.ShortText],
  [StandaloneSurveyImportQuestionType['Radio buttons'], StandaloneSurveyQuestionType.SingleChoice],
  [StandaloneSurveyImportQuestionType['True/False'], StandaloneSurveyQuestionType.TrueFalse],
  [StandaloneSurveyImportQuestionType.Section, StandaloneSurveyQuestionType.Section],
  [StandaloneSurveyImportQuestionType.Dropdown, StandaloneSurveyQuestionType.DropDown]
]);

export interface StandaloneSurveySectionsQuestionModel {
  formQuestions?: SurveyQuestionModel[];
  formSections?: UpdateFormSectionRequest[];
}

export interface IImportStandaloneSurveyModel {
  form: StandaloneSurveyModel;
  formQuestionsSections?: StandaloneSurveySectionsQuestionModel;
}

export class ImportStandaloneSurveyModel implements IImportStandaloneSurveyModel {
  public form: StandaloneSurveyModel;
  public formQuestionsSections?: StandaloneSurveySectionsQuestionModel;
}

export class StandaloneSurveyImportFormParser {
  public static buildSurveyPollModel(data: IStandaloneSurveyPollExcelTemplate): StandaloneSurveyModel {
    return new StandaloneSurveyModel(<IStandaloneSurveyModel>{
      title: data['Form name'],
      status: SurveyStatus.Draft,
      archiveDate: data['Archive date'] ? new Date(data['Archive date']) : new Date(),
      isShowFreeTextQuestionInPoll: false
    });
  }

  public static buildQuestionModel(
    questions: IStandaloneSurveyQuestionExcelTemplate[],
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): StandaloneSurveySectionsQuestionModel {
    const questionModels: SurveyQuestionModel[] = [];
    const sectionModels: FormSection[] = [];

    if (!questions) {
      return undefined;
    }

    questions.forEach((question, index) => {
      const questionType = this.convertToQuestionType(question['Question type']);
      let questionModel: SurveyQuestionModel;
      let sectionModel: FormSection;

      switch (questionType) {
        case StandaloneSurveyQuestionType.TrueFalse:
          questionModel = this.createTrueFalseQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.DropDown:
          questionModel = this.createDropDownQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.FillInTheBlanks:
          questionModel = this.createFillInTheBlankQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.ShortText:
          questionModel = this.createFreeTextQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.MultipleChoice:
          questionModel = this.createMultipleChoiceQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.DateRangePicker:
          questionModel = this.createDateRangePickerQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.DatePicker:
          questionModel = this.createDatePickerQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.SingleChoice:
          questionModel = this.createSingleChoiceQuestion(index, question, questionAnswers);
          questionModels.push(questionModel);
          break;
        case StandaloneSurveyQuestionType.Section:
          sectionModel = this.buildSectionModel(index, question, questionAnswers);
          sectionModels.push(sectionModel);
          break;
      }
    });

    return <StandaloneSurveySectionsQuestionModel>{
      formQuestions: questionModels,
      formSections: sectionModels.length !== 0 ? sectionModels : []
    };
  }

  public static buildSectionModel(
    priority: number,
    section: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): UpdateFormSectionRequest {
    const secionDetail = questionAnswers.find(x => x['Question ID'] === section['Question ID']);

    const newSection: UpdateFormSectionRequest = {
      id: Guid.create().toString(),
      formId: Guid.create().toString(),
      priority: priority,
      isDeleted: false,
      additionalDescription: secionDetail ? secionDetail['Question Answer'] : 'Description',
      mainDescription: section['Question title'] ? section['Question title'] : 'Section'
    };

    return newSection;
  }

  public static createTrueFalseQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    const questionAnswer = questionAnswers.find(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.TrueFalse;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.questionOptions = [new StandaloneSurveyQuestionOption(1, true), new StandaloneSurveyQuestionOption(2, false)];
    questionModel.isRequiredAnswer = true;

    return questionModel;
  }

  public static createSingleChoiceQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.SingleChoice;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    if (questionOptions) {
      const options: StandaloneSurveyQuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new StandaloneSurveyQuestionOption(code + 1, option['Question Answer']));
      });

      questionModel.questionOptions = options;
    }

    return questionModel;
  }

  public static createMultipleChoiceQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.MultipleChoice;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    if (questionOptions) {
      const options: StandaloneSurveyQuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new StandaloneSurveyQuestionOption(code + 1, option['Question Answer']));
      });

      questionModel.questionOptions = options;
    }

    return questionModel;
  }

  public static createFreeTextQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.ShortText;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    return questionModel;
  }

  public static createFillInTheBlankQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.FillInTheBlanks;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    if (questionOptions) {
      const options: StandaloneSurveyQuestionOption[] = [];
      const correctAnswers: string[] = [];
      questionOptions.forEach((option, code) => {
        if (option['Question Answer'].startsWith('[Blank]')) {
          const blankText = option['Question Answer'].replace('[Blank]', '');
          correctAnswers.push(blankText);
          options.push(new StandaloneSurveyQuestionOption(code + 1, blankText, StandaloneSurveyQuestionOptionType.Blank));
        }

        if (option['Question Answer'].startsWith('[Text]')) {
          const text = option['Question Answer'].replace('[Text]', '');
          options.push(new StandaloneSurveyQuestionOption(code + 1, text, StandaloneSurveyQuestionOptionType.Text));
        }
      });

      questionModel.questionOptions = options;
      questionModel.questionCorrectAnswer = correctAnswers ? correctAnswers : [];
    }

    return questionModel;
  }

  public static createDropDownQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.DropDown;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    if (questionOptions) {
      const options: StandaloneSurveyQuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new StandaloneSurveyQuestionOption(code + 1, option['Question Answer']));
      });

      questionModel.questionOptions = options;
    }

    return questionModel;
  }

  public static createDateRangePickerQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.DateRangePicker;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    return questionModel;
  }

  public static createDatePickerQuestion(
    priority: number,
    question: IStandaloneSurveyQuestionExcelTemplate,
    questionAnswers: IStandaloneSurveyQuestionAnswerExcelTemplate[]
  ): SurveyQuestionModel {
    const questionModel: SurveyQuestionModel = new SurveyQuestionModel();
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.priority = priority;
    questionModel.questionType = StandaloneSurveyQuestionType.DatePicker;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;

    return questionModel;
  }

  public static convertToQuestionType(data: StandaloneSurveyImportQuestionType): StandaloneSurveyQuestionType {
    return STANDALONE_SURVEY_QUESTION_TYPE_DIC.get(data);
  }
}
