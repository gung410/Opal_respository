import { AnswerFeedbackDialogComponent, PersonalFileDialogComponent } from '@opal20/domain-components';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, UploadParameters, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, HostBinding, Input, Output, ViewChild } from '@angular/core';
import { FileUploaderUtils, OpalDialogService, requiredIfValidator } from '@opal20/common-components';
import { FormType, IOptionMedia, MediaType, QuestionAnswerSingleValue, QuestionOption, QuestionType } from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';

@Component({
  selector: 'form-question-option-editor',
  templateUrl: './form-question-option-editor.component.html'
})
export class FormQuestionOptionEditorComponent extends BaseFormComponent {
  public readonly formTypeEnum: typeof FormType = FormType;
  public questionTypeEnum: typeof QuestionType = QuestionType;
  public mediaTypeEnum: typeof MediaType = MediaType;
  public isEmpty: boolean = false;

  public readonly mediaPath: string = `${AppGlobal.environment.cloudfrontUrl}/`;

  @ViewChild('valueInput', { static: false }) public valueInput: ElementRef<HTMLInputElement>;

  @ViewChild('videoElement', { static: false }) public videoElement: ElementRef<HTMLVideoElement>;

  @Input() public checked: boolean = false;
  @Input() public type: 'radio' | 'checkbox' = 'checkbox';
  @Input() public readonly: boolean = false;
  @Input() public value: QuestionAnswerSingleValue;
  @Input() public noRemove: boolean = false;
  @Input() public valueInputPlaceholder: string = '';
  @Input() public notRequired: boolean = false;
  @Input() public disableSetCorrectAnswer: boolean = false;
  @Input() public formType: FormType;
  @Input() public questionType?: string = '';

  @Input() public visibleOptionActions: boolean = false;

  @Input() public allowConfigureImage: boolean = true;

  @Input() public allowConfigureFeedback: boolean = false;
  @Input() public feedback: string = '';

  @Input() public allowConfigureBranching: boolean = false;
  @Input() public branchingOptions?: IDataItem[] = [];
  @Input() public isViewMode = false;
  // The next question id for the current question option.
  @Input() public nextQuestionId?: string = null;

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

  // These functional to handle for question type Single and Multiple choise
  @Output('updateStateWithoutValue') public updateStateWithoutValueEvent: EventEmitter<boolean> = new EventEmitter();
  @Output('optionMediaChangeWithoutValue') public optionMediaChangeWithoutValueEvent: EventEmitter<IOptionMedia> = new EventEmitter();
  @Output('removeCorrectAnswerWithEmptyValue') public removeCorrectAnswerWithEmptyValueEvent: EventEmitter<boolean> = new EventEmitter();

  @HostBinding('class.value-input-focused') public valueInputFocused: boolean = false;

  public _questionOption: QuestionOption;

  constructor(moduleFacadeService: ModuleFacadeService, public opalDialogService: OpalDialogService) {
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
    this.checked = !this.checked;
    this.checkedChangeEvent.emit(this.checked);
  }

  public onValueChanged(newValue: QuestionAnswerSingleValue): void {
    this.valueChangeEvent.emit(newValue);
  }

  public onRemoveClicked(e: MouseEvent): void {
    this.removeEvent.emit(e);
  }

  public onValueInputFocus(e: Event): void {
    // if this option is image only option
    if (this._questionOption && this.allowConfigureImage && this._questionOption.isEmptyValue) {
      // Empty value on focus input to add text value
      this.updateStateWithoutValueEvent.emit(false);
    }
    this.valueInputFocused = true;
    this.valueInputFocusEvent.emit(e);
  }

  public onValueInputFocusOut(optionValue: string): void {
    if (!Utils.isNullOrEmpty(optionValue)) {
      // if have input value and there is image in option
      if (this.allowConfigureImage && this._questionOption && (this._questionOption.imageUrl || this._questionOption.videoUrl)) {
        // remove current generated value if have
        this.removeCorrectAnswerWithEmptyValueEvent.emit(true);
      }
    } else {
      if (this.allowConfigureImage && this._questionOption && (this._questionOption.imageUrl || this._questionOption.videoUrl)) {
        this.updateStateWithoutValueEvent.emit(true);
      }
    }

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
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFileDialogComponent);
    const configurationPopup = dialogRef.content.instance as PersonalFileDialogComponent;
    configurationPopup.uploadFolder = 'form-question-option';
    configurationPopup.maxFileCount = 1;
    configurationPopup.icon = 'assets/images/icons/add-file.svg';
    const extensions: string[] = [...FileUploaderUtils.videoExtensions, ...FileUploaderUtils.graphicsExtensions];
    configurationPopup.filterByExtensions = extensions;
    configurationPopup.settings.extensions = extensions;
    dialogRef.result.toPromise().then((uploadParameters: UploadParameters[]) => {
      let mediaType: MediaType;
      switch (uploadParameters[0].fileExtension) {
        case 'mp4':
        case 'm4v':
        case 'ogv':
          mediaType = MediaType.Video;
          break;
        default:
          mediaType = MediaType.Image;
          break;
      }
      const media: IOptionMedia = {
        type: mediaType,
        src: uploadParameters[0].fileLocation
      };
      if (!this.value && this._questionOption) {
        this.optionMediaChangeWithoutValueEvent.emit(media);
        this.valueInput.nativeElement.blur();
        return;
      }
      this.optionMediaChangeEvent.emit(media);
    });
  }

  public openQuestionConfigurationPopup(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: AnswerFeedbackDialogComponent });
    const configurationPopup = dialogRef.content.instance as AnswerFeedbackDialogComponent;

    configurationPopup.feedback = this.feedback;
    configurationPopup.nextQuestionId = this.nextQuestionId;
    configurationPopup.branchingOptions = this.branchingOptions;
    configurationPopup.allowConfigureFeedback = this.allowConfigureFeedback;
    configurationPopup.allowConfigureBranching = this.allowConfigureBranching;
    configurationPopup.isViewMode = this.isViewMode;

    dialogRef.result
      .toPromise()
      .then(
        (data: { action: DialogFeedbackAction; feedback: ''; showDeleteButton?: boolean; event?: MouseEvent; nextQuestionId?: string }) => {
          if (data) {
            switch (data.action) {
              case DialogFeedbackAction.Submit:
                this.feedbackChangeEvent.emit(data.feedback);
                this.nextQuestionOptionChangeEvent.emit(data.nextQuestionId);
                break;
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

    if (this._questionOption && this._questionOption.isEmptyValue) {
      this.updateStateWithoutValueEvent.emit(false);
      this.removeCorrectAnswerWithEmptyValueEvent.emit(true);
      this.valueInput.nativeElement.focus();
    }

    this.optionMediaChangeEvent.emit(media);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      controls: {
        value: {
          defaultValue: this.value,
          validators: [
            {
              validator: requiredIfValidator(p => this.isTitleRequired()),
              validatorType: 'required'
            },
            { validator: Validators.maxLength(8000), validatorType: 'maxLength' }
          ]
        },
        checked: {
          defaultValue: this.checked
        }
      }
    };
    return result;
  }

  private isTitleRequired(): boolean {
    return !this.notRequired && this._questionOption && !this._questionOption.imageUrl && !this._questionOption.videoUrl;
  }
}

export enum DialogFeedbackAction {
  Submit = 'submit',
  Close = 'close'
}
