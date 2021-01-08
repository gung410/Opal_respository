import { EmailOption } from './email-option';
import { ExportOption } from './export-option';

export class ExportSystemUsageParameters {
  exportOption: ExportOption;
  emailOption?: EmailOption;
  sendEmail?: boolean;
  fromDate?: string;
  toDate?: string;
}
