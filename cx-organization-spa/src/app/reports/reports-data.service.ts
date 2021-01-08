import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { InstructionReporting } from 'app/user-accounts/models/reporting-by-systemrole.model';
import { Observable } from 'rxjs';
import { ExportReportInfo } from './models/export-report-info.model';

@Injectable({
  providedIn: 'root'
})
export class ReportsDataService {
  private organizationReportBaseUrl: string = `${AppConstant.api.organization}/reports`;
  private reportBaseUrl: string = AppConstant.moduleLink.Report;

  constructor(private httpHelper: HttpHelpers) {}

  exportUserAccountDetails(
    exportReportInfo: ExportReportInfo
  ): Observable<InstructionReporting> {
    return this.httpHelper.post(
      `${this.organizationReportBaseUrl}/users/accountDetails/export/async`,
      exportReportInfo
    );
  }

  exportAccountStatistics(
    exportReportInfo: ExportReportInfo
  ): Observable<InstructionReporting> {
    return this.httpHelper.post(
      `${this.organizationReportBaseUrl}/users/statistics/export/async`,
      exportReportInfo
    );
  }

  exportPrivilegedAccounts(
    exportPrivilegedAccountParameters: ExportReportInfo
  ): Observable<InstructionReporting> {
    return this.httpHelper.post(
      `${this.organizationReportBaseUrl}/users/privilegedaccount/export/async`,
      exportPrivilegedAccountParameters
    );
  }

  checkFileExists(filePath: string): Observable<boolean> {
    return this.httpHelper.get<boolean>(
      `${this.reportBaseUrl}/download/checkFileExists?filePath=${encodeURI(
        filePath
      )}`
    );
  }

  downloadFile(filePath: string, fileName?: string): void {
    console.log(
      `${new Date().toISOString()} - Processing download file ${filePath}`
    ); // TODO: Remove after troubleshooting.
    const downloadUrl = `${
      this.reportBaseUrl
    }/download/getFile?filePath=${encodeURI(filePath)}&fileName=${
      fileName ? encodeURI(fileName) : ''
    }`;

    // Since there is the report iframe authenticating in the background.
    // We need to delay calling to the download url.
    const delayBeforeDownloadFile = 2000;
    setTimeout(() => {
      this.proceedToDownload(fileName, downloadUrl);
    }, delayBeforeDownloadFile);
  }

  executeDownloadFile(fileName: string, downloadUrl: string): void {
    const delayBeforeDownloadFile = 2000;
    setTimeout(() => {
      this.proceedToDownload(fileName, downloadUrl);
    }, delayBeforeDownloadFile);
  }

  private proceedToDownload(fileName: string, downloadUrl: string): void {
    const isChromeIOS =
      /CriOS/i.test(navigator.userAgent) &&
      /iphone|ipod|ipad/i.test(navigator.userAgent);
    const isDownloadingZipFile = /.zip/i.test(fileName);
    if (isChromeIOS && !isDownloadingZipFile) {
      // Chrome IOS doesn't open the save popup for csv/excel/text file.
      // In order to prevent the browser open the file directly in the same tab,
      // we should force the browser to open the new tab in order to see the content of the file.
      window.open(downloadUrl, '_blank');
    } else {
      // Use redirection if the browser should the save popup instead of opening the file.
      this.downloadFileByHTMLElement(downloadUrl);
    }
  }

  private downloadFileByHTMLElement(filePath: string): void {
    const link = document.createElement('a');
    link.href = filePath;
    link.download = filePath.substr(filePath.lastIndexOf('/') + 1);
    link.click();
  }
}
