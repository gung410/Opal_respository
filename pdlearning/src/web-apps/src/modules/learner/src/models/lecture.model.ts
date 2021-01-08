import { CourseContentItemModel, CourseContentItemType, IMyLectureModel, LectureModel, MyLectureModel } from '@opal20/domain-api';

export interface ILearnerLectureModel {
  lectureId: string;
  myLectureInfo?: IMyLectureModel | undefined;
  lectureDetail?: LectureModel | undefined;
}

export class LearnerLectureModel implements ILearnerLectureModel {
  public lectureId: string;
  public myLectureInfo?: MyLectureModel;
  public lectureDetail?: LectureModel;

  public static fromCourseContentItem(courseContentItem: CourseContentItemModel): LearnerLectureModel | undefined {
    if (courseContentItem.type !== CourseContentItemType.Lecture) {
      return undefined;
    }

    const lectureDetail: LectureModel = courseContentItem.additionalInfo as LectureModel;

    return new LearnerLectureModel({
      lectureId: courseContentItem.id,
      lectureDetail: lectureDetail
    });
  }

  constructor(data?: ILearnerLectureModel) {
    if (data == null) {
      return;
    }
    this.lectureId = data.lectureId;
    this.myLectureInfo = new MyLectureModel(data.myLectureInfo);
    this.lectureDetail = new LectureModel(data.lectureDetail);
  }

  /* default is enabled */
  public get enablePassingRate(): boolean {
    return this.lectureDetail.quizConfig == null || this.lectureDetail.quizConfig.byPassPassingRate === false;
  }
}
