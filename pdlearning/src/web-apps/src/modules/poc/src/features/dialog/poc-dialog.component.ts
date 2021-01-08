import { BaseDialogComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { WindowRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'dialog-component',
  template: `
    <h3>This is dialog component</h3>
    <button (click)="close()">Close this</button>
  `
})
export class POCDialogComponent extends BaseDialogComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected windowRef: WindowRef) {
    super(moduleFacadeService, windowRef);
  }
}
