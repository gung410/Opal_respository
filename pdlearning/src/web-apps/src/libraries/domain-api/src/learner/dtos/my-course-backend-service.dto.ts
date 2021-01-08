import { IMyCourseResultModel, MyCourseResultModel } from '../models/my-course-result.model';
import { MyCourseStatus, MyRegistrationStatus } from '../models/my-course.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';

export class PagedCourseModelResult implements IPagedResultDto<IMyCourseResultModel> {
  public totalCount: number = 0;
  public items: MyCourseResultModel[] = [];

  constructor(data?: IPagedResultDto<IMyCourseResultModel>) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items ? data.items.map(p => new MyCourseResultModel(p)) : [];
  }
}

export interface IEnrollCourseRequest {
  courseId: string;
  lectureIds: string[];
}

export interface ICreateCourseReviewRequest {
  courseId: string;
  rating: number;
  commentContent?: string | undefined;
}

export interface IMyCourseRequest {
  id?: string;
  courseId?: string;
  status?: MyCourseStatus;
  courseType?: LearningCourseType;
  myRegistrationStatus?: MyRegistrationStatus;
  resultId?: string;
}

export interface IUpdateCourseStatus {
  courseId: string;
  status: MyCourseStatus;
}

export interface IReEnrollCourseRequest {
  courseId: string;
  lectureIds: string[];
  courseType: LearningCourseType;
}

export enum LearningCourseType {
  Microlearning = 'Microlearning',
  FaceToFace = 'FaceToFace'
}
