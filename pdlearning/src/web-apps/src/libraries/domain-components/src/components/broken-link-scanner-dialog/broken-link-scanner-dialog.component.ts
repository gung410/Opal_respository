import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { BrokenLinkReportApiService, BrokenLinkReportType, ScanUrlStatus } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { timeout } from 'rxjs/operators';

@Component({
  selector: 'broken-link-scanner-dialog',
  templateUrl: './broken-link-scanner-dialog.component.html'
})
export class BrokenLinkScannerDialogComponent extends BaseFormComponent {
  @Input() public html: string;
  @Input() public brokenLinkReportType: BrokenLinkReportType;

  public gridResult: BrokenLinkScannerItems[];
  public urlStatus = ScanUrlStatus;
  public totalLink: number;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public brokenLinkReportApiService: BrokenLinkReportApiService
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    const urlData = Utils.extracUrlfromHtml(this.html);
    this.gridResult = urlData.map(item => new BrokenLinkScannerItems(item));
    this.totalLink = this.gridResult.length;
    this.onCheckingUrls();
  }

  public async onCheckingUrls(): Promise<void> {
    for (const item of this.gridResult) {
      this.brokenLinkReportApiService
        .checkBrokenLink({ url: item.url }, false)
        .pipe(timeout(28000))
        .subscribe(
          val => {
            item.status = val.isValid ? ScanUrlStatus.Valid : ScanUrlStatus.Invalid;
            item.description = val.invalidReason;
            this.onFilterInvalidListUrl();
          },
          err => {
            item.status = ScanUrlStatus.Invalid;
            item.description = 'Request Timeout';
            this.onFilterInvalidListUrl();
          }
        );
    }
  }

  public onFilterInvalidListUrl(): void {
    this.gridResult = this.gridResult.filter(x => x.status === ScanUrlStatus.Checking || x.status === ScanUrlStatus.Invalid);
  }

  public onCancel(): void {
    if (this.isUrlChecking) {
      this.modalService.showConfirmMessage(
        'Broken links are checking, do you want to close ?',
        () => {
          this.dialogRef.close();
        },
        null,
        null
      );
    } else {
      this.dialogRef.close();
    }
  }

  public get isUrlChecking(): boolean {
    return this.gridResult.some(item => item.status === ScanUrlStatus.Checking);
  }

  public isAnyUrlInValid(): boolean {
    return this.gridResult.some(item => item.status === ScanUrlStatus.Invalid);
  }

  public isShowValidMessage(): boolean {
    return !this.isAnyUrlInValid() && !this.isUrlChecking;
  }

  public isShowInvalidMessage(): boolean {
    return this.isAnyUrlInValid() && !this.isUrlChecking;
  }

  public get numberLinkChecked(): number {
    return this.totalLink - this.gridResult.filter(x => x.status === ScanUrlStatus.Checking).length;
  }

  public getDialogTitle(): string {
    switch (this.brokenLinkReportType) {
      case BrokenLinkReportType.Form:
        return this.translate('Broken link(s) is detected in the form. Please check and change the link(s) shown below.');
      case BrokenLinkReportType.DigitalContent:
        return this.translate('Broken link(s) is detected in the learning content. Please check and change the link(s) shown below.');
      default:
        return this.translate('The system has detected broken link in the content. The list of broken link is:');
    }
  }
}

export class BrokenLinkScannerItems {
  public url: string;
  public description?: string;
  public status: ScanUrlStatus;

  constructor(data: string) {
    if (data) {
      this.url = data;
      this.status = ScanUrlStatus.Checking;
    }
  }
}
