import { FormParticipantStatus } from './form-participant-status.enum';

export interface IFormParticipant {
  id?: string;
  formId?: string;
  userId?: string;
  assignedDate?: Date;
  submittedDate?: Date;
  status?: FormParticipantStatus;
  changedBy?: string;
  createdBy?: string;
  isStarted?: string;
}

export class FormParticipant implements IFormParticipant {
  public id?: string;
  public formId?: string;
  public userId?: string;
  public assignedDate?: Date;
  public submittedDate?: Date;
  public status?: FormParticipantStatus;
  public changedBy?: string;
  public createdBy?: string;
  public isStarted?: string;

  constructor(data?: IFormParticipant) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.formId = data.formId;
    this.userId = data.userId;
    this.assignedDate = data.assignedDate;
    this.submittedDate = data.submittedDate;
    this.status = data.status;
    this.changedBy = data.changedBy;
    this.createdBy = data.createdBy;
    this.isStarted = data.isStarted;
  }
}
