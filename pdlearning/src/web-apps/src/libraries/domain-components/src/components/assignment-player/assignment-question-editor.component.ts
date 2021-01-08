import {
  AssignmentAnswerTrack,
  AssignmentQuestionAnswerSingleValue,
  AssignmentQuestionAnswerValue,
  AssignmentQuestionOption,
  AssignmentQuestionOptionType,
  AssignmentType,
  IOptionMedia,
  ParticipantAssignmentTrack,
  QuizAssignmentFormQuestion,
  QuizAssignmentQuestionType,
  UserInfoModel
} from '@opal20/domain-api';
import {
  BaseFormComponent,
  DateUtils,
  IFormBuilderDefinition,
  MAX_ZINDEX_OF_TOOLTIP,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ValidationErrors, Validators } from '@angular/forms';
import { ifValidator, requiredIfValidator, startEndValidator } from '@opal20/common-components';

import { AssignmentMode } from './../../models/assignment-mode.model';
import { AssignmentQuestionOptionEditorComponent } from './assignment-question-option-editor.component';
import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';
import { QuestionOptionImageUploadSettings } from '../../models/question-option-image-upload-setting.model';

@Component({
  selector: 'assignment-question-editor',
  templateUrl: './assignment-question-editor.component.html'
})
export class AssignmentQuestionEditorComponent extends BaseFormComponent {
  @ViewChildren(AssignmentQuestionOptionEditorComponent) public questionOptionEditors: QueryList<AssignmentQuestionOptionEditorComponent>;
  @ViewChild('titleInput', { static: false }) public titleInput: ElementRef<HTMLInputElement>;
  @ViewChild('addNewOptionEditor', { static: false }) public addNewOptionEditor: AssignmentQuestionOptionEditorComponent | undefined;
  @ViewChild('datePickerElement', { static: false }) public datePickerElement: DatePickerComponent | undefined;
  @ViewChild('datePickerFromElement', { static: false }) public datePickerFromElement: DatePickerComponent | undefined;
  @ViewChild('datePickerEndElement', { static: false }) public datePickerEndElement: DatePickerComponent | undefined;

  @Input() public assignmentType: AssignmentType;
  @Input() public data: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
  @Input() public positionInAssignment: number = 1;
  @Input() public selected: boolean = false;
  @Input() public totalQuestion: number = 1;
  @Input() public disableSetCorrectAnswer: boolean = false;
  @Input() public disableMoveUp: boolean = false;
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;
  @Input() public assignmentAnswerTrack: AssignmentAnswerTrack = new AssignmentAnswerTrack();

  @Output('dataChange')
  public dataChangeEvent: EventEmitter<QuizAssignmentFormQuestion> = new EventEmitter<QuizAssignmentFormQuestion>();
  @Output('moveUp')
  public moveUpEvent: EventEmitter<string> = new EventEmitter();
  @Output('moveDown')
  public moveDownEvent: EventEmitter<string> = new EventEmitter();
  @Output('delete')
  public deleteEvent: EventEmitter<string> = new EventEmitter();
  @Output('scoreChange')
  public scoreChangeEvent: EventEmitter<string> = new EventEmitter();

  public hovering: boolean = false;
  public QuizAssignmentQuestionType: typeof QuizAssignmentQuestionType = QuizAssignmentQuestionType;
  public AssignmentQuestionOptionType: typeof AssignmentQuestionOptionType = AssignmentQuestionOptionType;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public AssignmentType: typeof AssignmentType = AssignmentType;
  public addNewOptionValue: string = '';
  public addNewOptionIsCorrectAnswerValue: boolean = false;
  public logicValidationErrors: ValidationErrors | undefined;
  public originalQuestionTitle: string;
  public widthOfBlanks: number[] = [];
  public minWidthOfBlank: number = 50;
  public maxZIndexOfTooltip: number = MAX_ZINDEX_OF_TOOLTIP;
  public get logicValidationErrorKeys(): string[] | undefined {
    return this.logicValidationErrors ? Object.keys(this.logicValidationErrors) : undefined;
  }

