import { BaseFormComponent, ModuleFacadeService, UploadParameters } from '@opal20/infrastructure';
import { Component, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  DigitalContentDetailMode,
  DigitalContentDetailViewModel,
  FileUploaderHelpers,
  PersonalFileDialogComponent,
  PreviewDigitalContentComponent
} from '@opal20/domain-components';
import { FileUploaderUtils, OpalDialogService } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { IDigitalContent } from '@opal20/domain-api';

@Component({
  selector: 'digital-upload-content-editor-page',
  templateUrl: './digital-upload-content-editor-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalUploadContentEditorComponent extends BaseFormComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;

  @Input() public mode: DigitalContentDetailMode = DigitalContentDetailMode.View;
  @Input() public contentViewModel: DigitalContentDetailViewModel;
  @Input() public saveUploadContentCallback: () => Promise<IDigitalContent>;

  @ViewChild(PreviewDigitalContentComponent, { static: false })
  public previewDigitalContentComponent: PreviewDigitalContentComponent;

  constructor(protected moduleFacadeService: ModuleFacadeService, private opalDialogService: OpalDialogService) {
    super(moduleFacadeService);
  }

  public onReplaceBtnClicked(): void {
    if (this.contentViewModel.chapters) {
      this.moduleFacadeService.modalService.showConfirmMessage(
        'The annotation of the video will be ereased including the comments. Are you sure you want to continue?',
        this.onReplaceBtnConfirmed.bind(this)
      );
      return;
    }
    this.onReplaceBtnConfirmed();
  }

  public get canShowReplaceButton(): boolean {
    return this.mode === DigitalContentDetailMode.Edit || this.mode === DigitalContentDetailMode.Create;
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return this.previewDigitalContentComponent.validate();
  }

  private onReplaceBtnConfirmed(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFileDialogComponent);
    (dialogRef.content.instance as PersonalFileDialogComponent).maxFileCount = 1;
    (dialogRef.content.instance as PersonalFileDialogComponent).icon = 'assets/images/icons/add-file.svg';
    (dialogRef.content.instance as PersonalFileDialogComponent).uploadFolder = 'digital-contents';
    (dialogRef.content.instance as PersonalFileDialogComponent).settings.extensions = FileUploaderUtils.uploadContentAllowedExtensions;
    // Overide default keydown kendo handler
    dialogRef.dialog.instance.onComponentKeydown = () => {
      (dialogRef.content.instance as PersonalFileDialogComponent).onCancel();
    };

    dialogRef.result.toPromise().then((uploadParameters: UploadParameters[]) => {
      this.contentViewModel.data.fileName = uploadParameters[0].file ? uploadParameters[0].file.name : uploadParameters[0].fileName;
      this.contentViewModel.data.fileExtension = uploadParameters[0].fileExtension;
      this.contentViewModel.data.fileSize = uploadParameters[0].file ? uploadParameters[0].file.size : uploadParameters[0].fileSize;
      this.contentViewModel.data.fileType = FileUploaderHelpers.getFileType(uploadParameters[0].fileExtension);
      this.contentViewModel.data.fileLocation = uploadParameters[0].fileLocation;
      this.contentViewModel.chapters.splice(0, this.contentViewModel.data.chapters.length);
      this.saveUploadContentCallback().then(content => this.previewDigitalContentComponent.onContentLoaded());
    });
  }
}
