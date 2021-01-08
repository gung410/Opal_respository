import { IBrokenLinkReport } from '../model/broken-link-report';

export interface IBrokenLinkReportSearchResult {
  totalCount: number;
  items: IBrokenLinkReport[];
}

export class BrokenLinkReportSearchResult implements IBrokenLinkReportSearchResult {
  public totalCount: number;
  public items: IBrokenLinkReport[];
  constructor(data: IBrokenLinkReportSearchResult) {
    if (!data) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items;
  }
}

export interface IBrokenLinkCheckResult {
  isValid: boolean;
  invalidReason?: string;
}

export class BrokenLinkCheckResult {
  public isValid: boolean;
  public invalidReason?: string;
  constructor(data: IBrokenLinkCheckResult) {
    if (!data) {
      return;
    }
    this.isValid = data.isValid;
    this.invalidReason = data.invalidReason;
  }
}

export enum ScanUrlStatus {
  None = 'None',
  Valid = 'Valid',
  Invalid = 'Invalid',
  Checking = 'Checking'
}
