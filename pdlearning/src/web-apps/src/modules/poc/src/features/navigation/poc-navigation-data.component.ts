import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { BehaviorSubject } from 'rxjs';
import { Component } from '@angular/core';

@Component({
  selector: 'context-data',
  template: `
    <h3>Navigation Data Component</h3>
    Recieve data: {{ message$ | async }}
    <br />
    Send data: <input [(ngModel)]="name" />
    <button (click)="navigate()">Navigate</button>
  `
})
export class POCNavigationDataComponent extends BasePageComponent {
  public message$: BehaviorSubject<string> = new BehaviorSubject(JSON.stringify(this.getNavigateData()));
  public name: string = 'Navigation Data';

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public navigate(): void {
    this.navigateTo('navigation', { from: this.name });
  }
}
