import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'learning-my-achievement-dialog',
  templateUrl: './learning-my-achievements-dialog.component.html'
})
export class MyAchievementsDialogComponent extends BaseComponent {
  @Input() public onDownloadFn?: () => void;
  @Input() public safeUrl: SafeUrl;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onDownload(): void {
    if (this.onDownloadFn) {
      this.onDownloadFn();
    }
    this.dialogRef.close();
  }
}
