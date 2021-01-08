import { AnswerFeedbackDisplayOption, FormModel, FormStatus, FormType, IFormModel } from './form.model';
import { FormQuestionModel, QuestionType } from './form-question.model';

import { FormSection } from './../../form-section/models/form-section';
import { Guid } from '@opal20/infrastructure';
import { QuestionOption } from './form-question-option.model';
import { QuestionOptionType } from './question-option-type.model';
import { UpdateFormSectionRequest } from './../../form-section/dtos/update-form-section-request';

export interface IQuizExcelTemplate {
  'Form ID'?: number;
  'Form name'?: string;
  'Primary Approving Officer'?: string;
  'Alternate Approving Officer'?: string;
  'Archive date'?: string;
  'Time limit'?: number;
  'Max attempts'?: number;
  'Passing mark percent tage'?: number;
  'Passing mark score'?: number;
  'Display feedback'?: ImportDisplayFeedback;
  'Randomize question'?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export interface ISurveyPollExcelTemplate {
  'Form ID'?: number;
  'Form name'?: string;
  'Primary Approving Officer'?: string;
  'Alternate Approving Officer'?: string;
  'Archive date'?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export interface IQuestionExcelTemplate {
  'Form ID'?: number;
  'Question ID'?: number;
  'Question type'?: ImportQuestionType;
  'Question title': string;
  'Explanation note'?: string;
  Score: number;
  Hint?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export interface IQuestionAnswerExcelTemplate {
  'Question ID'?: number;
  'Question Answer'?: string;
  'Correct Answer'?: string;
  Feedback?: string;
  __rowNum__?: number; // this field use to get row index while reading excel file
}

export enum ImportQuestionType {
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

export enum ImportDisplayFeedback {
  'Immediately after the question is answered' = 'Immediately after the question is answered',
  'After the whole quiz is completed' = 'After the whole quiz is completed',
  'After x attempts on the quiz' = 'After x attempts on the quiz'
}

export const QUESTION_TYPE_DIC = new Map<ImportQuestionType, QuestionType>([
  [ImportQuestionType['Date picker: Date range'], QuestionType.DateRangePicker],
  [ImportQuestionType['Date picker: One date'], QuestionType.DatePicker],
  [ImportQuestionType['Fill in the blanks'], QuestionType.FillInTheBlanks],
  [ImportQuestionType['Check box'], QuestionType.MultipleChoice],
  [ImportQuestionType['Free text'], QuestionType.ShortText],
  [ImportQuestionType['Radio buttons'], QuestionType.SingleChoice],
  [ImportQuestionType['True/False'], QuestionType.TrueFalse],
  [ImportQuestionType.Section, QuestionType.Section],
  [ImportQuestionType.Dropdown, QuestionType.DropDown]
]);

export const DISPLAY_FEEDBACK_TYPE_DIC = new Map<ImportDisplayFeedback, AnswerFeedbackDisplayOption>([
  [ImportDisplayFeedback['After x attempts on the quiz'], AnswerFeedbackDisplayOption.AfterXAtemps],
  [ImportDisplayFeedback['After the whole quiz is completed'], AnswerFeedbackDisplayOption.AfterCompletedQuiz],
  [ImportDisplayFeedback['Immediately after the question is answered'], AnswerFeedbackDisplayOption.AfterAnsweredQuestion]
]);

export interface FormSectionsQuestionModel {
  formQuestions?: FormQuestionModel[];
  formSections?: UpdateFormSectionRequest[];
}

export interface IImportFormModel {
  form: FormModel;
  formQuestionsSections?: FormSectionsQuestionModel;
}

export class ImportFormModel implements IImportFormModel {
  public form: FormModel;
  public formQuestionsSections?: FormSectionsQuestionModel;
}

export class ImportFormParser {
  public static buildQuizModel(data: IQuizExcelTemplate, formType: FormType): FormModel {
    return new FormModel(<IFormModel>{
      title: data['Form name'] ? data['Form name'] : 'Draft',
      randomizedQuestions: data['Randomize question'] === 'Y' ? true : false,
      type: formType,
      status: FormStatus.Draft,
      isShowFreeTextQuestionInPoll: false,
      alternativeApprovingOfficerId: data['Alternate Approving Officer'],
      primaryApprovingOfficerId: data['Primary Approving Officer'],
      answerFeedbackDisplayOption: this.convertToAnswerFeedbackDisplayOption(data['Display feedback']),
      archiveDate: data['Archive date'] ? new Date(data['Archive date']) : new Date(),
      maxAttempt: data['Max attempts'],
      passingMarkPercentage: data['Passing mark percent tage'],
      passingMarkScore: data['Passing mark score'],
      inSecondTimeLimit: data['Time limit'] ? data['Time limit'] * 60 : undefined
    });
  }

  public static buildSurveyPollModel(data: IQuizExcelTemplate, formType: FormType): FormModel {
    return new FormModel(<IFormModel>{
      title: data['Form name'],
      randomizedQuestions: data['Randomize question'] === 'Y' ? true : false,
      type: formType,
      status: FormStatus.Draft,
      alternativeApprovingOfficerId: data['Alternate Approving Officer'],
      primaryApprovingOfficerId: data['Primary Approving Officer'],
      archiveDate: data['Archive date'] ? new Date(data['Archive date']) : new Date(),
      isShowFreeTextQuestionInPoll: false
    });
  }

  public static buildQuestionModel(
    questions: IQuestionExcelTemplate[],
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormSectionsQuestionModel {
    const questionModels: FormQuestionModel[] = [];
    const sectionModels: FormSection[] = [];

    if (!questions) {
      return undefined;
    }

    questions.forEach((question, index) => {
      const questionType = this.convertToQuestionType(question['Question type']);
      let questionModel: FormQuestionModel;
      let sectionModel: FormSection;

      switch (questionType) {
        case QuestionType.TrueFalse:
          questionModel = this.createTrueFalseQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.DropDown:
          questionModel = this.createDropDownQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.FillInTheBlanks:
          questionModel = this.createFillInTheBlankQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.ShortText:
          questionModel = this.createFreeTextQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.MultipleChoice:
          questionModel = this.createMultipleChoiceQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.DateRangePicker:
          questionModel = this.createDateRangePickerQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.DatePicker:
          questionModel = this.createDatePickerQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.SingleChoice:
          questionModel = this.createSingleChoiceQuestion(index, question, questionAnswers, formType);
          questionModels.push(questionModel);
          break;
        case QuestionType.Section:
          sectionModel = this.buildSectionModel(index, question, questionAnswers);
          sectionModels.push(sectionModel);
          break;
      }
    });

    return <FormSectionsQuestionModel>{
      formQuestions: questionModels,
      formSections: sectionModels.length !== 0 ? sectionModels : []
    };
  }

  public static buildSectionModel(
    priority: number,
    section: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[]
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
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionAnswer = questionAnswers.find(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.TrueFalse;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.questionOptions = [new QuestionOption(1, true), new QuestionOption(2, false)];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (formType === FormType.Quiz && questionAnswer) {
      questionModel.questionCorrectAnswer = questionAnswer['Question Answer'].startsWith('[True]') ? true : false;
    }

    return questionModel;
  }

  public static createSingleChoiceQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.SingleChoice;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (questionOptions) {
      const options: QuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new QuestionOption(code + 1, option['Question Answer'], option.Feedback));
      });

      questionModel.questionOptions = options;
    }

    if (formType === FormType.Quiz && questionOptions) {
      const correctAnswer = questionOptions.find(q => q['Correct Answer'] === 'X');
      questionModel.questionCorrectAnswer = correctAnswer ? correctAnswer['Question Answer'] : [];
    }

    return questionModel;
  }

  public static createMultipleChoiceQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.MultipleChoice;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (questionOptions) {
      const options: QuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new QuestionOption(code + 1, option['Question Answer'], option.Feedback));
      });

