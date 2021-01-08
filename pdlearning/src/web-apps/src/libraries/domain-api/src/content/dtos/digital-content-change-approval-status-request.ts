import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';

export interface IDigitalContentChangeApprovalStatusRequest {
  id: string;
  status: DigitalContentStatus;
  comment?: string;
}

export class DigitalContentChangeApprovalStatusRequest implements IDigitalContentChangeApprovalStatusRequest {
  public id: string;
  public status: DigitalContentStatus;
  public comment?: string;

  constructor(data?: IDigitalContentChangeApprovalStatusRequest) {
    if (data != null) {
      this.id = data.id;
      this.status = data.status;
      this.comment = data.comment;
    }
  }
}
