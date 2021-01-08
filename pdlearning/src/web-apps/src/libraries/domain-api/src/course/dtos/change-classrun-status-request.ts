import { ClassRunStatus } from '../models/classrun.model';
export interface IChangeClassRunStatusRequest {
  ids: string[];
  status: ClassRunStatus;
}
