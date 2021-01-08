import {
  BookmarkType,
  ClassRun,
  CourseStatus,
  MetadataId,
  MyClassRunModel,
  MyCourseDisplayStatus,
  MyCourseStatus,
  RegistrationType
} from '@opal20/domain-api';
import { DISPLAY_STATUS_MAP, MY_COURSE_STATUS_DISPLAY_MAP } from '../constants/learning-card-status.constant';

import { CourseModel } from './course.model';

export interface ILearningItemModel {
  id: string;
  title: string;
  imageUrl?: string | undefined;
  isBookmark: boolean;
  code?: string | undefined;
  type: LearningType;
}

export class LearningItemModel implements ILearningItemModel {
  public tagIds: string[] = [];
  public type: LearningType = LearningType.Course;

  private _course: CourseModel;
  constructor(courseModel?: CourseModel) {
    if (courseModel === undefined) {
      return;
    }
    this._course = courseModel;
    this.buildTags(courseModel);
  }
  //#region courseModel proxy
  public get course(): CourseModel {
    return this._course;
  }

  public get id(): string {
    return this._course.courseId;
  }

  public get courseId(): string {
    return this._course.courseId;
  }

  public get rating(): number {
    return this._course.rating;
  }

  public get reviewsCount(): number {
    return this._course.reviewsCount;
  }

  public get title(): string {
    return this._course.courseDetail.courseName;
  }

  public get imageUrl(): string {
    return this._course.courseDetail.thumbnailUrl;
  }

  public get duration(): number {
    return this._course.courseDetail.durationMinutes;
  }

  public get code(): string {
    return this._course.courseDetail.courseCode;
  }

  public get expiredDate(): Date {
    return this._course.courseDetail.expiredDate;
  }

  public get pdActivityType(): string {
    return this._course.courseDetail.pdActivityType;
  }

  public get status(): CourseStatus {
    return this._course.courseDetail.status !== undefined ? this._course.courseDetail.status : undefined;
  }

  public get latestCompletedLectureName(): string {
    return this._course.latestCompletedLectureName;
  }

  public get isBookmark(): boolean {
    return !!this._course.bookmarkInfo;
  }

  public get isMicrolearning(): boolean {
    return this._course.isMicrolearning;
  }

  public get expiredMyClassRun(): MyClassRunModel {
    return this._course.expiredMyClassRun;
  }

  public get displayStatus(): MyCourseDisplayStatus {
    return this._course.myCourseInfo ? this._course.myCourseInfo.displayStatus : undefined;
  }

  public get hasContentChanged(): boolean {
    return this._course.myCourseInfo ? this._course.myCourseInfo.hasContentChanged : false;
  }

  public get progress(): number {
    return this._course.myCourseInfo && this._course.myCourseInfo.progressMeasure ? this._course.myCourseInfo.progressMeasure : 0;
  }

  public get completedDate(): Date {
    return this._course.myCourseInfo ? this._course.myCourseInfo.completedDate : undefined;
  }

  public get learningStatus(): MyCourseStatus {
    return this._course.myCourseInfo ? this._course.myCourseInfo.status : undefined;
  }

  public get postCourseEvaluationFormCompleted(): boolean {
    return this._course.myClassRun && this._course.myClassRun.postCourseEvaluationFormCompleted;
  }

  public get myClassRun(): MyClassRunModel {
    return this._course.myClassRun;
  }

  public get myClassRuns(): MyClassRunModel[] {
    return this._course.myClassRuns;
  }

  public get classRunDetail(): ClassRun {
    return this._course.classRunDetail;
  }

  public get classRunsDetail(): ClassRun[] {
    return this._course.classRunsDetail;
  }

  public get upcomingSessionDate(): Date {
    return this._course.upcomingSessionDate;
  }

  public get isArchivedCourse(): boolean {
    return this._course.isArchived;
  }

  public get isExpiredCourse(): boolean {
    return this._course.isExpired;
  }

  public get isUnPublishedCourse(): boolean {
    return this._course.isUnPublished;
  }

  //#endregion

  public get isIncomplete(): boolean {
    return this.learningStatus === MyCourseStatus.Failed;
  }

  public get isInProgress(): boolean {
    return this.learningStatus === MyCourseStatus.InProgress;
  }

  public get isCompleted(): boolean {
    return this.learningStatus === MyCourseStatus.Completed;
  }

  public get showInProgress(): boolean {
    return !this.isExpiredCourse && !this.isArchivedCourse && this.isInProgress;
  }

  public get showCompleted(): boolean {
    return !this.isExpiredCourse && !this.isArchivedCourse && this.isCompleted;
  }

  public get showIncomplete(): boolean {
    return !this.isExpiredCourse && !this.isArchivedCourse && this.isIncomplete;
  }

  public get getCourseBookmarkType(): BookmarkType {
    return this.pdActivityType === MetadataId.Microlearning ? BookmarkType.Microlearning : BookmarkType.Course;
  }

  public get learningStatusText(): string {
    if (this.isArchivedCourse) {
      return 'Archived';
    }

    if (this.isExpiredCourse) {
      return 'Expired';
    }

    if (this.isIncomplete) {
      return 'Incomplete';
    }

    // !this.expiredMyClassRun -> the learning is in-progress but the class is cancelled
    if (MY_COURSE_STATUS_DISPLAY_MAP.has(this.learningStatus) && !this.expiredMyClassRun) {
      return MY_COURSE_STATUS_DISPLAY_MAP.get(this.learningStatus);
    }

    if (!this.displayStatus) {
      return '';
    }

    if (this.expiredMyClassRun) {
      switch (this.expiredMyClassRun.registrationType) {
        case RegistrationType.Manual:
          return DISPLAY_STATUS_MAP.get(MyCourseDisplayStatus.Expired);
        case RegistrationType.Nominated:
          return DISPLAY_STATUS_MAP.get(MyCourseDisplayStatus.NominatedExpired);
        case RegistrationType.AddedByCA:
          return DISPLAY_STATUS_MAP.get(MyCourseDisplayStatus.AddedByCAExpired);
      }
    }

    return DISPLAY_STATUS_MAP.get(this.displayStatus);
  }

  private buildTags(courseModel: CourseModel): void {
    const pdActivityType = courseModel.courseDetail.pdActivityType;
    const learningMode = courseModel.courseDetail.learningMode;
    const subjectArea = courseModel.courseDetail.subjectAreaIds && courseModel.courseDetail.subjectAreaIds[0];

    if (pdActivityType) {
      this.tagIds.push(pdActivityType);
    }
    if (learningMode) {
      this.tagIds.push(learningMode);
    }
    if (subjectArea) {
      this.tagIds.push(subjectArea);
    }
  }
}

export enum LearningType {
  Course = 'Course',
  Microlearning = 'Microlearning',
  LearningPath = 'LearningPath',
  DigitalContent = 'DigitalContent',
  Community = 'Community',
  LearningPathLMM = 'LearningPathLMM',
  StandaloneForm = 'StandaloneForm'
}

export class LearningItemResult {
  public items: ILearningItemModel[];
  public total: number;
  constructor(items: ILearningItemModel[], total: number) {
    this.items = items;
    this.total = total;
  }
}