  public imageUploadSettings: QuestionOptionImageUploadSettings = new QuestionOptionImageUploadSettings();
  public debounceValidate: () => void = Utils.debounce(() => this.validate(), 500);
  private currentUser = UserInfoModel.getMyUserInfo();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public displayCorrectDropdownQuestionClass(): boolean {
    return this.assignmentAnswerTrack.questionAnswer === this.data.question_CorrectAnswer && this.mode === AssignmentMode.ParticipantTrack;
  }

  public displayIncorrectDropdownQuestionClass(): boolean {
    return this.assignmentAnswerTrack.questionAnswer !== this.data.question_CorrectAnswer && this.mode === AssignmentMode.ParticipantTrack;
  }

  public displayCorrectQuestionOptionClass(option: AssignmentQuestionOption): boolean {
    return this.getCorrectQuestionOption(option) === true && this.mode !== AssignmentMode.LearnerAnswer;
  }

  public displayIncorrectQuestionOptionClass(option: AssignmentQuestionOption): boolean {
    return this.getCorrectQuestionOption(option) === false && this.mode === AssignmentMode.ParticipantTrack;
  }

  public displayCorrectDateClass(isFrom: boolean = true): boolean {
    return this.isDateCorrectAnswer(isFrom) === true && this.mode !== AssignmentMode.LearnerAnswer;
  }

  public displayIncorrectDateClass(isFrom: boolean = true): boolean {
    return this.isDateCorrectAnswer(isFrom) === false && this.mode === AssignmentMode.ParticipantTrack;
  }

  public onQuestionTitleChanged(value: string): void {
    this.data.question_Title = value ? value.replace(/\s/g, '') : '';
    this.updateData(p => {
      p.question_Title = value;
    });
  }

  public canGiveScore(): boolean {
    return (
      (this.data.question_Type !== QuizAssignmentQuestionType.FillInTheBlanks &&
        this.data.question_Type !== QuizAssignmentQuestionType.FreeText &&
        this.assignmentAnswerTrack.score == null) ||
      this.data.question_Type === QuizAssignmentQuestionType.FillInTheBlanks ||
      this.data.question_Type === QuizAssignmentQuestionType.FreeText
    );
  }

  public onBlankOptionChange(value: string, index: number): void {
    const indexOfBlankOption = this.getBlankIndex(index);

    this.updateData(question => {
      if (question.question_CorrectAnswer == null) {
        question.question_CorrectAnswer = [];
      }
      question.question_CorrectAnswer[indexOfBlankOption] = value;
    });

    setTimeout(() => {
      this.widthOfBlanks[index] = Math.max(this.minWidthOfBlank, (value.length + 1) * 8);
    });
  }

  public onChangeScore(value: number): void {
    this.assignmentAnswerTrack.giveScore = value;
    this.scoreChangeEvent.emit(this.data.id);
  }

  public getAnswerOfFillInTheBlankForOption(index: number): string {
    if (this.assignmentAnswerTrack.questionAnswer == null) {
      return '';
    }

    if (!Array.isArray(this.assignmentAnswerTrack.questionAnswer)) {
      return '';
    }

    const indexOfBlankOption = this.getBlankIndex(index);

    if (indexOfBlankOption > this.assignmentAnswerTrack.questionAnswer.length) {
      return '';
    }
    return this.assignmentAnswerTrack.questionAnswer[indexOfBlankOption].toString();
  }

  public onQuestionOptionValueChanged(value: string, optionIndex: number): void {
    this.updateData(p => {
      p.updateQuestionOptionValue(value, optionIndex);
    });
  }

  public onFreeTextQuestionCorrectAnswerChanged(value: string): void {
    this.updateData(p => {
      p.question_CorrectAnswer = value;
    });

    this.validate();
  }

  public onDatePickerQuestionCorrectAnswerChanged(value: Date): void {
    this.updateData(question => {
      question.question_CorrectAnswer = value;
    });

    this.validate();
  }

