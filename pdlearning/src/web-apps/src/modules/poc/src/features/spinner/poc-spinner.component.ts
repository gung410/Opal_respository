import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { delay } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'spinner',
  template: `
    <h3>Spinner Component</h3>
    <button (click)="showGlobal()">Show Global</button>
    <div class="spinner-wrapper" spinner></div>
    <button (click)="show()">Show Local</button>
    <button (click)="hide()">Hide Local</button>
  `
})
export class POCSpinnerComponent extends BasePageComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showGlobal(): void {
    this.moduleFacadeService.globalSpinnerService.show();
    of(true)
      .pipe(delay(2000))
      .subscribe(() => this.moduleFacadeService.globalSpinnerService.hide());
  }

  public show(): void {
    this.moduleFacadeService.spinnerService.show();
  }

  public hide(): void {
    this.moduleFacadeService.spinnerService.hide();
  }
}
