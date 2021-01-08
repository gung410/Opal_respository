import { Session } from '../models/session.model';

export interface ISaveSessionRequest {
  data: Session;
  updatePreRecordClipOnly?: boolean;
}
