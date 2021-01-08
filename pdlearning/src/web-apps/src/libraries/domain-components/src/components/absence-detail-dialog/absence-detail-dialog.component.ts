import { AmazonS3ApiService, BaseComponent, IGetFileResult, ModuleFacadeService, Utils } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'absence-detail-dialog',
  templateUrl: './absence-detail-dialog.component.html'
})
export class AbsenceDetailDialogComponent extends BaseComponent {
  public title: string = '';
  public reasonForAbsence: string = '';

  public get attachment(): string[] {
    return this._attachmentUrls;
  }

  public set attachment(value: string[]) {
    if (!Utils.isDifferent(value, this._attachmentUrls)) {
      return;
    }
    this._attachmentUrls = value;
    this.loadFilesFromCloudfront();
  }

  public cloudfrontAttachments: Dictionary<IGetFileResult> = {};
  public fileLocationFromCloudfrontURLs: string[] = [];

  private _attachmentUrls: string[];

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, public amazonS3ApiService: AmazonS3ApiService) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  public getFileName(path: string): string {
    return Utils.getFileNameFromPath(path);
  }

  public getAttachmentUrl(path: string): string {
    return this.cloudfrontAttachments[path] ? this.cloudfrontAttachments[path].url : '';
  }

  private loadFilesFromCloudfront(): void {
    const attachmentUrls = this.attachment.map(p => this.getFileLocationFromCloudfrontURL(p));
    this.amazonS3ApiService.getFiles(attachmentUrls).then(result => {
      this.cloudfrontAttachments = Utils.toDictionary(result, p => `${AppGlobal.environment.cloudfrontUrl}/${p.key}`);
    });
  }

  private getFileLocationFromCloudfrontURL(url: string): string {
    if (url == null) {
      return '';
    }
    // +1 to remove the slash at the end of the domain.
    const substringStart = AppGlobal.environment.cloudfrontUrl.length + 1;
    return url.substring(substringStart, url.length + 1);
  }
}
