import { ExportSummaryOption } from './export-summary-option';
import { ExportType } from './export-type';
import { InfoRecord } from './info-record';
import { SummaryPosition } from './summary-position';

export class ExportOption {
  csvDelimiter?: string;
  exportFields: any;
  verticalExportFields?: any;
  summaryOption?: ExportSummaryOption;
  summaryPosition?: SummaryPosition;
  showRecordType?: boolean;
  exportType?: ExportType;
  dateTimeFormat?: string;
  timeZoneOffset?: number;
  exportTitle?: string;
  infoRecords?: InfoRecord[];
  addDateTimeAsInfoRecords?: boolean;
  showRowNumber?: boolean;
  rowNumberColumnCaption?: string;
}
