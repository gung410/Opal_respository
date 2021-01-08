import { IValidatorDefinition } from '@opal20/infrastructure';
import { QuestionAnswerSingleValue } from './form-question.model';
import { QuestionOptionType } from './question-option-type.model';
import { Validators } from '@angular/forms';

export class QuestionOption {
  public code: number;
  public value: QuestionAnswerSingleValue;
  public feedback: string | undefined;
  public type: QuestionOptionType | undefined;
  public imageUrl: string | undefined;
  public videoUrl: string | undefined;
  public isEmptyValue: boolean | false;
  /**
   * Statistics for question answers.
   */
  public answerCount: number;
  public answerPercentage: number;
  /**
   * The next question id for the question option.
   */
  public nextQuestionId: string | undefined;
  public scaleId: string | undefined;

  public static ValueValidators(): IValidatorDefinition[] {
    return [
      { validator: Validators.required, validatorType: 'required' },
      { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
    ];
  }

  constructor(
    code: number,
    value: QuestionAnswerSingleValue,
    feedback: string = null,
    type: QuestionOptionType | null = null,
    imageUrl: string = null,
    videoUrl: string = null,
    isEmptyValue: boolean = false,
    scaleId: string = null
  ) {
    this.code = code;
    this.value = value;
    this.feedback = feedback;
    this.type = type;
    this.imageUrl = imageUrl;
    this.videoUrl = videoUrl;
    this.isEmptyValue = isEmptyValue;
    this.scaleId = scaleId;
  }
}

export interface IOptionMedia {
  type: MediaType | string;
  src: string;
}

export enum MediaType {
  Image = 'image',
  Video = 'video'
}
