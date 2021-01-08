import { MyCourseStatus } from '../models/my-course.model';

export interface IMyCoursesSummaryRequest {
  statusFilter: MyCourseStatus[];
}
