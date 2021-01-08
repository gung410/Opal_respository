import { AmazonS3UploaderService, BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FileType, PersonalFileModel } from '@opal20/domain-api';
import { PlatformHelper } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { FileUploaderHelpers } from '../../helpers/file-uploader.helper';

@Component({
  selector: 'personal-file-preview-dialog',
  templateUrl: './personal-file-preview-dialog.component.html'
})
export class PersonalFilePreviewDialogComponent extends BaseFormComponent {
  @Input() public personalFileVm: PersonalFileModel;

  public contentUrl: string;
  public isShowMessagePreventCookie: boolean = false;
  public fileExtension: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private uploaderService: AmazonS3UploaderService
  ) {
    super(moduleFacadeService);
  }

  public get canShowDocument(): boolean {
    return this.canShow(['pdf', 'docx', 'xlsx', 'pptx']);
  }

  public get canShowAudio(): boolean {
    return this.canShow(['mp3', 'ogg']);
  }

  public get canShowVideo(): boolean {
    return this.canShow(['mp4', 'm4v', 'ogv']);
  }

  public get canShowScorm(): boolean {
    return this.canShow(['scorm']);
  }

  public get canShowImage(): boolean {
    return this.canShow(['jpeg', 'jpg', 'gif', 'png', 'svg']);
  }

  public onInit(): void {
    this.processData();
  }

  public processData(): void {
    this.fileExtension = this.personalFileVm.fileExtension;
    // Use Amazon CloudFront query string parameters authentication instead of signed cookies,
    // because we can't pass cookies to Google document viewer.
    if (this.canShowDocument) {
      this.uploaderService.getFile(this.personalFileVm.fileLocation).then(url => {
        this.contentUrl = url;
      });
    }

    if (this.canShowImage || this.canShowVideo || this.canShowAudio || this.canShowImage || this.canShowScorm) {
      this.contentUrl = `${AppGlobal.environment.cloudfrontUrl}/${this.personalFileVm.fileLocation}`;
    }
  }

  public handleImgError(event: Event): void {
    this.isShowMessagePreventCookie = event.type === 'error' && PlatformHelper.isIOSDevice() ? true : false;
  }

  public getFileTypeDisplay(type: FileType): string {
    return FileUploaderHelpers.getFileTypeDisplay(type);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  private canShow(extensions: string[]): boolean {
    return extensions.some(item => item === this.fileExtension);
  }
}
