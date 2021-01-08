import {
  BookmarkInfoModel,
  ClassRun,
  Course,
  CourseStatus,
  IPagedResultDto,
  ISearchFilterResultModel,
  MetadataId,
  MyClassRunModel,
  MyCourseModel,
  MyCourseResultModel,
  MyLectureModel,
  RegistrationMethod,
  Statistic
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class CourseModel {
  public courseId: string;
  public rating: number = 0;
  public reviewsCount: number = 0;
  public completedTimes: number = 0;
  public bookmarkInfo?: BookmarkInfoModel;
  public myCourseInfo?: MyCourseModel;
  public myClassRuns?: MyClassRunModel[] = [];
  public myLecturesInfo?: MyLectureModel[];

  public latestCompletedLectureName?: string;
  public courseDetail?: Course;
  public classRunDetail?: ClassRun;
  public classRunsDetail?: ClassRun[];

  /**
   * Use to show rejected class runs on the list (value available only first load, will not be updated)
   */
  public rejectedClassRunDict: Dictionary<MyClassRunModel> = {};
  /**
   * Use to show withdrawn class runs on the list (value available only first load, will not be updated)
   */
  public withdrawnClassRunDict: Dictionary<MyClassRunModel> = {};

  public expiredMyClassRun?: MyClassRunModel;
  public upcomingSessionDate?: Date;

  private _rejectedMyClassRuns: MyClassRunModel[];
  private _withdrawnMyClassRuns: MyClassRunModel[];

  constructor(
    myCourseDto?: MyCourseResultModel,
    course?: Course,
    classRunsDetail?: ClassRun[],
    latestCompletedLectureName?: string,
    upcomingSession?: Date
  ) {
    this.updateMyCourse(myCourseDto);
    this.courseDetail = course;
    this.updateClassRunsDetails(classRunsDetail);
    this.updateClassRunDetail();
    this.latestCompletedLectureName = latestCompletedLectureName;
    this.upcomingSessionDate = upcomingSession;
  }

  public get isMicrolearning(): boolean {
    return this.courseDetail && this.courseDetail.pdActivityType && this.courseDetail.pdActivityType === MetadataId.Microlearning;
  }

  public get isPrivateCourse(): boolean {
    return this.courseDetail.registrationMethod === RegistrationMethod.Private;
  }

  public get isUnPublished(): boolean {
    return this.courseDetail.status !== CourseStatus.Published;
  }

  public get isArchived(): boolean {
    return this.courseDetail.status === CourseStatus.Archived;
  }

  public get isExpired(): boolean {
    return this.courseDetail.expiredDate != null && this.courseDetail.expiredDate.getTime() < new Date().getTime();
  }

  public get isCourseUnavailable(): boolean {
    return this.isArchived || this.isExpired || this.isUnPublished;
  }

  public get myClassRun(): MyClassRunModel {
    return this.myClassRuns && this.myClassRuns[0];
  }

  public hasReachedMaxRelearningTimes(): boolean {
    return this.completedTimes >= this.courseDetail.maxReLearningTimes;
  }

  public get rejectedMyClassRuns(): MyClassRunModel[] {
    return this._rejectedMyClassRuns;
  }

  public set rejectedMyClassRuns(value: MyClassRunModel[]) {
    if (value == null) {
      return;
    }
    this._rejectedMyClassRuns = value;
    this.rejectedClassRunDict = Utils.toDictionary(this._rejectedMyClassRuns, p => p.classRunId);
  }

  public get withdrawnMyClassRuns(): MyClassRunModel[] {
    return this._withdrawnMyClassRuns;
  }

  public set withdrawnMyClassRuns(value: MyClassRunModel[]) {
    if (value == null) {
      return;
    }
    this._withdrawnMyClassRuns = value;
    this.withdrawnClassRunDict = Utils.toDictionary(this._withdrawnMyClassRuns, p => p.classRunId);
  }

  public updateClassRunDetail(): void {
    this.classRunDetail =
      this.classRunsDetail && this.myClassRun ? this.classRunsDetail.find(p => p.id === this.myClassRun.classRunId) : undefined;
  }

  private updateMyCourse(myCourseDto?: MyCourseResultModel): void {
    if (myCourseDto == null) {
      return;
    }
    this.courseId = myCourseDto.courseId;
    this.rating = myCourseDto.rating;
    this.reviewsCount = myCourseDto.reviewsCount;
    this.completedTimes = myCourseDto.completedTimes;
    this.bookmarkInfo = myCourseDto.bookmarkInfo;
    this.myCourseInfo = myCourseDto.myCourseInfo;
    this.myLecturesInfo = myCourseDto.myLecturesInfo ? myCourseDto.myLecturesInfo : [];
    this.myClassRuns = myCourseDto.myClassRuns ? myCourseDto.myClassRuns : [];
    this.rejectedMyClassRuns = myCourseDto.rejectedMyClassRuns ? myCourseDto.rejectedMyClassRuns : [];
    this.withdrawnMyClassRuns = myCourseDto.withdrawnMyClassRuns ? myCourseDto.withdrawnMyClassRuns : [];
    this.expiredMyClassRun = myCourseDto.expiredMyClassRun;
  }

  private updateClassRunsDetails(classRunsDetail?: ClassRun[]): void {
    this.classRunsDetail = classRunsDetail ? classRunsDetail : [];
  }
}

export class CourseViewSearchFilterResultModel implements ISearchFilterResultModel<CourseModel> {
  public statistics: Statistic[] = [];
  public totalCount: number;
  public items: CourseModel[] = [];

  constructor(data?: ISearchFilterResultModel<CourseModel>) {
    if (data == null) {
      return;
    }
    this.statistics = data.statistics ? data.statistics : [];
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items : [];
  }
}

export class CourseViewPagedResult implements IPagedResultDto<CourseModel> {
  public totalCount: number;
  public items: CourseModel[] = [];

  constructor(data?: IPagedResultDto<CourseModel>) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items : [];
  }
}
