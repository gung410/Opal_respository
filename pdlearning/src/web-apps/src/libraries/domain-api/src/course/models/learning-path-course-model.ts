export interface ILearningPathCourseModel {
  id?: string;
  courseId: string;
  order: number;
}

export class LearningPathCourseModel implements ILearningPathCourseModel {
  public id?: string;
  public courseId: string;
  public order: number;
  constructor(data?: ILearningPathCourseModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.courseId = data.courseId;
    this.order = data.order;
  }
}
