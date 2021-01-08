import { ContentStatus } from '../models/course.model';
export interface IChangeClassRunContentStatusRequest {
  ids: string[];
  contentStatus: ContentStatus;
  comment: string;
}
