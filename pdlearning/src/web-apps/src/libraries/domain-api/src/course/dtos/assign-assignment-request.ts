import { Registration } from '../models/registrations.model';

export interface IAssignAssignmentRequest {
  registrations: AssignAssignmentPaticipant[];
  assignmentId: string;
  startDate: Date;
  endDate: Date;
}

export class AssignAssignmentPaticipant {
  public registrationId: string;
  public userId: string;

  constructor(data: Registration) {
    this.registrationId = data.id;
    this.userId = data.userId;
  }
}
