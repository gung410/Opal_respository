import { ClassRunChangeStatus } from '../models/registrations.model';
export interface IChangeRegistrationChangeClassRunStatusRequest {
  ids: string[];
  classRunChangeStatus: ClassRunChangeStatus;
  comment?: string;
}
