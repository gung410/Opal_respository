import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { POCDialogComponent } from './poc-dialog.component';

@Component({
  selector: 'dialog-page',
  template: `
    <h3>Dialog Page</h3>
    <button (click)="showDialog()">Show Dialog</button>
  `
})
export class POCDialogPageComponent extends BasePageComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showDialog(): void {
    this.moduleFacadeService.windowService.open({
      title: 'Dialog Title',
      content: POCDialogComponent
    });
  }
}
