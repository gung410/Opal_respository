import { UserType } from 'app/staff/staff.container/staff-list/models/staff.model';
import { ApprovalGroupTypeEnum } from '../constants/approval-group.enum';

export class ApprovalGroup extends UserType {
  departmentId?: number;
  approverId?: number;
  type: ApprovalGroupTypeEnum;
}