  public onDateRangePickerQuestionCorrectAnswerChanged(value: Date, isFromDate?: boolean): void {
    this.updateData(p => {
      if (!(p.question_CorrectAnswer instanceof Array)) {
        p.question_CorrectAnswer = new Array();
      }

      if (isFromDate) {
        p.question_CorrectAnswer[0] = value;
      } else {
        p.question_CorrectAnswer[1] = value;
      }
    });

    this.debounceValidate();
  }

  public onUpdateOrRemoveTextOption(textValue: AssignmentQuestionAnswerSingleValue, currentQuestionOptionIndex: number): void {
    this.updateData(async (data: QuizAssignmentFormQuestion) => {
      if (!textValue) {
        data.removeOption(currentQuestionOptionIndex);
        data.generateTitle();
        return;
      }
      data.updateQuestionOptionValue(textValue, currentQuestionOptionIndex);
      data.generateTitle();
    });

    this.validate();
  }

  public onUpdateOrRemoveBlankOption(blankValue: AssignmentQuestionAnswerSingleValue, currentQuestionOptionIndex: number): void {
    this.updateData(async (data: QuizAssignmentFormQuestion) => {
      if (!blankValue) {
        data.removeOption(currentQuestionOptionIndex);
        data.generateTitle();
        return;
      }
      data.updateQuestionOptionValue(blankValue, currentQuestionOptionIndex);
      data.generateTitle();
    });
  }

  public onAddNewTextOptionSubmit(textValue: AssignmentQuestionAnswerSingleValue): void {
    if (!textValue) {
      return;
    }
    this.updateData(async (data: QuizAssignmentFormQuestion) => {
      data.addTextOption(textValue);
      data.generateTitle();
    });
    this.validate();
  }

  public onAddNewBlankOptionSubmit(blankValue: AssignmentQuestionAnswerSingleValue): void {
    if (!blankValue) {
      return;
    }
    this.updateData(async (data: QuizAssignmentFormQuestion) => {
      data.addBlankOption(blankValue, null);
      data.generateTitle();
    });
    this.validate();
  }

  public onTrueFalseQuestionOptionCheckedChange(e: boolean, value: AssignmentQuestionAnswerSingleValue): void {
    if (e === true) {
      this.updateData(p => {
        p.question_CorrectAnswer = value;
      });
      this.validate();
    }
  }

  public onSingleChoiceQuestionOptionCheckedChange(e: boolean, value: AssignmentQuestionAnswerSingleValue): void {
    if (e === true) {
      this.updateData(p => {
        p.question_CorrectAnswer = value;
      });
      this.validate();
    }
  }

