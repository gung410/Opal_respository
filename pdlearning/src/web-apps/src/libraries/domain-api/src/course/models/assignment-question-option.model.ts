import { AssignmentQuestionAnswerSingleValue } from './quiz-assignment-form-question.model';
import { IValidatorDefinition } from '@opal20/infrastructure';
import { Validators } from '@angular/forms';

export interface IAssignmentQuestionOption {
  code: number;
  value: AssignmentQuestionAnswerSingleValue;
  feedback: string;
  type: AssignmentQuestionOptionType;
  imageUrl: string;
  videoUrl: string;
}

export class AssignmentQuestionOption implements IAssignmentQuestionOption {
  public code: number;
  public value: AssignmentQuestionAnswerSingleValue;
  public feedback: string;
  public type: AssignmentQuestionOptionType = AssignmentQuestionOptionType.Text;
  public imageUrl: string;
  public videoUrl: string | undefined;

  public static validators(): IValidatorDefinition[] {
    return [
      { validator: Validators.required, validatorType: 'required' },
      { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
    ];
  }

  constructor(data?: IAssignmentQuestionOption, isDateQuestion: boolean = false) {
    if (data != null) {
      this.code = data.code;
      this.value = isDateQuestion === true ? new Date(data.value.toString()) : data.value;
      this.feedback = data.feedback;
      this.type = data.type;
      this.imageUrl = data.imageUrl;
      this.videoUrl = data.videoUrl;
    }
  }
}

export enum AssignmentQuestionOptionType {
  Text = 'Text',
  Blank = 'Blank'
}
