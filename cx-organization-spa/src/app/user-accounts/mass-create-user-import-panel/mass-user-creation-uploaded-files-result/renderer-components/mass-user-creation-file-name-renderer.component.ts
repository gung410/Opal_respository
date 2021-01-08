import { Component } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ReportsDataService } from 'app/reports/reports-data.service';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { FileTypeExtension } from 'app/shared/constants/file-type.enum';
import { FileInfoApiService } from 'app/user-accounts/services/file-info-api.service';
import { FileInfoListViewModel } from 'app/user-accounts/viewmodels/file-info-list.viewmodel';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'mass-user-creation-file-name-renderer',
  template: `<span
      *ngIf="fileInfo.originalFileName"
      style="color: blue;
  text-decoration: underline;
  cursor: pointer;"
      (click)="onClickItem(fileInfo.originalFileName, fileInfo.filePath)"
      >{{ fileInfo.originalFileName || 'N/A' }}</span
    ><span *ngIf="!fileInfo.originalFileName">N/A</span>`
})
export class MassUserCreationFileNameRendererComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  params: any;
  fileInfo: FileInfoListViewModel;
  resultId: number;
  constructor(
    private globalLoader: CxGlobalLoaderService,
    private fileInfoApiService: FileInfoApiService,
    private translateAdapterService: TranslateAdapterService,
    private toastrService: ToastrService,
    private reportsDataService: ReportsDataService
  ) {
    super();
  }

  // called on init
  agInit(params: any): void {
    this.params = params;
    if (params && params.data) {
      this.fileInfo = params.data as FileInfoListViewModel;
      this.resultId = this.params.value.resultId;
    }
  }

  refresh(params: any): boolean {
    this.params = params;

    return true;
  }

  onClickItem(originFileName: string, filePath: string): void {
    this.downloadFile(originFileName, filePath);
  }

  private async downloadFile(originFileName: string, filePath: string) {
    this.reportsDataService
      .checkFileExists(filePath)
      .subscribe((fileExists) => {
        if (fileExists) {
          const fileName = this.buildSuffixFileName(originFileName);
          this.reportsDataService.downloadFile(filePath, fileName);
        } else {
          console.error(`Not found file path '${filePath}'`);
          this.toastrService.error(
            this.translateAdapterService.getValueImmediately(
              'RequestErrorMessage.FileNotFound'
            )
          );
        }
      });
  }

  private buildSuffixFileName(fileName: string): string {
    return `${fileName}-${new Date().toISOString()}.${FileTypeExtension.CSV}`;
  }
}
