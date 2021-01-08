import { EmailOption } from './email-option';
import { ExportOption } from './export-option';
import { UserEventType } from './user-event-type';

export class ExportReportInfo {
  exportOption: ExportOption;
  emailOption?: EmailOption;
  sendEmail?: boolean;
  separatedByAccountType?: boolean;
  parentDepartmentIds?: number[];
  filterOnSubDepartment?: boolean;
  userCreatedAfter?: string;
  userCreatedBefore?: string;
  lastLoginAfter?: string;
  lastLoginBefore?: string;
  eventCreatedBefore?: string;
  eventCreatedAfter?: string;
  pageSize?: number;
  pageIndex?: number;
  eventTypes?: UserEventType[];
}
