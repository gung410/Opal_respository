import { BookmarkInfoModel } from './bookmark-info.model';

export interface ILearnerLearningPath {
  id: string;
  title: string;
  thumbnailUrl?: string;
  createdBy: string;
  createdDate: Date;
  courses: ILearnerLearningPathCourse[];
  bookmarkInfo?: BookmarkInfoModel | undefined;
  isPublic: boolean;
}

export interface ILearnerLearningPathCourse {
  id: string;
  courseId: string;
  order?: number;
  learningPathId: string;
}

export class LearnerLearningPath implements ILearnerLearningPath {
  public id: string;
  public title: string;
  public thumbnailUrl?: string;
  public createdBy: string;
  public createdDate: Date;
  public courses: LearnerLearningPathCourse[];
  public bookmarkInfo?: BookmarkInfoModel | undefined;
  public isPublic: boolean;
  constructor(data?: ILearnerLearningPath) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.title = data.title;
    this.thumbnailUrl = data.thumbnailUrl;
    this.createdBy = data.createdBy;
    this.createdDate = data.createdDate ? new Date(data.createdDate) : new Date();
    this.courses = data.courses ? data.courses.map(c => new LearnerLearningPathCourse(c)) : [];
    this.bookmarkInfo = data.bookmarkInfo;
    this.isPublic = data.isPublic;
  }
}

export class LearnerLearningPathCourse implements ILearnerLearningPathCourse {
  public id: string;
  public courseId: string;
  public order?: number;
  public learningPathId: string;
  constructor(data?: ILearnerLearningPathCourse) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.courseId = data.courseId;
    this.order = data.order;
    this.learningPathId = data.learningPathId;
  }
}
