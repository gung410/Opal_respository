import { AttendanceTracking, IAttendanceTracking } from '../models/attendance-tracking.model';

export interface ISearchAttendanceTrackingResult {
  items: IAttendanceTracking[];
  totalCount: number;
}

export class SearchAttendaceTrackingResult {
  public items: AttendanceTracking[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAttendanceTrackingResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new AttendanceTracking(item));
    this.totalCount = data.totalCount;
  }
}
