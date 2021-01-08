import { ContentStatus } from '../models/course.model';
export interface IChangeCourseContentStatusRequest {
  ids: string[];
  contentStatus: ContentStatus;
  comment: string;
}
