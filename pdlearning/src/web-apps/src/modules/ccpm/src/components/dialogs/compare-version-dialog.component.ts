import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'compare-version-dialog',
  templateUrl: './compare-version-dialog.component.html'
})
export class CompareVersionDialogComponent extends BaseComponent {
  @Input() public title: string = '';

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
}
