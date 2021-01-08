import { FormQuestionModelAnswerValue } from '../models/form-question.model';
import { PersonalFileModel } from '../../personal-space/models/personal-file.model';

export interface IUpdateFormAnswerRequest {
  formAnswerId: string;
  myCourseId?: string;
  questionAnswers?: IUpdateFormQuestionAnswerRequest[];
  isSubmit?: boolean;
}

export interface ISaveFormAnswer {
  formId: string;
  myCourseId: string;
  courseId?: string;
  classRunId?: string;
  assignmentId?: string;
}

export interface IUpdateFormQuestionAnswerRequest {
  formQuestionId: string;
  answerValue?: FormQuestionModelAnswerValue;
  isSubmit?: boolean;
  spentTimeInSeconds?: number;
  formAnswerAttachments?: PersonalFileModel[];
  markedScore?: number;
}
