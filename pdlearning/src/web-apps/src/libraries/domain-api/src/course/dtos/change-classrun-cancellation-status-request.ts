import { ClassRunCancellationStatus } from '../models/classrun.model';
export interface IClassRunCancellationStatusRequest {
  ids: string[];
  cancellationStatus: ClassRunCancellationStatus;
  comment?: string;
}
