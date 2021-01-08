import { IValidatorDefinition } from '@opal20/infrastructure';
import { StandaloneSurveyQuestionAnswerSingleValue } from './form-question.model';
import { StandaloneSurveyQuestionOptionType } from './question-option-type.model';
import { Validators } from '@angular/forms';

export class StandaloneSurveyQuestionOption {
  public code: number;
  public value: StandaloneSurveyQuestionAnswerSingleValue;
  public type: StandaloneSurveyQuestionOptionType | undefined;
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

  public static ValueValidators(): IValidatorDefinition[] {
    return [
      { validator: Validators.required, validatorType: 'required' },
      { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
    ];
  }

  constructor(
    code: number,
    value: StandaloneSurveyQuestionAnswerSingleValue,
    type: StandaloneSurveyQuestionOptionType | null = null,
    imageUrl: string = null,
    videoUrl: string = null,
    isEmptyValue: boolean = false
  ) {
    this.code = code;
    this.value = value;
    this.type = type;
    this.imageUrl = imageUrl;
    this.videoUrl = videoUrl;
    this.isEmptyValue = isEmptyValue;
  }
}

export interface IStandaloneSurveyOptionMedia {
  type: StandaloneSurveyMediaType | string;
  src: string;
}

export enum StandaloneSurveyMediaType {
  Image = 'image',
  Video = 'video'
}
