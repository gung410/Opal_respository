import { ScoreMode } from './score-mode.model';

export interface IAssignmentAssessmentConfig {
  assessmentId: string;
  numberAutoAssessor: number;
  scoreMode: ScoreMode;
}

export class AssignmentAssessmentConfig implements IAssignmentAssessmentConfig {
  public assessmentId: string = '';
  public numberAutoAssessor: number = 0;
  public scoreMode: ScoreMode = ScoreMode.Instructor;

  constructor(data?: IAssignmentAssessmentConfig) {
    if (data != null) {
      this.assessmentId = data.assessmentId;
      this.numberAutoAssessor = data.numberAutoAssessor;
      this.scoreMode = data.scoreMode;
    }
  }
}
