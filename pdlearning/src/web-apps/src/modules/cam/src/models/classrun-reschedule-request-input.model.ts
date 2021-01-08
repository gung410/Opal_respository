import { RescheduleSession } from '@opal20/domain-api';

export class ClassRunRescheduleInput {
  public id?: string;
  public comment: string;
  public startDateTime: Date;
  public endDateTime: Date;
  public rescheduleSessions: RescheduleSession[];
}
