import { CourseCriteriaPlaceOfWork, ICourseCriteriaPlaceOfWork } from './course-criterial-place-of-word.model';
import { CourseCriteriaServiceScheme, ICourseCriteriaServiceScheme } from './course-criteria-service-scheme.model';

import { Course } from './course.model';
import { Utils } from '@opal20/infrastructure';
export interface ICourseCriteria {
  id: string;
  accountType?: CourseCriteriaAccountType;
  courseCriteriaServiceSchemes?: ICourseCriteriaServiceScheme[];
  tracks?: string[];
  devRoles?: string[];
  teachingLevels?: string[];
  teachingCourseOfStudy?: string[];
  jobFamily?: string[];
  coCurricularActivity?: string[];
  subGradeBanding?: string[];
  placeOfWork?: ICourseCriteriaPlaceOfWork;
  preRequisiteCourses?: string[];
}

export class CourseCriteria implements ICourseCriteria {
  public id: string;
  public accountType: CourseCriteriaAccountType = CourseCriteriaAccountType.AllLearners;
  public courseCriteriaServiceSchemes: CourseCriteriaServiceScheme[] = [];
  public tracks: string[] = [];
  public devRoles: string[] = [];
  public teachingLevels: string[] = [];
  public teachingCourseOfStudy: string[] = [];
  public jobFamily: string[] = [];
  public coCurricularActivity: string[] = [];
  public subGradeBanding: string[] = [];
  public placeOfWork: CourseCriteriaPlaceOfWork = new CourseCriteriaPlaceOfWork();
  public preRequisiteCourses: string[] = [];

  constructor(data?: ICourseCriteria) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.accountType = data.accountType != null ? data.accountType : this.accountType;
    this.courseCriteriaServiceSchemes = data.courseCriteriaServiceSchemes
      ? data.courseCriteriaServiceSchemes.map(p => new CourseCriteriaServiceScheme(p))
      : [];
    this.tracks = data.tracks ? data.tracks : [];
    this.devRoles = data.devRoles ? data.devRoles : [];
    this.teachingLevels = data.teachingLevels ? data.teachingLevels : [];
    this.teachingCourseOfStudy = data.teachingCourseOfStudy ? data.teachingCourseOfStudy : [];
    this.jobFamily = data.jobFamily ? data.jobFamily : [];
    this.coCurricularActivity = data.coCurricularActivity ? data.coCurricularActivity : [];
    this.subGradeBanding = data.subGradeBanding ? data.subGradeBanding : [];
    this.placeOfWork = data.placeOfWork ? new CourseCriteriaPlaceOfWork(data.placeOfWork) : new CourseCriteriaPlaceOfWork();
    this.preRequisiteCourses = data.preRequisiteCourses ? data.preRequisiteCourses : [];
  }
  public getAllMetadataTagIds(): string[] {
    return (<string[]>[])
      .concat(this.courseCriteriaServiceSchemes.map(p => p.serviceSchemeId))
      .concat(this.tracks)
      .concat(this.jobFamily)
      .concat(this.devRoles)
      .concat(this.teachingLevels)
      .concat(this.teachingCourseOfStudy)
      .concat(this.coCurricularActivity)
      .concat(this.subGradeBanding)
      .concat(this.preRequisiteCourses);
  }

  public reuseCourseExistingData(course: Course): void {
    this.courseCriteriaServiceSchemes = Utils.addIfNotExist(
      this.courseCriteriaServiceSchemes,
      course.serviceSchemeIds.map(p => new CourseCriteriaServiceScheme({ serviceSchemeId: p })),
      p => p.serviceSchemeId
    );
    this.tracks = Utils.addIfNotExist(this.tracks, course.trackIds, p => p);
    this.jobFamily = Utils.addIfNotExist(this.jobFamily, course.jobFamily, p => p);
    this.devRoles = Utils.addIfNotExist(this.devRoles, course.developmentalRoleIds, p => p);
    this.teachingLevels = Utils.addIfNotExist(this.teachingLevels, course.teachingLevels, p => p);
    this.teachingCourseOfStudy = Utils.addIfNotExist(this.teachingCourseOfStudy, course.teachingCourseStudyIds, p => p);
    this.coCurricularActivity = Utils.addIfNotExist(this.coCurricularActivity, course.cocurricularActivityIds, p => p);
    this.subGradeBanding = Utils.addIfNotExist(this.subGradeBanding, course.easSubstantiveGradeBandingIds, p => p);
    this.preRequisiteCourses = Utils.addIfNotExist(this.preRequisiteCourses, course.prerequisiteCourseIds, p => p);
  }
}

export enum CourseCriteriaAccountType {
  AllLearners = 'AllLearners',
  MOELearners = 'MOELearners',
  ExternalLearners = 'ExternalLearners'
}
