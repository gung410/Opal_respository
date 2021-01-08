import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ChangeDetectorRef, Component } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { ReportsDataService } from 'app-services/reports-data.services';
import { BaseComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'nomination-file-name-renderer',
  template: `<span
      *ngIf="displayFileName"
      style="color: blue;
  text-decoration: underline;
  cursor: pointer;"
      (click)="onClickItem($event)"
      >{{ displayFileName || 'N/A' }}</span
    ><span *ngIf="!displayFileName">N/A</span>`,
})
export class NominationFileNameRendererComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  public params: any;
  public displayFileName: string;
  public resultId: number;
  constructor(
    private globalLoader: CxGlobalLoaderService,
    private reportsDataService: ReportsDataService,
    private assignPDOService: AssignPDOService,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }

  // called on init
  public agInit(params: any): void {
    this.params = params;
    if (params && params.value) {
      this.resultId = this.params.value.resultId;
      this.displayFileName = this.params.value.displayFileName;
    }
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.resultId = this.params.value.resultId;
    this.displayFileName = this.params.value.displayFileName;

    return true;
  }

  public onClickItem(event: any): void {
    this.downloadFile();
  }

  private async downloadFile() {
    this.globalLoader.showLoader();
    const downloadObj = await this.assignPDOService.downloadMassNominationReportFileAsync(
      this.resultId
    );

    let interval: any;

    const checkDownloadFile = () => {
      this.subscriptionAdder = this.reportsDataService
        .checkFileExists(downloadObj.filePath)
        .subscribe((fileExists) => {
          if (fileExists) {
            // Stop calling the check download file function and save the file.
            clearInterval(interval);
            this.reportsDataService.downloadFile(
              downloadObj.filePath,
              `${this.displayFileName}`
            );
            this.globalLoader.hideLoader();
          }
        });
    };

    /**
     * Set interval time to call check download file after sometime.
     */
    const intervalInMilliseconds = 10000;
    interval = setInterval(checkDownloadFile, intervalInMilliseconds);
  }
}
