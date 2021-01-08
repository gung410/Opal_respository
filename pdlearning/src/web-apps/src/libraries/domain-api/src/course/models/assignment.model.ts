import { AssignmentAssessmentConfig, IAssignmentAssessmentConfig } from './assignment-assessment-config.model';
import { IQuizAssignmentForm, QuizAssignmentForm } from './quiz-assignment-form.model';

import { AssignmentType } from './assignment-type.model';
import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '@opal20/domain-api';
export interface IAssignment {
  id?: string;
  courseId: string;
  classRunId?: string;
  title: string;
  randomizedQuestions: boolean;
  type: AssignmentType;
  createdBy: string;
  quizAssignmentForm?: IQuizAssignmentForm;
  assessmentConfig?: IAssignmentAssessmentConfig;
}

export class Assignment implements IAssignment {
  public static optionalProps: (keyof IAssignment)[] = ['quizAssignmentForm'];
  public id?: string;
  public courseId: string;
  public classRunId?: string;
  public title: string = '';
  public randomizedQuestions: boolean = false;
  public type: AssignmentType = AssignmentType.Quiz;
  public createdBy: string;
  public quizAssignmentForm?: QuizAssignmentForm = new QuizAssignmentForm();
  public assessmentConfig?: AssignmentAssessmentConfig;

  public static hasCreateAssignmentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ManageAssignments);
  }

  constructor(data?: IAssignment) {
    if (data) {
      this.id = data.id;
      this.courseId = data.courseId;
      this.classRunId = data.classRunId;
      this.title = data.title;
      this.randomizedQuestions = data.randomizedQuestions;
      this.type = data.type;
      this.createdBy = data.createdBy;
      this.quizAssignmentForm =
        data.quizAssignmentForm != null ? new QuizAssignmentForm(data.quizAssignmentForm) : new QuizAssignmentForm();
      this.assessmentConfig = data.assessmentConfig != null ? new AssignmentAssessmentConfig(data.assessmentConfig) : this.assessmentConfig;
    }
  }
}
