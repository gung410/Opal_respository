import { Course, LearningPathCourseModel } from '@opal20/domain-api';

export interface ILearningPathCourseViewModel {
  learningPathCourse: LearningPathCourseModel;
  course: Course;
}

export class LearningPathCourseViewModel implements ILearningPathCourseViewModel {
  public learningPathCourse: LearningPathCourseModel;
  public course: Course;

  constructor(data: LearningPathCourseModel, course: Course) {
    this.learningPathCourse = data;
    this.course = course;
  }
}
