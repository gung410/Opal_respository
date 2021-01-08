import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { FileRestrictions } from '@progress/kendo-angular-upload';
import { IDownloadTemplateOption } from '../../models/download-template-file.model';
@Component({
  selector: 'select-files-dialog',
  templateUrl: './select-files-dialog.component.html'
})
export class SelectFilesDialogComponent extends BaseComponent {
  public selectedFile: File | null;
  @Input() public downloadTemplateOption?: IDownloadTemplateOption<unknown>;

  public fileRestrictions: FileRestrictions = {
    allowedExtensions: ['.csv', '.xls', '.xlsx']
  };

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onImport(): void {
    this.dialogRef.close(this.selectedFile);
  }

  public onDownloadTemplate(templateFileFormatType: unknown): void {
    if (this.downloadTemplateOption != null) {
      this.downloadTemplateOption.downloadTemplateFn(templateFileFormatType);
    }
  }
}
