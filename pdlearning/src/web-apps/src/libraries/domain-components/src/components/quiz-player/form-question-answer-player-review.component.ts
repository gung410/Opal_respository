import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  FileType,
  FormAnswerModel,
  FormQuestionAnswerModel,
  FormQuestionModel,
  FormQuestionModelSingleAnswerValue,
  FormSectionViewModel,
  FormType,
  PersonalFileModel,
  QuestionAnswerSingleValue,
  QuestionOptionType,
  QuestionType
} from '@opal20/domain-api';
import { PlatformHelper } from '@opal20/common-components';

import { DomSanitizer } from '@angular/platform-browser';
import { FileUploaderHelpers } from '../../helpers/file-uploader.helper';

@Component({
  selector: 'form-question-answer-player-review',
  templateUrl: './form-question-answer-player-review.component.html'
})
export class FormQuestionAnswerPlayerReviewComponent extends BaseComponent {
  @Input() public formQuestion: FormQuestionModel[];
  @Input() public formAnswer: FormAnswerModel;
  @Input() public formDataType: FormType;
  @Input() public sectionsQuestion: FormSectionViewModel[];

  @Output('exit') public exitEvent: EventEmitter<unknown> = new EventEmitter<unknown>();

  public readonly questionOptionTypeEnum: typeof QuestionOptionType = QuestionOptionType;
  public questionType = QuestionType;
  public questionIdToFormAnswerDic: Dictionary<FormQuestionAnswerModel | undefined> = {};
  public readonly formType = FormType;
  public isShowMessagePreventCookie: boolean = false;

  constructor(private sanitizer: DomSanitizer, moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.sectionsQuestion.forEach(section => {
      section.questions.forEach(question => {
        question.questionTitleSafeHtml = this.sanitizer.bypassSecurityTrustHtml(question.questionTitle);
      });
    });
  }

  public getCurrentQuestionAnswer(id: string): FormQuestionAnswerModel | undefined {
    return this.questionIdToFormAnswerDic[id];
  }

  public getTrueFalseQuestionOptionLabel(optionValue: FormQuestionModelSingleAnswerValue): string {
    return optionValue ? 'True' : 'False';
  }

  public getMediaUrl(meidaUrl: string): string {
    return meidaUrl ? `${AppGlobal.environment.cloudfrontUrl}/${meidaUrl}` : '';
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

  public getAnswerValue(id: string): string {
    return this.formAnswer.questionAnswers.find(x => x.formQuestionId === id).answerValue.toString();
  }

  public getFormAnswerAttachment(id: string): PersonalFileModel[] {
    return this.formAnswer.questionAnswers.find(x => x.formQuestionId === id).formAnswerAttachments;
  }

  public getQuestionAnswer(id: string): FormQuestionAnswerModel {
    return this.formAnswer.questionAnswers.find(x => x.formQuestionId === id);
  }

  public getDateRangePickerAnswer(id: string, index: number): Date {
    return new Date(this.getQuestionAnswer(id).answerValue[index]);
  }

  public getDatePickerAnswer(id: string): Date {
    const dateAnswer = (this.getQuestionAnswer(id).answerValue as unknown) as Date;
    return new Date(dateAnswer);
  }

  public isShortTextAnswerCorrect(answerValue: string, answerCorrectValue: string): boolean {
    return answerValue && answerCorrectValue
      ? answerValue.toLowerCase().trim() === answerCorrectValue.toLowerCase().trim() && this.formDataType === FormType.Quiz
      : false;
  }

  public isShortTextAnswerIncorrect(answerValue: string, answerCorrectValue: string): boolean {
    return answerValue && answerCorrectValue
      ? !(answerValue.toLowerCase().trim() === answerCorrectValue.toLowerCase().trim()) && this.formDataType === FormType.Quiz
      : false;
  }

  public isMultipleChoiceAnswerMatch(model: FormQuestionAnswerModel, option: QuestionAnswerSingleValue): boolean {
    return model.answerValue !== undefined && model.answerValue instanceof Array && model.answerValue.indexOf(<never>option) >= 0;
  }

  public isCheckOptionValueCorrect(option: FormQuestionModel, value: string): boolean {
    return option.isOptionValueCorrect(value) && this.formDataType === FormType.Quiz;
  }

  public isCheckOptionValueIncorrect(option: FormQuestionModel, value: string): boolean {
    return !option.isOptionValueCorrect(value) && this.formDataType === FormType.Quiz;
  }

  public isCheckAnswerCorrect(option: FormQuestionModel): boolean {
    return option.isAnswerCorrect(this.getQuestionAnswer(option.id).answerValue) && this.formDataType === FormType.Quiz;
  }

  public isCheckAnswerIncorrect(option: FormQuestionModel): boolean {
    return !option.isAnswerCorrect(this.getQuestionAnswer(option.id).answerValue) && this.formDataType === FormType.Quiz;
  }

  public onClickExit(): void {
    this.exitEvent.emit();
  }
}
