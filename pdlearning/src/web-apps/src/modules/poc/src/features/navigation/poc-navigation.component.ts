import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { BehaviorSubject } from 'rxjs';
import { Component } from '@angular/core';

@Component({
  selector: 'navigation',
  template: `
    <h3>Navigation Component</h3>
    Recieve data: {{ message$ | async }}
    <br />
    Send data: <input [(ngModel)]="name" />
    <button (click)="navigate()">Navigate</button>
  `
})
export class POCNavigationComponent extends BasePageComponent {
  public message$: BehaviorSubject<string> = new BehaviorSubject(JSON.stringify(this.getNavigateData()));
  public name: string = 'Navigation';

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public navigate(): void {
    this.navigateTo('navigation-data', { from: this.name });
  }
}
