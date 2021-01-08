import { BaseComponent, ModuleFacadeService, NotificationType } from '@opal20/infrastructure';
import { Component, ElementRef, Inject, Input, TemplateRef, ViewChild } from '@angular/core';
import { ExcelError, ExcelReader, ExcelReaderException, FileUploaderUtils } from '@opal20/common-components';
import {
  FormApiService,
  FormType,
  IQuestionAnswerExcelTemplate,
  IQuestionExcelTemplate,
  IQuizExcelTemplate,
  ISurveyPollExcelTemplate,
  ImportDisplayFeedback,
  ImportFormModel,
  ImportFormParser,
  ImportQuestionType,
  UserRepository
} from '@opal20/domain-api';

import { APP_BASE_HREF } from '@angular/common';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'import-form-dialog',
  templateUrl: './import-form-dialog.component.html'
})
export class ImportFormDialogComponent extends BaseComponent {
  @Input() public formType: FormType;
  public importFile: File | undefined;
  public importDataValid: boolean = false;
  public importPercentage: number = 0;
  public importProgressTitle: string;
  public importParameter: ImportFormParameters | undefined;
  public importType: FormType[] = [FormType.Quiz, FormType.Survey, FormType.Poll];

  public errors: ExcelError[] = [];
  public errorDialogRef: DialogRef;
  @ViewChild('errorResultDialog', { static: false })
  public errorResultDialog: TemplateRef<unknown>;