  public onMultipleChoiceQuestionOptionCheckedChange(e: boolean, value: string): void {
    this.updateData(p => {
      if (!(p.question_CorrectAnswer instanceof Array)) {
        p.question_CorrectAnswer = new Array();
      }

      if (e === false) {
        Utils.remove(p.question_CorrectAnswer, questionCorrectAnswerItem => questionCorrectAnswerItem === value);
      } else {
        p.question_CorrectAnswer.push(value);
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
    if (this.addNewOptionEditor) {
      this.addNewOptionEditor.focusValueInput();
      this.addNewOptionEditor.clearNewOptionValue();
    }
  }

  public getTrueFalseQuestionOptionLabel(optionValue: AssignmentQuestionAnswerSingleValue): string {
    return optionValue ? 'True' : 'False';
  }

  public updateData(updatefn: (data: QuizAssignmentFormQuestion) => void): void {
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

  public questionOptionTrackByFn(index: number, item: AssignmentQuestionOption): number {
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

  public canEditQuestion(): boolean {
    return this.mode === AssignmentMode.Edit;
  }

  public showQuestionHint(): boolean {
    return this.mode === AssignmentMode.LearnerAnswer && !Utils.isNullOrEmpty(this.data.question_Hint);
  }

  public showAnswerExplanatoryNote(): boolean {
    return (
      this.mode === AssignmentMode.LearnerAnswer &&
      !Utils.isNullOrEmpty(this.data.question_Hint) &&
      this.assignmentAnswerTrack.submitedDate != null
    );
  }

  public readOnlyQuestion(): boolean {
    return this.mode === AssignmentMode.ParticipantTrack || this.assignmentAnswerTrack.submitedDate != null;
  }

  public canShowQuestionTitle(): boolean {
    return !(this.data.question_Type === QuizAssignmentQuestionType.FillInTheBlanks);
  }

  public isDisabledQuestionDeleteBtn(): boolean {
    return false;
  }

  public getBlankOptionAnswerOfLearner(index: number): AssignmentQuestionAnswerSingleValue {
    if (this.mode === AssignmentMode.LearnerAnswer && this.data.question_CorrectAnswer != null) {
      const indexOfBlankOption = this.getBlankIndex(index);

      return this.data.question_CorrectAnswer[indexOfBlankOption];
    }

    return null;
  }

  public onOptionMediaChange(media: IOptionMedia, optionIndex: number): void {
    this.updateData(question => {
      question.addQuestionOptionMediaUrl(media, optionIndex);
    });
  }

  public onOptionFeedbackChange(feedbackValue: string, optionIndex: number): void {
    this.data = Utils.clone(this.data, p => {
      const currentQuestionOption = p.question_Options[optionIndex];
      currentQuestionOption.feedback = feedbackValue;
    });
    this.dataChangeEvent.emit(this.data);
  }

  public onFeedbackCorrectChange(feedbackCorrect: string): void {
    this.data = Utils.clone(this.data, p => {
      p.question_FeedbackCorrectAnswer = feedbackCorrect;
    });
    this.dataChangeEvent.emit(this.data);
  }

  public onFeedbackWrongChange(feedbackWrong: string): void {
    this.data = Utils.clone(this.data, p => {
      p.question_FeedbackWrongAnswer = feedbackWrong;
    });
    this.dataChangeEvent.emit(this.data);
  }

  public getCheckedQuestionOption(option: AssignmentQuestionOption): boolean {
    if (this.mode === AssignmentMode.ParticipantTrack) {
      return this.assignmentAnswerTrack.questionOptionCorrectDic[option.code] !== null;
    } else {
      return this.data.isOptionValueCorrect(option.value);
    }
  }

  public getCorrectQuestionOption(option: AssignmentQuestionOption): boolean {
    if (this.mode === AssignmentMode.ParticipantTrack) {
      return this.assignmentAnswerTrack.questionOptionCorrectDic[option.code];
    } else {
      return this.data.isOptionValueCorrect(option.value);
    }
  }

  public getAnswerForDateQuestion(isFrom: boolean = true): Date {
    if (this.mode === AssignmentMode.ParticipantTrack) {
      return this.getAnswerDate(this.assignmentAnswerTrack.questionAnswer, isFrom);
    } else {
      return this.getAnswerDate(this.data.question_CorrectAnswer, isFrom);
    }
  }

  public getAnswerForDropdownQuestion(): AssignmentQuestionAnswerValue {
    if (this.mode === AssignmentMode.ParticipantTrack) {
      return this.assignmentAnswerTrack.questionAnswer;
    } else {
      return this.data.question_CorrectAnswer;
    }
  }

  public canScoreGivingAssignment(): boolean {
    return ParticipantAssignmentTrack.hasScoreGivingAssignmentPermission(this.currentUser);
  }

  public isDateCorrectAnswer(isFrom: boolean = true): boolean {
    if (this.mode === AssignmentMode.ParticipantTrack) {
      if (this.data.question_Type === QuizAssignmentQuestionType.DatePicker) {
        const date = this.getAnswerDate(this.assignmentAnswerTrack.questionAnswer, isFrom);

        if (date == null) {
          return null;
        }
        return DateUtils.removeTime(date).getTime() === DateUtils.removeTime(this.data.question_CorrectAnswer as Date).getTime();
      } else if (this.data.question_Type === QuizAssignmentQuestionType.DateRangePicker) {
        if (isFrom === true) {
          const date = this.getAnswerDate(this.assignmentAnswerTrack.questionAnswer, true);

          if (date == null) {
            return null;
          }
          return DateUtils.removeTime(date).getTime() === DateUtils.removeTime(this.data.question_CorrectAnswer[0] as Date).getTime();
        } else {
          const date = this.getAnswerDate(this.assignmentAnswerTrack.questionAnswer, false);

          if (date == null) {
            return null;
          }
          return DateUtils.removeTime(date).getTime() === DateUtils.removeTime(this.data.question_CorrectAnswer[1] as Date).getTime();
        }
      }

      return false;
    }

    return true;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      validateByGroupControlNames:
        this.data.question_Type === QuizAssignmentQuestionType.DateRangePicker
          ? [['dateRangePickerFromCorrectAnswer', 'dateRangePickerToCorrectAnswer']]
          : null,
      controls: {
        title: {
          defaultValue: this.data.question_Title,
          validators: [
            {
              validator: requiredIfValidator(p => this.data.question_Type !== QuizAssignmentQuestionType.FillInTheBlanks),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p => this.data.question_Type !== QuizAssignmentQuestionType.FillInTheBlanks,
                () => Validators.maxLength(8000)
              ),
              validatorType: 'maxlength'
            }
          ]
        }
      }
    };

    switch (this.data.question_Type) {
      case QuizAssignmentQuestionType.FreeText:
        result.controls.freeTextCorrectAnswer = {
          defaultValue: this.data.question_CorrectAnswer,
          validators: [
            { validator: requiredIfValidator(p => this.assignmentType === AssignmentType.Quiz), validatorType: 'required' },
            { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
          ]
        };
        break;
      case QuizAssignmentQuestionType.DatePicker:
        result.controls.datePickerCorrectAnswer = {
          validators: [{ validator: requiredIfValidator(p => this.assignmentType === AssignmentType.Quiz), validatorType: 'required' }]
        };
        break;
      case QuizAssignmentQuestionType.DateRangePicker:
        result.controls.dateRangePickerFromCorrectAnswer = {
          validators: [
            {
              validator: ifValidator(
                p => this.assignmentType === AssignmentType.Quiz,
                () =>
                  startEndValidator(
                    'fromDateWithToDate',
                    p => p.value,
                    p => (this.data.question_CorrectAnswer ? this.data.question_CorrectAnswer[1] : null),
                    true,
                    'dateOnly'
                  )
              ),
              validatorType: 'fromDateWithToDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be greater than To Date')
            }
          ]
        };
        result.controls.dateRangePickerToCorrectAnswer = {
          validators: [
            {
              validator: ifValidator(
                p => this.assignmentType === AssignmentType.Quiz,
                () =>
                  startEndValidator(
                    'toDateWithFromDate',
                    p => (this.data.question_CorrectAnswer ? this.data.question_CorrectAnswer[0] : null),
                    p => p.value,
                    true,
                    'dateOnly'
                  )
              ),
              validatorType: 'toDateWithFromDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be less than From Date')
            }
          ]
        };
        break;
    }
    return result;
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
    this.originalQuestionTitle = this.data.question_Title;

    this.imageUploadSettings.allowedUploadImage =
      this.data.question_Type === (QuizAssignmentQuestionType.SingleChoice || QuizAssignmentQuestionType.MultipleChoice);
  }

  private anyLogicError(): ValidationErrors | undefined {
    this.logicValidationErrors = QuizAssignmentFormQuestion.validate(this.data, this.assignmentType);
    return this.logicValidationErrors;
  }

  private getAnswerDate(answer: AssignmentQuestionAnswerValue, isFrom: boolean): Date {
    if (this.data.question_Type === QuizAssignmentQuestionType.DatePicker) {
      return answer != null ? (answer as Date) : null;
    } else if (this.data.question_Type === QuizAssignmentQuestionType.DateRangePicker) {
      if (isFrom) {
        return answer != null && answer[0] != null ? answer[0] : null;
      } else {
        return answer != null && answer[1] != null ? answer[1] : null;
      }
    }

    return null;
  }

  private getBlankIndex(index: number): number {
    let indexOfBlankOption = -1;
    for (let i = 0; i < index; i++) {
      if (this.data.question_Options[i].type === AssignmentQuestionOptionType.Blank) {
        indexOfBlankOption++;
      }
    }

    return indexOfBlankOption + 1;
  }
}
