import { RegistrationStatus } from '../models/registrations.model';
export interface IChangeRegistrationStatusRequest {
  ids: string[];
  status: RegistrationStatus;
  comment: string;
}