  @ViewChild('fileSelectEl', { static: false })
  public inputFileEl: ElementRef;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public userRepository: UserRepository,
    public dialogRef: DialogRef,
    private formApiService: FormApiService,
    @Inject(APP_BASE_HREF) private baseHref: string
  ) {
    super(moduleFacadeService);
  }

  public onFileSelect(files: FileList): Promise<void> {
    if (!files[0] || files.length !== 1) {
      return;
    }

    const importParameter = new ImportFormParameters(files[0]);
    Promise.resolve(importParameter)
      .then(() => this.validateImportFile(importParameter))
      .then(() => this.startImportTransaction(importParameter))
      .then(() => this.readFormSheet(importParameter))
      .then(() => this.readQuestionSheet(importParameter))
      .then(() => this.readQuestionAnswerSheet(importParameter))
      .then(() => this.additionalValidateForQuiz(importParameter))
      .then(() => this.completeImportTransaction(importParameter))
      .catch(err => {
        this.moduleFacadeService.modalService.showErrorMessage(err);
        this.resetImportProcess();
      });
  }

  public validateImportFile(parameter: ImportFormParameters): Promise<void> {
    return new Promise((resolve, rejected) => {
      if (!parameter.file.name.includes('.xlsx')) {
        rejected('The file extension must be xlsx');
      }

      if (FileUploaderUtils.exceedLimitFileSize(parameter.file, 10)) {
        rejected('File size exceeded 10MB');
      }

      resolve();
    });
  }

  public startImportTransaction(parameter: ImportFormParameters): Promise<void> {
    return new Promise(resolve => {
      this.importFile = parameter.file;
      this.importProgressTitle = 'Start validate data';
      resolve();
    });
  }
  /*
   *
   * 'Quiz', 'Survey', 'Poll' Spreadsheet region
   *
   */
  public readFormSheet(parameter: ImportFormParameters): Promise<void> {
    switch (this.formType) {
      case FormType.Quiz:
        return this.readQuizSheet(parameter);
      case FormType.Survey:
      case FormType.Poll:
        return this.readPollOrSurveySheet(parameter);
      default:
        return;
    }
  }

  public readQuizSheet(parameter: ImportFormParameters): Promise<void> {
    return Promise.resolve()
      .then(() => ExcelReader.ParseSheetDataToObject<IQuizExcelTemplate>(parameter.file, 'Quiz'))
      .then(result => (parameter.quizTemplates = result))
      .then(() => this.checkQuizSheetValidFormat(parameter, 'Quiz'))
      .then(() => this.checkQuizUserEmailDataCorrect(parameter))
      .catch(err => Promise.reject(err));
  }

  public readPollOrSurveySheet(parameter: ImportFormParameters): Promise<void> {
    return Promise.resolve()
      .then(() => ExcelReader.ParseSheetDataToObject<ISurveyPollExcelTemplate>(parameter.file, this.formType.toString()))
      .then(result => (parameter.pollAndSurveyTemplates = result))
      .then(() => this.checkPollOrSurveySheetValidFormat(parameter, this.formType.toString()))
      .then(() => this.checkPollAndSurveyUserEmailDataCorrect(parameter, this.formType.toString()))
      .catch(err => Promise.reject(err));
  }

  public async checkQuizSheetValidFormat(parameter: ImportFormParameters, sheetName: string): Promise<void> {
    return new Promise((resolve, reject) => {
      const quizData = parameter.quizTemplates;

      if (quizData.length === 0) {
        return reject('No data to import !');
      }

      quizData.forEach((rowData, index) => {
        const rowNumber = rowData.__rowNum__ + 1;
        this.drawImportProgress(index + 1, quizData.length, sheetName);
        if (rowData['Form ID'] === undefined) {
          parameter.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Form ID', rowNumber));
        }

        if (rowData['Form ID'] && this.hasDuplicateFormId(quizData, rowData)) {
          parameter.errors.push(ExcelReaderException.duplicateUniqueField(sheetName, 'Form ID', rowNumber, rowData['Form ID']));
        }

        if (rowData['Form name'] === undefined) {
          parameter.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Form Name', rowNumber));
        }

        if (rowData['Alternate Approving Officer'] && !this.vaidateEmailFormat(rowData['Alternate Approving Officer'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Alternate Approving Officer', rowNumber));
        }

        if (rowData['Primary Approving Officer'] && !this.vaidateEmailFormat(rowData['Primary Approving Officer'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Primary Approving Officer', rowNumber));
        }

        if (rowData['Randomize question'] !== 'Y' && rowData['Randomize question'] !== 'N') {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Randomize question', rowNumber));
        }

        if (rowData['Archive date'] && !this.validDateFormat(rowData['Archive date'].toString())) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Archive date', rowNumber));
        }

        if (rowData['Display feedback'] && !this.validateDisplayFeedbackFormat(rowData['Display feedback'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Display feedback', rowNumber));
        }

        if (rowData['Passing mark percent tage'] && isNaN(rowData['Passing mark percent tage'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Passing mark percent tage', rowNumber));
        }

        if (rowData['Passing mark percent tage'] && this.reachedMaxValuePassingMarkPercentage(rowData['Passing mark percent tage'])) {
          parameter.errors.push(ExcelReaderException.reachedMaxValue(sheetName, 'Passing mark percent tage', rowNumber));
        }

        if (rowData['Passing mark score'] && isNaN(rowData['Passing mark score'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Passing mark score', rowNumber));
        }

        if (rowData['Max attempts'] && isNaN(rowData['Max attempts'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Max attempts', rowNumber));
        }

        if (rowData['Time limit'] && isNaN(rowData['Time limit'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Time limit', rowNumber));
        }

        if (
          (rowData['Max attempts'] && rowData['Passing mark percent tage']) ||
          (rowData['Max attempts'] && rowData['Passing mark score']) ||
          (rowData['Passing mark percent tage'] && rowData['Passing mark score'])
        ) {
          parameter.errors.push(
            new ExcelError(
              sheetName,
              `The max attempts, Passing Mark Percentage and Passing Mark Score can only filled one. Please re-check at row ${index + 2}`
            )
          );
        }
      });
      resolve();
    });
  }

  public async checkPollOrSurveySheetValidFormat(parameter: ImportFormParameters, sheetName: string): Promise<void> {
    return new Promise((resolve, reject) => {
      const result = parameter.pollAndSurveyTemplates;

      if (result.length === 0) {
        return reject('No data to import !');
      }

      result.forEach((rowData, index) => {
        const rowNumber = rowData.__rowNum__ + 1;
        this.drawImportProgress(index + 1, result.length, sheetName);
        if (rowData['Alternate Approving Officer'] && !this.vaidateEmailFormat(rowData['Alternate Approving Officer'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Alternate Approving Officer', rowNumber));
        }

        if (rowData['Primary Approving Officer'] && !this.vaidateEmailFormat(rowData['Primary Approving Officer'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Primary Approving Officer', rowNumber));
        }

        if (rowData['Archive date'] && !this.validDateFormat(rowData['Archive date'].toString())) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Archive date', rowNumber));
        }

        if (rowData['Form ID'] === undefined) {
          parameter.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Form ID', rowNumber));
        }

        if (rowData['Form ID'] && isNaN(rowData['Form ID'])) {
          parameter.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Form ID', rowNumber));
        }

        if (rowData['Form ID'] && this.hasDuplicateFormId(result, rowData)) {
          parameter.errors.push(ExcelReaderException.duplicateUniqueField(sheetName, 'Form ID', rowNumber, rowData['Form ID']));
        }
      });
      resolve();
    });
  }

  public async checkQuizUserEmailDataCorrect(parameter: ImportFormParameters): Promise<void> {
    return new Promise(resolve => {
      const data = parameter.quizTemplates;
      const totalEmail = [
        ...[...new Set(data.map(item => item['Alternate Approving Officer']).filter(email => this.vaidateEmailFormat(email)))],
        ...[...new Set(data.map(item => item['Primary Approving Officer']).filter(email => this.vaidateEmailFormat(email)))]
      ];

      if (totalEmail.length === 0) {
        return resolve();
      }

      this.userRepository
        .loadBaseUserInfoList(
          {
            emails: totalEmail,
            pageSize: null,
            pageIndex: null,
            searchKey: null
          },
          false
        )
        .subscribe(existUserInSystem => {
          data.forEach((rowData, index) => {
            const rowNumber = rowData.__rowNum__ + 1;
            const aOEmail = rowData['Alternate Approving Officer'];
            const pOEmail = rowData['Primary Approving Officer'];

            if (aOEmail && !existUserInSystem.some(x => x.emailAddress === aOEmail)) {
              parameter.errors.push(ExcelReaderException.dataNotFound('Quiz', 'Alternate Approving Officer Email', rowNumber));
            } else {
              rowData['Alternate Approving Officer'] = aOEmail ? existUserInSystem.find(x => x.emailAddress === aOEmail).id : null;
            }

            if (pOEmail && !existUserInSystem.some(x => x.emailAddress === pOEmail)) {
              parameter.errors.push(ExcelReaderException.dataNotFound('Quiz', 'Primary Approving Officer Email', rowNumber));
            } else {
              rowData['Primary Approving Officer'] = pOEmail ? existUserInSystem.find(x => x.emailAddress === pOEmail).id : null;
            }
          });
          resolve();
        });
    });
  }

  public async checkPollAndSurveyUserEmailDataCorrect(parameter: ImportFormParameters, sheetName: string): Promise<void> {
    return new Promise(resolve => {
      const data = parameter.pollAndSurveyTemplates;
      const totalEmail = [
        ...[...new Set(data.map(item => item['Alternate Approving Officer']).filter(email => this.vaidateEmailFormat(email)))],
        ...[...new Set(data.map(item => item['Primary Approving Officer']).filter(email => this.vaidateEmailFormat(email)))]
      ];

      if (totalEmail.length === 0) {
        return resolve();
      }

      this.userRepository
        .loadBaseUserInfoList(
          {
            emails: totalEmail,
            pageSize: null,
            pageIndex: null,
            searchKey: null
          },
          false
        )
        .subscribe(existUserInSystem => {
          data.forEach((rowData, index) => {
            const rowNumber = rowData.__rowNum__ + 1;
            const aOEmail = rowData['Alternate Approving Officer'];
            const pOEmail = rowData['Primary Approving Officer'];

            if (aOEmail && !existUserInSystem.some(x => x.emailAddress === aOEmail)) {
              parameter.errors.push(ExcelReaderException.dataNotFound(sheetName, 'Alternate Approving Officer Email', rowNumber));
            } else {
              rowData['Alternate Approving Officer'] = aOEmail ? existUserInSystem.find(x => x.emailAddress === aOEmail).id : null;
            }

            if (pOEmail && !existUserInSystem.some(x => x.emailAddress === pOEmail)) {
              parameter.errors.push(ExcelReaderException.dataNotFound(sheetName, 'Primary Approving Officer Email', rowNumber));
            } else {
              rowData['Primary Approving Officer'] = pOEmail ? existUserInSystem.find(x => x.emailAddress === pOEmail).id : null;
            }
          });
          resolve();
        });
    });
  }

  public hasDuplicateFormId(questionList: IQuizExcelTemplate[], question: IQuizExcelTemplate): boolean {
    return questionList.filter(quest => quest['Form ID'] === question['Form ID']).length > 1;
  }

  public reachedMaxValuePassingMarkPercentage(value: number): boolean {
    return !isNaN(value) && value > 100;
  }

  public vaidateEmailFormat(email: string): boolean {
    const regex = new RegExp(/\S+@\S+\.\S+/);
    return regex.test(email);
  }

  public validateDisplayFeedbackFormat(type: ImportDisplayFeedback): boolean {
    return Object.values(ImportDisplayFeedback).includes(type);
  }

  /* end region */

  /*
   *
   * 'Question' Spreadsheet region
   *
   */
  public readQuestionSheet(param: ImportFormParameters): Promise<void> {
    const formImportData = this.formType === FormType.Quiz ? param.quizTemplates : param.pollAndSurveyTemplates;
    const sheetName = 'Question';

    return ExcelReader.ParseSheetDataToObject<IQuestionExcelTemplate>(param.file, sheetName)
      .then(result => {
        result.forEach((rowData, index) => {
          const rowNumber = rowData.__rowNum__ + 1;
          this.drawImportProgress(index + 1, result.length, sheetName);

          if (rowData['Question ID'] === undefined) {
            param.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Question ID', rowNumber));
          }

          if (rowData['Question ID'] && isNaN(rowData['Question ID'])) {
            param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Question ID', rowNumber));
          }

          if (rowData['Question ID'] && this.hasDuplicateQuestionId(result, rowData)) {
            param.errors.push(ExcelReaderException.duplicateUniqueField(sheetName, 'Question ID', rowNumber, rowData['Question ID']));
          }

          if (!rowData['Form ID'] || !formImportData.some(x => x['Form ID'] === rowData['Form ID'])) {
            param.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Form ID', rowNumber));
          }

          if (rowData['Form ID'] && isNaN(rowData['Form ID'])) {
            param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Form ID', rowNumber));
          }

          if (rowData['Question title'] === undefined) {
            param.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Question Title', rowNumber));
          }

          if (rowData.Score && isNaN(rowData.Score)) {
            param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Question Score', rowNumber));
          }

          if (this.formType === FormType.Poll && this.hasDuplicateQuestion(result, rowData)) {
            param.errors.push(new ExcelError(sheetName, `Poll can only have 1 question, error at row ${rowNumber}`));
          }

          if (this.validateQuestionTypeFormat(rowData['Question type'])) {
            if (!this.validateQuestionTypeByFormType(rowData['Question type'])) {
              param.errors.push(
                new ExcelError(sheetName, `${this.formType.toString()} doesn't have this Question Type, error at ${rowNumber}`)
              );
            }
          } else {
            param.errors.push(ExcelReaderException.invalidFormat('Question', 'Question Type', rowNumber));
          }
        });
        param.questionTemplates = result;
        Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  }

  public hasDuplicateQuestionId(questionList: IQuestionExcelTemplate[], question: IQuestionExcelTemplate): boolean {
    return questionList.filter(quest => quest['Question ID'] === question['Question ID']).length > 1;
  }

  public hasDuplicateQuestion(questionList: IQuestionExcelTemplate[], question: IQuestionExcelTemplate): boolean {
    return questionList.filter(quest => quest['Form ID'] === question['Form ID']).length > 1;
  }

  public validateQuestionTypeFormat(type: ImportQuestionType): boolean {
    return Object.values(ImportQuestionType).includes(type);
  }

  public validDateFormat(input: string): boolean {
    // format dd/MM/YYYY
    const date = input.trim();
    const regex = new RegExp(/^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/);
    return regex.test(date);
  }

  public validateQuestionTypeByFormType(type: ImportQuestionType): boolean {
    switch (this.formType) {
      case FormType.Quiz:
        return true;
      case FormType.Poll:
        return type === ImportQuestionType['Radio buttons'] || type === ImportQuestionType['Check box'];
      case FormType.Survey:
        return type !== ImportQuestionType.Section && type !== ImportQuestionType['Fill in the blanks'];
    }
  }
  /* end region */

  /*
   *
   * Question Answer Spreadsheet region
   *
   */
  public readQuestionAnswerSheet(param: ImportFormParameters): Promise<void> {
    const sheetName = `Question's Answer`;
    return ExcelReader.ParseSheetDataToObject<IQuestionAnswerExcelTemplate>(param.file, sheetName)
      .then(result => {
        result.forEach((rowData, index) => {
          const rowNumber = rowData.__rowNum__ + 1;

          this.drawImportProgress(index + 1, result.length, sheetName);

          if (!rowData['Question ID'] || !this.isQuestionIDExistInQuestionSheet(param.questionTemplates, rowData)) {
            param.errors.push(ExcelReaderException.entityNotFound(sheetName, 'Question ID', rowNumber));
            return;
          }

          if (!rowData['Question Answer']) {
            param.errors.push(
              new ExcelError(sheetName, `The question at row ${rowNumber} in Question spreadsheet does not have any question answer.`)
            );
            return;
          }

          if (this.formType === FormType.Quiz && rowData['Correct Answer'] && !this.validCorrectAnsweFormat(rowData['Correct Answer'])) {
            param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Correct Answer', rowNumber));
          }

          const question = param.questionTemplates.find(x => x['Question ID'] === rowData['Question ID']);
          const questionType = question['Question type'];
          const questionAnswer = rowData['Question Answer'].toString();

          switch (questionType) {
            // TRUE/FALSE
            case ImportQuestionType['True/False']:
              if (this.formType === FormType.Quiz && this.getAnswerOptionNumber(result, rowData) > 1) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have one options. Error at row ${rowNumber}`));
              }
              if (this.formType === FormType.Quiz && !this.validTrueFalseAnswerFormat(rowData['Question Answer'])) {
                param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'True/False', rowNumber));
              }
              break;
            // DATE RANGE PICKER
            case ImportQuestionType['Date picker: Date range']:
              if (this.formType === FormType.Quiz && this.getAnswerOptionNumber(result, rowData) > 2) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have two options. Error at row ${rowNumber}`));
              }
              if (this.formType === FormType.Quiz && !this.validDateRangeAnswerFormat(questionAnswer)) {
                param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Date picker: Date range', rowNumber));
                break;
              }
              if (this.formType === FormType.Quiz && this.isDuplicateAnswerValue(result, rowData, '[Start date]')) {
                param.errors.push(
                  ExcelReaderException.duplicateUniqueField(sheetName, 'Date picker: Date range', rowNumber, '[Start date]')
                );
              }
              if (this.formType === FormType.Quiz && this.isDuplicateAnswerValue(result, rowData, '[End date]')) {
                param.errors.push(ExcelReaderException.duplicateUniqueField(sheetName, 'Date picker: Date range', rowNumber, '[End date]'));
              }
              break;
            // DATE PICKER
            case ImportQuestionType['Date picker: One date']:
              if (this.formType === FormType.Quiz && this.getAnswerOptionNumber(result, rowData) > 1) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have one options. Error at row ${rowNumber}`));
              }

              if (this.formType === FormType.Quiz && !this.validDateFormat(questionAnswer)) {
                param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Date Picker: One date', rowNumber));
              }
              break;
            // FREE TEXT
            case ImportQuestionType['Free text']:
              if (this.formType === FormType.Quiz && this.getAnswerOptionNumber(result, rowData) > 1) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have one options. Error at row ${rowNumber}`));
              }
              break;
            case ImportQuestionType.Section:
              if (this.formType !== FormType.Poll && this.getAnswerOptionNumber(result, rowData) > 1) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have one options. Error at row ${rowNumber}`));
              }
              break;
            // FILL IN THE BLANKS
            case ImportQuestionType['Fill in the blanks']:
              if (!this.validFillInTheBlanksFormat(questionAnswer)) {
                param.errors.push(ExcelReaderException.invalidFormat(sheetName, 'Fill In The Blanks', rowNumber));
                break;
              }
              if (this.missingFillInTheBlankAnswer(questionAnswer, '[Blank]')) {
                param.errors.push(new ExcelError(sheetName, `Fill In The Blanks missing [Blank] answer. Error at row ${rowNumber}`));
              }

              if (this.missingFillInTheBlankAnswer(questionAnswer, '[Text]')) {
                param.errors.push(new ExcelError(sheetName, `Fill In The Blanks missing [Text] answer. Error at row ${rowNumber}`));
              }
              break;
            case ImportQuestionType['Radio buttons']:
              if (this.isDuplicateCorrectAnswer(result, rowData)) {
                param.errors.push(new ExcelError(sheetName, `${questionType} can only have one Correct Answer. Error at row ${rowNumber}`));
              }
              break;
            default:
              break;
          }
        });
        param.questionAnswerTemplates = result;
        Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  }

  public isQuestionIDExistInQuestionSheet(questionList: IQuestionExcelTemplate[], questionAnswer: IQuestionAnswerExcelTemplate): boolean {
    return questionList.some(x => x['Question ID'] === questionAnswer['Question ID']);
  }

  public validFillInTheBlanksFormat(answerValue: string): boolean {
    return answerValue && this.formType === FormType.Quiz && (answerValue.startsWith('[Text]') || answerValue.startsWith('[Blank]'));
  }

  public validDateRangeAnswerFormat(answerValue: string): boolean {
    return (
      answerValue &&
      this.formType === FormType.Quiz &&
      ((answerValue.startsWith('[Start date]') && this.validDateFormat(answerValue.replace('[Start date]', ''))) ||
        (answerValue.startsWith('[End date]') && this.validDateFormat(answerValue.replace('[End date]', ''))))
    );
  }

  public validTrueFalseAnswerFormat(answerValue: string): boolean {
    return answerValue && this.formType === FormType.Quiz && (answerValue.startsWith('[True]') || answerValue.startsWith('[False]'));
  }

  public isDuplicateAnswerValue(answerList: IQuestionAnswerExcelTemplate[], answer: IQuestionAnswerExcelTemplate, option: string): boolean {
    return answerList.filter(x => x['Question ID'] === answer['Question ID'] && x['Question Answer'].startsWith(option)).length > 1;
  }

  public getAnswerOptionNumber(answerList: IQuestionAnswerExcelTemplate[], answer: IQuestionAnswerExcelTemplate): number {
    return answerList.filter(x => x['Question ID'] === answer['Question ID']).length;
  }

  public isDuplicateCorrectAnswer(answerList: IQuestionAnswerExcelTemplate[], answer: IQuestionAnswerExcelTemplate): boolean {
    return (
      answerList.filter(x => x['Question ID'] === answer['Question ID'] && x['Correct Answer'] && x['Correct Answer'] === 'X').length === 2
    );
  }

  public validCorrectAnsweFormat(value: string): boolean {
    return value.trim().toLowerCase() === 'x';
  }

  public missingFillInTheBlankAnswer(value: string, key: string): boolean {
    return value.startsWith(key) && value.replace(key, '').trim() === '';
  }
  /* end region */

  /*
   *
   * Additional Validate
   *
   */
  public additionalValidateForQuiz(param: ImportFormParameters): Promise<void> {
    if (this.formType === FormType.Quiz) {
      const quizData = param.quizTemplates;
      const sheetName = 'Quiz';

      quizData.forEach((form, index) => {
        const question = param.questionTemplates.filter(x => x['Form ID'] === form['Form ID']);
        const totalScore = question.reduce((x, y) => x + y.Score, 0);

        if (form['Passing mark score'] && form['Passing mark score'] > totalScore) {
          param.errors.push(new ExcelError(sheetName, `Passing Mark Score higher than total Question Score. Error at row ${index + 2}`));
        }

        if (form['Passing mark score'] && form['Passing mark score'] < totalScore) {
          param.errors.push(new ExcelError(sheetName, `Passing Mark Score less than total Question Score. Error at row ${index + 2}`));
        }
      });
    }

    return Promise.resolve();
  }
  /* end region */

  public completeImportTransaction(parameter: ImportFormParameters): Promise<void> {
    if (parameter.errors.length !== 0) {
      this.errors = parameter.errors;
      this.importProgressTitle = 'Invalid data';
      this.errorDialogRef = this.moduleFacadeService.dialogService.open({
        content: this.errorResultDialog
      });

      return Promise.resolve();
    }

    this.importProgressTitle = 'Data is valid';
    this.importParameter = parameter;

    Promise.resolve();
  }

  public onClickUploadForm(): void {
    let dataItems: ImportFormModel[];

    switch (this.formType) {
      case FormType.Quiz:
        dataItems = this.buildUploadQuizModel();
        break;
      case FormType.Poll:
      case FormType.Survey:
        dataItems = this.buildUploadSurveyPollModel();
        break;
    }

    this.formApiService.importForm({ formWithQuestionsSections: dataItems }, true).subscribe(() => {
      this.showNotification(`Import form successfully`, NotificationType.Success);
      this.dialogRef.close(true);
    });
  }

  public buildUploadQuizModel(): ImportFormModel[] {
    const importItems: ImportFormModel[] = [];
    const quizTemplates = this.importParameter.quizTemplates;
    const questionTemplates = this.importParameter.questionTemplates;
    const questionAnswerTemplates = this.importParameter.questionAnswerTemplates;

    quizTemplates.forEach(quiz => {
      const questions = this.getQuestionListByFormID(questionTemplates, quiz['Form ID']);

      const uploadData = <ImportFormModel>{
        form: ImportFormParser.buildQuizModel(quiz, this.formType),
        formQuestionsSections: questions
          ? ImportFormParser.buildQuestionModel(questions, questionAnswerTemplates, FormType.Quiz)
          : undefined
      };

      importItems.push(uploadData);
    });

    return importItems;
  }

  public buildUploadSurveyPollModel(): ImportFormModel[] {
    const importItems: ImportFormModel[] = [];
    const templates = this.importParameter.pollAndSurveyTemplates;
    const questionTemplates = this.importParameter.questionTemplates;
    const questionAnswerTemplates = this.importParameter.questionAnswerTemplates;

    templates.forEach(template => {
      const questions = this.getQuestionListByFormID(questionTemplates, template['Form ID']);

      const uploadData = <ImportFormModel>{
        form: ImportFormParser.buildSurveyPollModel(template, this.formType),
        formQuestionsSections: questions
          ? ImportFormParser.buildQuestionModel(questions, questionAnswerTemplates, FormType.Quiz)
          : undefined
      };

      importItems.push(uploadData);
    });

    return importItems;
  }

  public getQuestionListByFormID(questions: IQuestionExcelTemplate[], formId: number): IQuestionExcelTemplate[] {
    return questions.filter(x => x['Form ID'] === formId);
  }

  public get getDownloadBtnTitle(): string {
    return `Download ${this.formType.toString()} Template`;
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onCloseErrorDialog(): void {
    this.errorDialogRef.close();
    this.resetImportProcess();
  }

  public onDownloadTemplate(): void {
    const downloadEl = document.createElement('a');
    switch (this.formType) {
      case FormType.Quiz:
        downloadEl.href = this.baseHref + 'assets/templates/Quiz_Template.xlsx';
        downloadEl.download = 'QuizTemplate';
        break;
      case FormType.Survey:
        downloadEl.href = this.baseHref + 'assets/templates/Survey_Template.xlsx';
        downloadEl.download = 'SurveyTemplate';
        break;
      case FormType.Poll:
        downloadEl.href = this.baseHref + 'assets/templates/Poll_Template.xlsx';
        downloadEl.download = 'PollTemplate';
        break;
    }

    downloadEl.click();
  }

  public resetImportProcess(): void {
    this.errors = [];
    this.importFile = undefined;
    this.importPercentage = 0;
    this.importProgressTitle = undefined;
    this.importParameter = undefined;
  }

  public onIconSelectFileClick(): void {
    this.inputFileEl.nativeElement.click();
  }

  private drawImportProgress(current: number, max: number, sheetName: string): void {
    this.importProgressTitle = `Validate ${sheetName}`;
    this.importPercentage = (current / max) * 100;
  }
}

export class ImportFormParameters {
  constructor(
    public file?: File,
    public errors: ExcelError[] = [],
    public quizTemplates: IQuizExcelTemplate[] = [],
    public pollAndSurveyTemplates: ISurveyPollExcelTemplate[] = [],
    public questionTemplates: IQuestionExcelTemplate[] = [],
    public questionAnswerTemplates: IQuestionAnswerExcelTemplate[] = []
  ) {}
}
