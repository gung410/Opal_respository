import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, NotificationType, Utils } from '@opal20/infrastructure';
import {
  BrokenLinkReportType,
  FormQuestionModel,
  FormType,
  IOptionMedia,
  ISaveQuestionBankRequest,
  QuestionAnswerSingleValue,
  QuestionAnswerValue,
  QuestionBankApiService,
  QuestionOption,
  QuestionOptionType,
  QuestionType,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ValidationErrors, Validators } from '@angular/forms';

import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { FormQuestionOptionEditorComponent } from './form-question-option-editor.component';
import { QuestionBankAddDialogComponent } from './dialogs/question-bank-add-dialog.component';
import { requiredIfValidator } from '@opal20/common-components';

const OPAL_MULTIPLE_CHOICE = 'opal-multiple-choice';

@Component({
  selector: 'form-question-editor',
  templateUrl: './form-question-editor.component.html'
})
export class FormQuestionEditorComponent extends BaseFormComponent {
  public readonly formTypeEnum: typeof FormType = FormType;

  @ViewChildren(FormQuestionOptionEditorComponent) public questionOptionEditors: QueryList<FormQuestionOptionEditorComponent>;
  @ViewChild('titleInput', { static: false }) public titleInput: ElementRef<HTMLInputElement>;
  @ViewChild('addNewOptionEditor', { static: false }) public addNewOptionEditor: FormQuestionOptionEditorComponent | undefined;
  @ViewChild('datePickerElement', { static: false }) public datePickerElement: DatePickerComponent | undefined;
  @ViewChild('datePickerFromElement', { static: false }) public datePickerFromElement: DatePickerComponent | undefined;
  @ViewChild('datePickerEndElement', { static: false }) public datePickerEndElement: DatePickerComponent | undefined;

  @Input() public formType: FormType;
  @Input() public data: FormQuestionModel = new FormQuestionModel();
  @Input() public selected: boolean = false;
  @Input() public disableSetCorrectAnswer: boolean = false;
  @Input() public disableMoveUp: boolean = false;
  @Input() public branchingOptionQuestions: FormQuestionModel[] = [];
  @Input() public isViewMode: boolean = false;

  @Output('dataChange')
  public dataChangeEvent: EventEmitter<FormQuestionModel> = new EventEmitter<FormQuestionModel>();
  @Output('moveUp')
  public moveUpEvent: EventEmitter<string> = new EventEmitter();
  @Output('moveDown')
  public moveDownEvent: EventEmitter<string> = new EventEmitter();
  @Output('delete')
  public deleteEvent: EventEmitter<string> = new EventEmitter();

  public hovering: boolean = false;
  public questionType: typeof QuestionType = QuestionType;
  public questionOptionType: typeof QuestionOptionType = QuestionOptionType;
  public addNewOptionValue: string = '';
  public addNewOptionIsCorrectAnswerValue: boolean = false;
  public logicValidationErrors: ValidationErrors | undefined;
  public originalQuestionTitle: string;
  public brokenLinkReportTypeForm: BrokenLinkReportType = BrokenLinkReportType.Form;

