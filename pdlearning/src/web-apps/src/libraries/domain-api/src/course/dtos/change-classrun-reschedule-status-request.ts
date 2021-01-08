import { ClassRunRescheduleStatus } from '../models/classrun.model';
import { RescheduleSession } from './../models/reschedule-session.model';
export interface IClassRunRescheduleStatusRequest {
  ids: string[];
  startDateTime?: Date;
  endDateTime?: Date;
  rescheduleSessions?: RescheduleSession[];
  rescheduleStatus: ClassRunRescheduleStatus;
  comment?: string;
}
