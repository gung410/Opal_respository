import { Assignment } from '../models/assignment.model';
import { AssignmentType } from '../models/assignment-type.model';
import { IQuizAssignmentFormQuestion } from '../models/quiz-assignment-form-question.model';
import { ScoreMode } from '../models/score-mode.model';

export interface ISaveAssignmentRequest {
  data: SaveAssignmentRequestData;
}

export class SaveAssignmentRequestData {
  public id?: string;
  public courseId: string;
  public classRunId?: string;
  public title: string = '';
  public type: AssignmentType = AssignmentType.Quiz;
  public quizForm?: ISaveAssignmentRequestDataQuizForm;
  public assessmentId?: string;
  public scoreMode?: ScoreMode;

  constructor(data?: Assignment, quizForm?: ISaveAssignmentRequestDataQuizForm) {
    if (data != null) {
      this.id = data.id;
      this.courseId = data.courseId;
      this.classRunId = data.classRunId;
      this.title = data.title;
      this.type = data.type;
      this.quizForm = quizForm;
      this.assessmentId = data.assessmentConfig ? data.assessmentConfig.assessmentId : null;
      this.scoreMode = data.assessmentConfig ? data.assessmentConfig.scoreMode : null;
    }
  }
}

export interface ISaveAssignmentRequestDataQuizForm {
  randomizedQuestions: boolean;
  questions?: IQuizAssignmentFormQuestion[];
}
