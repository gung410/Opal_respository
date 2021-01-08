import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { interval, Observable, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { AppConstant } from '../app.constant';

@Injectable({
  providedIn: 'root',
})
export class ReportsDataService {
  private reportBaseUrl: string = AppConstant.moduleLink.report;

  constructor(private httpHelper: HttpHelpers) {}

  checkFileExists(filePath: string): Observable<boolean> {
    return this.httpHelper.get<boolean>(
      `${this.reportBaseUrl}/download/checkFileExists?filePath=${encodeURI(
        filePath
      )}`
    );
  }

  /**
   * DO NOT use this method until you can fix the issue
   * which the "checkFileExists" function is triggered EVERY 10 seconds even though the check is returned TRUE.
   * @param filePath The file path to download the file.
   */
  checkFileExistsUntilFound(filePath: string): Observable<boolean> {
    const keepInterval: Subject<boolean> = new Subject<boolean>();
    const checkFileExists = (): Observable<boolean> => {
      return this.checkFileExists(filePath).pipe(
        tap((fileExists) => {
          if (fileExists === true) {
            keepInterval.complete();
          }
        })
      );
    };
    /**
     * Set interval time to call check file exists after sometime.
     */
    const intervalInMilliseconds = 10000;

    return interval(intervalInMilliseconds)
      .pipe(takeUntil(keepInterval))
      .switchMap(checkFileExists);
  }

  downloadFile(filePath: string, fileName?: string): void {
    const downloadUrl = `${
      this.reportBaseUrl
    }/download/getFile?filePath=${encodeURI(filePath)}&fileName=${
      fileName ? encodeURI(fileName) : ''
    }`;
    this.executeDownloadFile(fileName, downloadUrl);
  }

  executeDownloadFile(fileName: string, downloadUrl: string): void {
    // Since there is the report iframe authenticating in the background.
    // We need to delay calling to the download url.
    const delayBeforeDownloadFile = 2000;
    setTimeout(() => {
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
        window.location.href = downloadUrl;
      }
    }, delayBeforeDownloadFile);
  }
}