  public get logicValidationErrorKeys(): string[] | undefined {
    return this.logicValidationErrors ? Object.keys(this.logicValidationErrors) : undefined;
  }
  public mode: FormDetailMode = this.formEditModeService.initMode;
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;
  public currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private readonly MIN_TIME: string = '0001-01-01'; // By default, min year of Kendo date picker is 1900

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private formEditModeService: FormEditModeService,
    private questionBankService: QuestionBankApiService
  ) {
    super(moduleFacadeService);
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public get isDisableMoveUp(): boolean {
    return (this.data.minorPriority == null && this.data.priority === 0) || this.data.minorPriority === 0 || this.disableMoveUp;
  }

  public get isDisableMoveDown(): boolean {
    return (
      (this.data.minorPriority == null && this.data.priority === this.branchingOptionQuestions.length - 1) ||
      this.data.minorPriority === this.branchingOptionQuestions.length - 1 ||
      this.formType === this.formTypeEnum.Poll
    );
  }

  public get indexInform(): string {
    const sectionIndex = this.data.priority + 1;
    const questionIndex = this.data.minorPriority >= 0 ? this.data.minorPriority + 1 : '';
    return questionIndex ? `${sectionIndex}.${questionIndex}` : `${sectionIndex}`;
  }

  public onQuestionTitleChanged(value: string): void {
    this.data.questionTitle = value ? value.replace(/\s/g, '') : '';
    this.updateData(p => {
      p.questionTitle = value;
    });
  }

  public onQuestionOptionValueChanged(value: string, optionIndex: number): void {
    this.updateData(p => {
      p.updateQuestionOptionValue(value, optionIndex);
    });
  }

  public onShortTextQuestionCorrectAnswerChanged(value: string): void {
    this.updateData(p => {
      p.questionCorrectAnswer = value;
    });
  }

  public onDatePickerQuestionCorrectAnswerChanged(value: Date): void {
    const date = value ? value.toDateString() : '';
    this.updateData(question => {
      question.questionCorrectAnswer = date;
    });
  }

  public onDateRangePickerQuestionCorrectAnswerChanged(value: Date, isFromDate?: boolean): void {
    const date = value ? value.toDateString() : '';
    this.updateData(p => {
      if (!(p.questionCorrectAnswer instanceof Array)) {
        p.questionCorrectAnswer = new Array();
      }

      if (isFromDate) {
        p.questionCorrectAnswer[0] = date;
        this.datePickerEndElement.min = this.formatToKendoDate(p.questionCorrectAnswer[0]);
      } else {
        p.questionCorrectAnswer[1] = date;
        this.datePickerFromElement.max = this.formatToKendoDate(p.questionCorrectAnswer[1]);
      }
    });
  }

  public ngAfterViewInit(): void {
    super.ngAfterViewInit();
  }

  public onUpdateOrRemoveTextOption(textValue: QuestionAnswerSingleValue, currentQuestionOptionIndex: number): void {
    this.updateData(async (data: FormQuestionModel) => {
      if (!textValue) {
        data.removeOption(currentQuestionOptionIndex);
        data.generateTitle();
        return;
      }
      data.updateQuestionOptionValue(textValue, currentQuestionOptionIndex);
      data.generateTitle();
    });
  }

  public onUpdateOrRemoveBlankOption(blankValue: QuestionAnswerSingleValue, currentQuestionOptionIndex: number): void {
    this.updateData(async (data: FormQuestionModel) => {
      if (!blankValue) {
        data.removeOption(currentQuestionOptionIndex);
        data.generateTitle();
        return;
      }
      data.updateQuestionOptionValue(blankValue, currentQuestionOptionIndex);
      data.generateTitle();
    });
  }

  public onAddNewTextOptionSubmit(textValue: QuestionAnswerSingleValue): void {
    if (!textValue) {
      return;
    }
    this.updateData(async (data: FormQuestionModel) => {
      data.addTextOption(textValue);
      data.generateTitle();
    });
    this.validate();
  }

  public onAddNewBlankOptionSubmit(blankValue: QuestionAnswerSingleValue): void {
    if (!blankValue) {
      return;
    }
    this.updateData(async (data: FormQuestionModel) => {
      data.addBlankOption(blankValue, null);
      data.generateTitle();
    });
    this.validate();
  }

  public onTrueFalseQuestionOptionCheckedChange(e: boolean, value: QuestionAnswerSingleValue): void {
    this.updateData(p => {
      p.questionCorrectAnswer = value;
    });
    this.validate();
  }

  public onSingleChoiceQuestionOptionCheckedChange(e: boolean, value: QuestionAnswerSingleValue): void {
    if (e === true) {
      this.updateData(p => {
        p.questionCorrectAnswer = value;
      });
      this.validate();
    }
  }

  public onMultipleChoiceQuestionOptionCheckedChange(e: boolean, value: string): void {
    this.updateData(p => {
      if (!(p.questionCorrectAnswer instanceof Array)) {
        throw new Error('Question correct answer must be an array');
      }

      if (e === false) {
        Utils.remove(p.questionCorrectAnswer, questionCorrectAnswerItem => questionCorrectAnswerItem === value);
      } else {
        p.questionCorrectAnswer.push(value);
      }
    });
    this.validate();
  }

  public onRemoveCorrectAnswerWithEmptyValue(isRemove: boolean, code: number, value: QuestionAnswerSingleValue): void {
    this.updateData(p => {
      const val = p.generateValueForImageOption(code);
      if (!(p.questionCorrectAnswer instanceof Array)) {
        throw new Error('Question correct answer must be an array');
      }
      if (isRemove) {
        Utils.remove(p.questionCorrectAnswer, questionCorrectAnswerItem => questionCorrectAnswerItem === val);
      } else {
        this.data.updateQuestionCorrectAnswer(value, val);
      }
    });
    this.validate();
  }

  public onRemoveOptionClicked(e: MouseEvent, optionIndex: number): void {
    e.stopImmediatePropagation();
    this.updateData(p => {
      p.removeOption(optionIndex);
    });
  }

  public onContextMenuItemSelect(eventData: { id: string }): void {
    switch (eventData.id) {
      case 'Delete':
        this.delete();
        break;
      case 'MoveUp':
        this.moveUp();
        break;
      case 'MoveDown':
        this.moveDown();
        break;
      case 'MoveToBank':
        this.moveToBank();
    }
  }

  public processAddNewQuestionOption(newOptionValue: string): void {
    if (newOptionValue.trim() === '') {
      return;
    }
    this.addNewOptionValue = newOptionValue;

    this.updateData(p => {
      p.addNewOption(this.addNewOptionValue, this.addNewOptionIsCorrectAnswerValue);
    });
    this.addNewOptionValue = '';
    this.addNewOptionIsCorrectAnswerValue = false;
    this.validate();
    setTimeout(() => {
      if (this.processAddNewQuestionOption) {
        this.addNewOptionEditor.focusValueInput();
        this.addNewOptionEditor.clearNewOptionValue();
      }
    });
  }

  public getTrueFalseQuestionOptionLabel(optionValue: QuestionAnswerSingleValue): string {
    return optionValue ? 'True' : 'False';
  }

  public updateData(updatefn: (data: FormQuestionModel) => void): void {
    this.data = Utils.clone(this.data, p => {
      updatefn(p);
    });
    this.dataChangeEvent.emit(this.data);
  }

  public moveUp(): void {
    this.moveUpEvent.emit(this.data.id);
  }

  public moveDown(): void {
    this.moveDownEvent.emit(this.data.id);
  }

  public delete(): void {
    this.deleteEvent.emit(this.data.id);
  }

  public async moveToBank(): Promise<void> {
    const isValid = await this.validate();

    if (!isValid) {
      return;
    }

    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: QuestionBankAddDialogComponent });

    dialogRef.result.toPromise().then((qBank: { title: string; questionGroupName: string }) => {
      if (qBank && qBank.title) {
        const questionBankReq: ISaveQuestionBankRequest = {
          title: qBank.title,
          questionGroupName: qBank.questionGroupName,
          questionTitle: this.data.questionTitle,
          questionType: this.data.questionType,
          questionCorrectAnswer: this.data.questionCorrectAnswer,
          questionOptions: this.data.questionOptions,
          questionHint: this.data.questionHint,
          answerExplanatoryNote: this.data.answerExplanatoryNote,
          feedbackCorrectAnswer: this.data.feedbackCorrectAnswer,
          feedbackWrongAnswer: this.data.feedbackWrongAnswer,
          questionLevel: this.data.questionLevel,
          randomizedOptions: this.data.randomizedOptions,
          score: this.data.score,
          isDeleted: this.data.isDeleted,
          isScoreEnabled: this.data.isScoreEnabled
        };
        this.questionBankService.createQuestionBank(questionBankReq);
        this.showNotification('Successfully move the question to question bank.', NotificationType.Success);
      }
    });
  }

  public questionOptionTrackByFn(index: number, item: QuestionOption): number {
    return item.code;
  }

  @HostListener('mouseover')
  public onMouseOver(): void {
    this.hovering = true;
  }

  @HostListener('mouseout')
  public onMouseOut(): void {
    this.hovering = false;
  }

  public formatToKendoDate(dateString: QuestionAnswerValue | undefined): Date {
    return dateString ? new Date(dateString.toString()) : null;
  }

  public canEditQuestion(): boolean {
    return this.mode !== FormDetailMode.View && !this.data.isSurveyTemplateQuestion && !this.isViewMode;
  }

  public canShowQuestionTitle(): boolean {
    return (
      !(this.data.questionType === this.questionType.FillInTheBlanks) &&
      this.data.questionType !== this.questionType.Qset &&
      this.data.questionType !== this.questionType.Section &&
      this.data.questionType !== this.questionType.Note
    );
  }

  public isHideIndexInForm(): boolean {
    return this.data.questionType === this.questionType.ShortText && this.formType === FormType.Poll;
  }

  public isDisabledQuestionDeleteBtn(): boolean {
    return (
      this.data.questionType === this.questionType.Section ||
      this.data.questionType === this.questionType.Note ||
      this.data.questionType === this.questionType.Qset ||
      this.data.questionType === this.questionType.Smatrix ||
      (this.data.questionType === this.questionType.ShortText && this.formType === FormType.Poll)
    );
  }

  public onOptionMediaChange(media: IOptionMedia, optionIndex: number): void {
    this.updateData(question => {
      question.addQuestionOptionMediaUrl(media, optionIndex);
    });
  }

  // These functionals use for Single And Multiple choise with image option
  public optionMediaInsertEvent(media: IOptionMedia): void {
    if (!media) {
      return;
    }

    this.updateData(p => {
      this.addNewOptionValue = p.generateValueForImageOption(p.questionOptions.length + 1);
      p.addNewMediaOption(this.addNewOptionValue, this.addNewOptionIsCorrectAnswerValue, media);
    });

    this.addNewOptionValue = '';
    this.addNewOptionIsCorrectAnswerValue = false;
    this.validate();
    setTimeout(() => {
      if (this.processAddNewQuestionOption) {
        this.addNewOptionEditor.focusValueInput();
        this.addNewOptionEditor.clearNewOptionValue();
      }
    });
  }

  public onUpdateStateWithoutValue(isEmpltyValue: boolean, optionIndex: number): void {
    this.updateData(question => {
      if (isEmpltyValue) {
        question.questionOptions[optionIndex].isEmptyValue = true;
        question.questionOptions[optionIndex].value = question.generateValueForImageOption(optionIndex + 1);
      } else {
        question.questionOptions[optionIndex].isEmptyValue = false;
        question.questionOptions[optionIndex].value = '';
      }
    });
  }

  public onOptionMediaChangeWithoutValue(media: IOptionMedia, optionIndex: number): void {
    this.updateData(question => {
      question.questionOptions[optionIndex].isEmptyValue = true;
      question.questionOptions[optionIndex].value = question.generateValueForImageOption(optionIndex + 1);
      question.addQuestionOptionMediaUrl(media, optionIndex);
    });
  }

  public isChecked(data: FormQuestionModel, option: QuestionOption, optionIndex: number): boolean {
    const val = option.value || data.generateValueForImageOption(optionIndex + 1);
    return data.isOptionValueCorrect(val);
  }
  // These functionals use for Single And Multiple choise with image option

  public onOptionFeedbackChange(feedbackValue: string, optionIndex: number): void {
    this.data = Utils.clone(this.data, p => {
      const currentQuestionOption = p.questionOptions[optionIndex];
      currentQuestionOption.feedback = feedbackValue;
    });
    this.dataChangeEvent.emit(this.data);
  }

  public onFeedbackCorrectChange(feedbackCorrect: string): void {
    this.data = Utils.clone(this.data, p => {
      p.feedbackCorrectAnswer = feedbackCorrect;
    });
    this.dataChangeEvent.emit(this.data);
  }

  public onFeedbackWrongChange(feedbackWrong: string): void {
    this.data = Utils.clone(this.data, p => {
      p.feedbackWrongAnswer = feedbackWrong;
    });
    this.dataChangeEvent.emit(this.data);
  }

  ///////////// Branching //////////////
  public getBranchingOptions(): IDataItem[] {
    const branchingOptions = this.branchingOptionQuestions
      .filter(question => question.id !== this.data.id)
      .map(question => {
        return <IDataItem>{
          text: `${question.priority + 1}`,
          value: question.id
        };
      });
    return [{ text: this.translate('Continue to next question'), value: null }].concat(branchingOptions);
  }

  public onChooseNextQuestion(nextQuestionId: string): void {
    this.updateData(q => (q.nextQuestionId = nextQuestionId));
  }

  public onNextQuestionOptionChange(nextQuestionId: string, optionIndex: number): void {
    this.data = Utils.clone(this.data, p => {
      p.questionOptions[optionIndex].nextQuestionId = nextQuestionId;
    });
    this.dataChangeEvent.emit(this.data);
  }
  ///////////// End Branching //////////////

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      controls: {
        title: {
          defaultValue: this.data.questionTitle,
          validators: [
            { validator: Validators.required, validatorType: 'required' },
            { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
          ]
        }
      }
    };

    switch (this.data.questionType) {
      case QuestionType.ShortText:
        result.controls.shortTextCorrectAnswer = {
          defaultValue: this.data.questionCorrectAnswer,
          validators: [
            { validator: requiredIfValidator(p => this.formType === FormType.Quiz), validatorType: 'required' },
            { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
          ]
        };
        break;
      case QuestionType.DatePicker:
        result.controls.datePickerCorrectAnswer = {
          defaultValue: this.data.questionCorrectAnswer,
          validators: [{ validator: requiredIfValidator(p => this.formType === FormType.Quiz), validatorType: 'required' }]
        };
        break;
    }
    return result;
  }

  protected ignoreValidateForm(): boolean {
    if (this.data.questionType === QuestionType.FillInTheBlanks) {
      return true;
    }
    return false;
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return Promise.all(
      this.questionOptionEditors
        .toArray()
        .reverse()
        .map(p => p.validate())
    ).then(finalResult => {
      return !finalResult.includes(false) && !this.anyLogicError();
    });
  }

  protected onInit(): void {
    // Keep the original question title which is used to display on UI, prevent auto save update new value!
    this.originalQuestionTitle = this.data.questionTitle;

    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });
  }

  protected onAfterViewInit(): void {
    this.initDateQuestionBeginningValue();
  }

  private anyLogicError(): ValidationErrors | undefined {
    this.logicValidationErrors = FormQuestionModel.Validate(this.data, this.formType);
    return this.logicValidationErrors;
  }

  private initDateQuestionBeginningValue(): void {
    // Init initial value and min date for Date picker question
    if (this.datePickerElement) {
      this.datePickerElement.value = this.formatToKendoDate(this.data.questionCorrectAnswer);
      this.datePickerElement.min = new Date(this.MIN_TIME);
    }

    // Init initial value and min date for Date range picker question
    if (this.datePickerFromElement) {
      this.datePickerFromElement.value = this.formatToKendoDate(
        this.data.questionCorrectAnswer ? this.data.questionCorrectAnswer[0] : undefined
      );
      this.datePickerFromElement.min = new Date(this.MIN_TIME);
      this.datePickerFromElement.max = this.formatToKendoDate(
        this.data.questionCorrectAnswer ? this.data.questionCorrectAnswer[1] : undefined
      );
      this.datePickerEndElement.value = this.formatToKendoDate(
        this.data.questionCorrectAnswer ? this.data.questionCorrectAnswer[1] : undefined
      );
      this.datePickerEndElement.min = this.data.questionCorrectAnswer
        ? this.formatToKendoDate(this.data.questionCorrectAnswer[0])
        : new Date(this.MIN_TIME);
    }
  }
}
