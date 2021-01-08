import { CourseStatus } from '../../share/models/course-status.enum';

export interface IChangeCourseStatusRequest {
  ids: string[];
  status: CourseStatus;
  comment: string;
}
