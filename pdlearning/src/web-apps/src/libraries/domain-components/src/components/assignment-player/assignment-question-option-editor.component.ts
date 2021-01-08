import {
  AssignmentQuestionOption,
  AssignmentType,
  IOptionMedia,
  MediaType,
  QuestionAnswerSingleValue,
  QuestionOption,
  QuizAssignmentQuestionType
} from '@opal20/domain-api';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, HostBinding, Input, Output, ViewChild } from '@angular/core';

import { AnswerFeedbackDialogComponent } from '../answer-feedback-dialog/answer-feedback-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { QuestionOptionImageUploadDialogComponent } from '../question-option-image-upload-dialog/question-option-image-upload-dialog.component';
import { QuestionOptionImageUploadSettings } from '../../models/question-option-image-upload-setting.model';

@Component({
  selector: 'assignment-question-option-editor',
  templateUrl: './assignment-question-option-editor.component.html'
})
export class AssignmentQuestionOptionEditorComponent extends BaseFormComponent {
  public AssignmentType: typeof AssignmentType = AssignmentType;
  public QuizAssignmentQuestionType: typeof QuizAssignmentQuestionType = QuizAssignmentQuestionType;
  public MediaType: typeof MediaType = MediaType;

  public readonly mediaPath: string = `${AppGlobal.environment.cloudfrontUrl}/`;

  @ViewChild('valueInput', { static: false }) public valueInput: ElementRef<HTMLInputElement>;
  @ViewChild('videoElement', { static: false }) public videoElement: ElementRef<HTMLVideoElement>;

  @Input() public checked: boolean = false;
  @Input() public type: 'radio' | 'checkbox' = 'checkbox';
  @Input() public viewMode: boolean = false;
  @Input() public value: QuestionAnswerSingleValue;
  @Input() public noRemove: boolean = false;
  @Input() public valueInputPlaceholder: string = '';
  @Input() public notRequired: boolean = false;
  @Input() public disableSetCorrectAnswer: boolean = false;
  @Input() public assignmentType: AssignmentType;
  @Input() public imageUploadSettings: QuestionOptionImageUploadSettings = new QuestionOptionImageUploadSettings();
  @Input() public feedback: string = '';
  @Input() public questionType?: string = '';
  @Input() public allowConfigureImage: boolean = true;
  @Input() public allowConfigureFeedback: boolean = false;
  @Input() public nextQuestionId?: string = null;
  @Input() public visibleOptionActions: boolean = false;

  @Input() public set questionOption(value: QuestionOption) {
    this._questionOption = value;
    if (this.videoElement) {
      this.videoElement.nativeElement.load();
    }
  }
  public get questionOption(): QuestionOption {
    return this._questionOption;
  }

  @Output('valueChange') public valueChangeEvent: EventEmitter<QuestionAnswerSingleValue> = new EventEmitter();
  @Output('checkedChange') public checkedChangeEvent: EventEmitter<boolean> = new EventEmitter();
  @Output('remove') public removeEvent: EventEmitter<MouseEvent> = new EventEmitter();
  @Output('valueInputFocus') public valueInputFocusEvent: EventEmitter<Event> = new EventEmitter();
  @Output('valueInputFocusOut') public valueInputFocusOutEvent: EventEmitter<string> = new EventEmitter();
  @Output('optionMediaChange') public optionMediaChangeEvent: EventEmitter<IOptionMedia> = new EventEmitter();
  @Output('feedbackChange') public feedbackChangeEvent: EventEmitter<string> = new EventEmitter();
  @Output('nextQuestionOptionChange') public nextQuestionOptionChangeEvent: EventEmitter<string> = new EventEmitter();

  @HostBinding('class.value-input-focused') public valueInputFocused: boolean = false;

  private _questionOption: QuestionOption;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get mediaUrl(): string {
    if (!this._questionOption) {
      return '';
    }
    return this._questionOption.imageUrl ? this._questionOption.imageUrl : this._questionOption.videoUrl;
  }

  public get mediaSource(): string {
    return this.mediaPath + this.mediaUrl;
  }

  public get mediaType(): MediaType | string {
    if (!this._questionOption) {
      return '';
    }
    return this._questionOption.videoUrl ? MediaType.Video : MediaType.Image;
  }
  public onCheckboxClicked(e: MouseEvent): void {
    if (this.disableSetCorrectAnswer) {
      e.preventDefault();
      return;
    }

    const newCheckedValue = this.type === 'radio' ? true : !this.checked;
    if (newCheckedValue !== this.checked) {
      this.checked = newCheckedValue;
      this.checkedChangeEvent.emit(this.checked);
    } else {
      e.preventDefault();
    }
  }

  public onValueChanged(newValue: QuestionAnswerSingleValue): void {
    if (newValue !== this.value) {
      this.value = newValue;
      this.valueChangeEvent.emit(newValue);
    }
  }

  public onRemoveClicked(e: MouseEvent): void {
    this.removeEvent.emit(e);
  }

  public onValueInputFocus(e: Event): void {
    this.valueInputFocused = true;
    this.valueInputFocusEvent.emit(e);
  }

  public onValueInputFocusOut(optionValue: string): void {
    this.valueInputFocused = false;
    this.valueInputFocusOutEvent.emit(optionValue);
  }

  public onValueInputEnter(): void {
    this.valueInput.nativeElement.blur();
  }

  public focusValueInput(): void {
    this.valueInput.nativeElement.focus();
  }

  public clearNewOptionValue(): void {
    this.valueInput.nativeElement.value = '';
  }

  public insertImage(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: QuestionOptionImageUploadDialogComponent });
    dialogRef.result.toPromise().then((media: IOptionMedia) => {
      this.optionMediaChangeEvent.emit(media);
    });
  }

  public openQuestionConfigurationPopup(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: AnswerFeedbackDialogComponent });
    const configurationPopup = dialogRef.content.instance as AnswerFeedbackDialogComponent;

    configurationPopup.feedback = this.feedback;
    configurationPopup.nextQuestionId = this.nextQuestionId;
    configurationPopup.allowConfigureFeedback = this.allowConfigureFeedback;

    dialogRef.result
      .toPromise()
      .then(
        (data: { action: DialogFeedbackAction; feedback: ''; showDeleteButton?: boolean; event?: MouseEvent; nextQuestionId?: string }) => {
          if (data) {
            switch (data.action) {
              case DialogFeedbackAction.Submit: {
                this.feedbackChangeEvent.emit(data.feedback);
                this.nextQuestionOptionChangeEvent.emit(data.nextQuestionId);
                break;
              }
              default:
                break;
            }
          }
        }
      );
  }

  public deleteImage(): void {
    const media: IOptionMedia = {
      type: this.mediaType,
      src: ''
    };
    this.optionMediaChangeEvent.emit(media);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      controls: {
        value: {
          defaultValue: this.value,
          validators: this.notRequired
            ? AssignmentQuestionOption.validators().filter(p => p.validatorType !== 'required')
            : AssignmentQuestionOption.validators()
        },
        checked: {
          defaultValue: this.checked
        }
      }
    };
    return result;
  }
}

export enum DialogFeedbackAction {
  Submit = 'submit',
  Delete = 'delete',
  Close = 'close'
}
