import { AssignAssignmentPaticipant } from '@opal20/domain-api';
import { DialogAction } from '@progress/kendo-angular-dialog';

export interface IAssignAssignmentDialogEvent {
  action: DialogAction;
  registrations: AssignAssignmentPaticipant[];
  startDate: Date;
  endDate: Date;
}
