import { FormParticipantStatus } from '../models/form-participant-status.enum';

export interface IUpdateFormParticipantStatusRequest {
  formId: string;
  formParticipantStatus?: FormParticipantStatus;
}