      questionModel.questionOptions = options;
    }

    if (formType === FormType.Quiz && questionOptions) {
      const correctAnswer = questionOptions.filter(q => q['Correct Answer'] === 'X').map(x => x['Question Answer']);
      questionModel.questionCorrectAnswer = correctAnswer ? correctAnswer : [];
    }

    return questionModel;
  }

  public static createFreeTextQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionAnswer = questionAnswers.find(q => q['Question ID'] === question['Question ID']);
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.ShortText;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (formType === FormType.Quiz && questionAnswer) {
      questionModel.questionCorrectAnswer = questionAnswer['Question Answer'];
    }

    return questionModel;
  }

  public static createFillInTheBlankQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.FillInTheBlanks;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (questionOptions) {
      const options: QuestionOption[] = [];
      const correctAnswers: string[] = [];
      questionOptions.forEach((option, code) => {
        if (option['Question Answer'].startsWith('[Blank]')) {
          const blankText = option['Question Answer'].replace('[Blank]', '');
          correctAnswers.push(blankText);
          options.push(new QuestionOption(code + 1, blankText, option.Feedback, QuestionOptionType.Blank));
        }

        if (option['Question Answer'].startsWith('[Text]')) {
          const text = option['Question Answer'].replace('[Text]', '');
          options.push(new QuestionOption(code + 1, text, option.Feedback, QuestionOptionType.Text));
        }
      });

      questionModel.questionOptions = options;
      questionModel.questionCorrectAnswer = correctAnswers ? correctAnswers : [];
    }

    return questionModel;
  }

  public static createDropDownQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionOptions = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);

    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.DropDown;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (questionOptions) {
      const options: QuestionOption[] = [];
      questionOptions.forEach((option, code) => {
        options.push(new QuestionOption(code + 1, option['Question Answer'], option.Feedback));
      });

      questionModel.questionOptions = options;
    }

    if (formType === FormType.Quiz && questionOptions) {
      const correctAnswer = questionOptions.find(q => q['Correct Answer'] === 'X');
      questionModel.questionCorrectAnswer = correctAnswer ? correctAnswer['Question Answer'] : [];
    }

    return questionModel;
  }

  public static createDateRangePickerQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const answers = questionAnswers.filter(q => q['Question ID'] === question['Question ID']);
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.DateRangePicker;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (formType === FormType.Quiz && answers) {
      questionModel.questionCorrectAnswer = [];
      const startDate = answers.find(x => x['Question Answer'].startsWith('[Start date]'));
      const endDate = answers.find(x => x['Question Answer'].startsWith('[End date]'));
      const startDateOption = startDate ? new Date(startDate['Question Answer'].replace('[Start date]', '').trim()).toDateString() : null;
      const endDateOption = endDate ? new Date(endDate['Question Answer'].replace('[End date]', '').trim()).toDateString() : null;

      questionModel.questionCorrectAnswer.push(startDateOption, endDateOption);
    }

    return questionModel;
  }

  public static createDatePickerQuestion(
    priority: number,
    question: IQuestionExcelTemplate,
    questionAnswers: IQuestionAnswerExcelTemplate[],
    formType: FormType
  ): FormQuestionModel {
    const questionModel: FormQuestionModel = new FormQuestionModel();
    const questionAnswer = questionAnswers.find(q => q['Question ID'] === question['Question ID']);
    questionModel.formId = Guid.create().toString();
    questionModel.id = Guid.create().toString();
    questionModel.score = question.Score ? question.Score : 1;
    questionModel.priority = priority;
    questionModel.questionType = QuestionType.DatePicker;
    questionModel.questionHint = question.Hint;
    questionModel.createdDate = new Date();
    questionModel.questionTitle = question['Question title'];
    questionModel.isRequiredAnswer = true;
    questionModel.answerExplanatoryNote = question['Explanation note'];

    if (formType === FormType.Quiz && questionAnswer) {
      questionModel.questionCorrectAnswer = new Date(questionAnswer['Question Answer'].trim()).toDateString();
    }

    return questionModel;
  }

  public static convertToAnswerFeedbackDisplayOption(data: ImportDisplayFeedback): AnswerFeedbackDisplayOption {
    return DISPLAY_FEEDBACK_TYPE_DIC.get(data);
  }

  public static convertToQuestionType(data: ImportQuestionType): QuestionType {
    return QUESTION_TYPE_DIC.get(data);
  }
}
