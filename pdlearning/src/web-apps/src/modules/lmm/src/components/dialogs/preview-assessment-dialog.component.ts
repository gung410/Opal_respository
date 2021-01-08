import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { PreviewMode } from '@opal20/domain-components';

@Component({
  selector: 'preview-assessment-dialog',
  templateUrl: './preview-assessment-dialog.component.html'
})
export class PreviewAssessmentDialogComponent extends BaseComponent {
  @Input() public assessmentId: string;

  public previewOptions = [PreviewMode.Web, PreviewMode.Mobile];
  public PreviewMode: typeof PreviewMode = PreviewMode;
  public previewMode: PreviewMode = PreviewMode.Web;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public closePreview(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }
}
