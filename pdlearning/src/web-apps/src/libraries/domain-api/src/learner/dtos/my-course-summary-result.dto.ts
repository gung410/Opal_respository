import { MyCourseStatus } from '../models/my-course.model';

export interface IMyCoursesSummaryResult {
  statusFilter: MyCourseStatus;
  total: number;
}

export class MyCoursesSummaryResult {
  public total: number = 0;
  public statusFilter: MyCourseStatus;

  constructor(data?: IMyCoursesSummaryResult) {
    if (data == null) {
      return;
    }
    this.total = data.total;
    this.statusFilter = data.statusFilter;
  }
}
