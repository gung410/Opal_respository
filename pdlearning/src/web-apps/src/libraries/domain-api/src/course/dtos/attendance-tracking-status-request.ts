import { AttendanceStatus } from '../models/attendance-tracking.model';

export interface IAttendanceTrackingStatusRequest {
  sessionId?: string;
  ids: string[];
  status?: AttendanceStatus;
}
