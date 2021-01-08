import {
  AnswerFeedbackDisplayOption,
  FileType,
  FormQuestionAnswerModel,
  FormQuestionAnswerService,
  FormQuestionModel,
  FormQuestionModelSingleAnswerValue,
  FormType,
  FormWithQuestionsModel,
  PersonalFileModel,
  QuestionAnswerSingleValue,
  QuestionOption,
  QuestionOptionType,
  QuestionType
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService, UploadParameters, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { FileUploaderUtils, OpalDialogService, PlatformHelper } from '@opal20/common-components';

import { BatchUploadFilesDialogComponent } from '../batch-upload-files-dialog/batch-upload-files-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { MainQuizPlayerPageService } from '../../services/main-quiz-player-page.service';
import { shuffle } from 'lodash-es';
import { FileUploaderHelpers } from '../../helpers/file-uploader.helper';

@Component({
  selector: 'form-question-answer-player',
  templateUrl: './form-question-answer-player.component.html'
})
export class FormQuestionAnswerPlayerComponent extends BaseComponent {
  public readonly questionOptionTypeEnum: typeof QuestionOptionType = QuestionOptionType;
  public readonly questionType: typeof QuestionType = QuestionType;
  public readonly formType: typeof FormType = FormType;
  public renderQuestionOptions: QuestionOption[] | undefined;
  public displayAnswerResultToaster: boolean = false;
  public feedbackOnSelectedOption: string;
  public safeQuestionTitle: SafeHtml;
  public isShowMessagePreventCookie: boolean = false;

  @Input() public formData: FormWithQuestionsModel;
  @Input() public positionInForm: string | number = 1;
  public get optionCodeOrderList(): string[] | undefined {
    return this._optionCodeOrderList;
  }
  @Input() public isPreviewMode: boolean = false;

  @Output('questionAnswerChange') public questionAnswerChangeEvent: EventEmitter<FormQuestionAnswerModel> = new EventEmitter();
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
  protected _visualQuestionOptionAnswers: FormQuestionModelSingleAnswerValue[];

  private _question: FormQuestionModel;
  private _optionCodeOrderList: string[] | undefined;
  private _questionAnswer: FormQuestionAnswerModel;

  constructor(
    private mainQuizPlayerService: MainQuizPlayerPageService,
    private sanitizer: DomSanitizer,
    protected formQuestionAnswerService: FormQuestionAnswerService,
    moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  @Input()
  public set question(v: FormQuestionModel) {
    if (this._question && this._question.id === v.id) {
      return;
    }

    this._question = new FormQuestionModel(JSON.parse(JSON.stringify(v)));
    this.displayAnswerResultToaster = false;
    if (this._visualQuestionOptionAnswers && this._visualQuestionOptionAnswers.length) {
      this._visualQuestionOptionAnswers = [];
    }
    if (this._question.randomizedOptions) {
      this._question.questionOptions = shuffle(this._question.questionOptions);
    }
  }
  public get question(): FormQuestionModel {
    return this._question;
  }

  @Input()
  public set questionAnswer(v: FormQuestionAnswerModel) {
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
  public get questionAnswer(): FormQuestionAnswerModel {
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

  public isMultipleChoiceAnswerMatch(model: FormQuestionAnswerModel, option: QuestionAnswerSingleValue): boolean {
    return FormQuestionAnswerModel.isMultipleChoiceAnswerMatch(model, option);
  }

  public getFeedbackOnSelectedOption(value: FormQuestionModelSingleAnswerValue): string {
    switch (this._question.questionType) {
      case QuestionType.TrueFalse: {
        const isCorrectOption = this._question.isOptionValueCorrect(value);
        if (!isCorrectOption) {
          return this._question.feedbackWrongAnswer;
        }
        return this._question.feedbackCorrectAnswer;
      }
      case QuestionType.DropDown:
      case QuestionType.SingleChoice: {
        const selectedOption = this._question.questionOptions.find(p => p.value === value);
        return selectedOption.feedback;
      }
      default: {
        return '';
      }
    }
  }

  public updateFormQuestionAnswer(updatefn: (data: FormQuestionAnswerModel) => void): void {
    this.questionAnswer = Utils.clone(this.questionAnswer, p => {
      updatefn(p);
    });
    this.questionAnswerChangeEvent.emit(this.questionAnswer);
  }

  public onFillInTheBlankQuestionAnswerSingleValueFilledUp(value: FormQuestionModelSingleAnswerValue, currentOptionIndex: number): void {
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

  public onTrueFalseQuestionOptionSelected(value: FormQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
    this.feedbackOnSelectedOption = this.getFeedbackOnSelectedOption(value);
  }

  public onSingleChoiceQuestionOptionSelected(value: FormQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
    this.feedbackOnSelectedOption = this.getFeedbackOnSelectedOption(value);
  }

  public onDropDownListSelected(value: FormQuestionModelSingleAnswerValue): void {
    this.updateFormQuestionAnswer(p => {
      p.answerValue = value;
    });
    this.feedbackOnSelectedOption = this.getFeedbackOnSelectedOption(value);
  }

  public onMultipleChoiceQuestionOptionSelected(value: FormQuestionModelSingleAnswerValue): void {
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

  public getTrueFalseQuestionOptionLabel(optionValue: FormQuestionModelSingleAnswerValue): string {
    return optionValue ? 'True' : 'False';
  }

  public disabledAttachFile(): boolean {
    return this.questionAnswer.formAnswerAttachments && this.questionAnswer.formAnswerAttachments.length >= 10;
  }

  public uploadAnswerAttachmentFiles(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(BatchUploadFilesDialogComponent);
    const configurationPopup = dialogRef.content.instance as BatchUploadFilesDialogComponent;
    const currentAttachment = this.questionAnswer.formAnswerAttachments;
    const fileNumber = currentAttachment ? 10 - currentAttachment.length : 10;

    configurationPopup.uploadFolder = 'form-attachments';
    configurationPopup.isPreviewMode = this.isPreviewMode;
    configurationPopup.icon = 'assets/images/icons/add-file.svg';
    configurationPopup.settings.extensions = FileUploaderUtils.allowedAnswerAttachmentExts;
    configurationPopup.fileNumber = fileNumber;

    dialogRef.result.subscribe((result: UploadParameters[]) => {
      if (result && result.length > 0) {
        const fileAttachments = this.createFileAttachments(result);
        this.updateFormAnswerAttachment(fileAttachments);
      }
    });
  }

  public deleteFormAnswerAttachment(location: string): void {
    this.updateFormQuestionAnswer(p => {
      p.formAnswerAttachments = p.formAnswerAttachments.filter(file => file.fileLocation !== location);
    });
  }

  public get isFeedbackShowed(): boolean {
    if (this.formData.form.type !== this.formType.Quiz || !this.feedbackOnSelectedOption) {
      return false;
    }
    switch (this._question.questionType) {
      case QuestionType.DropDown:
      case QuestionType.TrueFalse:
      case QuestionType.SingleChoice: {
        return true;
      }
      default: {
        return false;
      }
    }
  }

  public get isExplationNoteShowed(): boolean {
    return FormQuestionModel.questionTypeToShowExplationNote.includes(this._question.questionType);
  }

  public canShowFeedbackContainer(): boolean {
    if (this.formData.form.answerFeedbackDisplayOption === AnswerFeedbackDisplayOption.AfterAnsweredQuestion) {
      return this.questionAnswer.submittedDate && this.feedbackOnSelectedOption !== '';
    }
  }

  public canShowExplationNoteContainer(): boolean {
    return !Utils.isNullOrEmpty(this.question.answerExplanatoryNote) && this.questionAnswer.submittedDate !== undefined;
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

  public handleImgError(event: Event): void {
    this.isShowMessagePreventCookie = event.type === 'error' && PlatformHelper.isIOSDevice() ? true : false;
  }

  public canShowVideo(fileExtension: string): boolean {
    return FileUploaderHelpers.getFileType(fileExtension) === FileType.Video;
  }

  public canShowAudio(fileExtension: string): boolean {
    return FileUploaderHelpers.getFileType(fileExtension) === FileType.Audio;
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

  private getRenderQuestionOptions(): QuestionOption[] | undefined {
    if (!this.question.questionOptions || !this.optionCodeOrderList) {
      return this.question.questionOptions;
    }
    const questionOptionsDic = Utils.toDictionary(this.question.questionOptions, p => p.code);
    return this.optionCodeOrderList.map(code => questionOptionsDic[code]);
  }

  private createFileAttachments(fileParameter: UploadParameters[]): PersonalFileModel[] {
    const toUploadFileParams = fileParameter.map(item => {
      return <PersonalFileModel>{
        id: this.questionAnswer.id,
        fileName: item.fileName,
        fileType: FileUploaderHelpers.getFileType(item.fileExtension),
        fileSize: item.fileSize,
        fileExtension: item.fileExtension,
        fileLocation: item.fileLocation
      };
    });

    return toUploadFileParams;
  }

  private updateFormAnswerAttachment(attachmentFiles: PersonalFileModel[]): void {
    this.updateFormQuestionAnswer(p => {
      if (p.formAnswerAttachments === undefined) {
        p.formAnswerAttachments = [];
      }

      p.formAnswerAttachments = p.formAnswerAttachments.concat(attachmentFiles);
    });
  }
}

function createDebounceFn(func: (...args: unknown[]) => void): (...args: unknown[]) => void {
  return Utils.debounce(func, 300);
}
