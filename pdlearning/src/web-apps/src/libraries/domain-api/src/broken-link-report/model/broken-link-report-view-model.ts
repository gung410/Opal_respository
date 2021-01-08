import { BrokenLinkReport, IBrokenLinkReport } from './broken-link-report';

import { PublicUserInfo } from '../../share/models/user-info.model';

export interface IBrokenLinkReportViewModel extends IBrokenLinkReport {
  reportByUser: PublicUserInfo;
}

export class BrokenLinkReportViewModel extends BrokenLinkReport {
  public reportByUser: PublicUserInfo;
  public static createViewModel(brokenlinkReport: IBrokenLinkReport, reportByUser: PublicUserInfo): BrokenLinkReportViewModel {
    return new BrokenLinkReportViewModel({
      ...brokenlinkReport,
      reportByUser: reportByUser
    });
  }

  constructor(data?: IBrokenLinkReportViewModel) {
    super(data);
    if (data != null) {
      this.reportByUser = data.reportByUser;
    }
  }
}
