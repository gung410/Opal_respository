import { BookmarkInfoModel, IBookmarkInfoModel } from './bookmark-info.model';
import { IMyClassRunModel, MyClassRunModel } from './my-class-run.model';
import { IMyCourseModel, MyCourseModel } from './my-course.model';
import { IMyLectureModel, MyLectureModel } from './my-lecture.model';

export interface IMyCourseResultModel {
  courseId: string;
  rating: number;
  reviewsCount: number;
  completedTimes: number;
  bookmarkInfo?: IBookmarkInfoModel;
  myCourseInfo?: IMyCourseModel;
  myLecturesInfo?: IMyLectureModel[];

  myClassRuns?: IMyClassRunModel[];
  rejectedMyClassRuns?: IMyClassRunModel[];
  withdrawnMyClassRuns?: IMyClassRunModel[];
  expiredMyClassRun?: IMyClassRunModel;
}

export class MyCourseResultModel implements IMyCourseResultModel {
  public courseId: string;
  public rating: number = 0;
  public reviewsCount: number = 0;
  public completedTimes: number = 0;
  public bookmarkInfo?: BookmarkInfoModel;
  public myCourseInfo?: MyCourseModel;
  public myLecturesInfo?: MyLectureModel[];

  public myClassRuns?: MyClassRunModel[];
  public rejectedMyClassRuns?: MyClassRunModel[];
  public withdrawnMyClassRuns?: MyClassRunModel[];
  public expiredMyClassRun?: MyClassRunModel;
  constructor(data?: IMyCourseResultModel) {
    if (data == null) {
      return;
    }
    this.courseId = data.courseId;
    this.rating = data.rating ? data.rating : 0;
    this.reviewsCount = data.reviewsCount ? data.reviewsCount : 0;
    this.completedTimes = data.completedTimes ? data.completedTimes : 0;
    this.bookmarkInfo = data.bookmarkInfo ? new BookmarkInfoModel(data.bookmarkInfo) : undefined;
    this.myCourseInfo = data.myCourseInfo ? new MyCourseModel(data.myCourseInfo) : undefined;
    this.myLecturesInfo = data.myLecturesInfo ? data.myLecturesInfo.map(_ => new MyLectureModel(_)) : [];

    this.myClassRuns = data.myClassRuns ? data.myClassRuns.map(_ => new MyClassRunModel(_)) : [];
    this.rejectedMyClassRuns = data.rejectedMyClassRuns ? data.rejectedMyClassRuns.map(_ => new MyClassRunModel(_)) : [];
    this.withdrawnMyClassRuns = data.withdrawnMyClassRuns ? data.withdrawnMyClassRuns.map(_ => new MyClassRunModel(_)) : [];
    this.expiredMyClassRun = data.expiredMyClassRun ? new MyClassRunModel(data.expiredMyClassRun) : undefined;
  }

  public get myClassRun(): MyClassRunModel {
    return this.myClassRuns && this.myClassRuns[0];
  }
}
