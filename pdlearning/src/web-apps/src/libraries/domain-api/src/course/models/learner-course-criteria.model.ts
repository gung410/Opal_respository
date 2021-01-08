import { ILearnerCourseCriteriaDepartment, LearnerCourseCriteriaDepartment } from './learner-course-criteria-department.model';

import { Utils } from '@opal20/infrastructure';
export interface ILearnerCourseCriteria {
  userId: string;
  accountType: CourseUserAccountType;
  serviceSchemes: string[];
  tracks: string[];
  devRoles: string[];
  teachingLevels: string[];
  teachingCourseOfStudy: string[];
  jobFamily: string[];
  coCurricularActivity: string[];
  subGradeBanding: string[];
  department: ILearnerCourseCriteriaDepartment;
}

export class LearnerCourseCriteria implements ILearnerCourseCriteria {
  public userId: string;
  public accountType: CourseUserAccountType = CourseUserAccountType.Internal;
  public serviceSchemes: string[] = [];
  public tracks: string[] = [];
  public devRoles: string[] = [];
  public teachingLevels: string[] = [];
  public teachingCourseOfStudy: string[] = [];
  public jobFamily: string[] = [];
  public coCurricularActivity: string[] = [];
  public subGradeBanding: string[] = [];
  public department: LearnerCourseCriteriaDepartment = new LearnerCourseCriteriaDepartment();

  constructor(data?: ILearnerCourseCriteria) {
    if (data != null) {
      this.userId = data.userId;
      this.accountType = Utils.defaultIfNull(data.accountType, CourseUserAccountType.Internal);
      this.serviceSchemes = Utils.defaultIfNull(data.serviceSchemes, []);
      this.tracks = Utils.defaultIfNull(data.tracks, []);
      this.devRoles = Utils.defaultIfNull(data.devRoles, []);
      this.teachingLevels = Utils.defaultIfNull(data.teachingLevels, []);
      this.teachingCourseOfStudy = Utils.defaultIfNull(data.teachingCourseOfStudy, []);
      this.jobFamily = Utils.defaultIfNull(data.jobFamily, []);
      this.coCurricularActivity = Utils.defaultIfNull(data.coCurricularActivity, []);
      this.subGradeBanding = Utils.defaultIfNull(data.subGradeBanding, []);
      this.department = data.department != null ? new LearnerCourseCriteriaDepartment(data.department) : this.department;
    }
  }
}

export enum CourseUserAccountType {
  Internal = 'Internal',
  External = 'External'
}
