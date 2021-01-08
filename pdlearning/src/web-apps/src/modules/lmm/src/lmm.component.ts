import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'lmm-app',
  template: `
    <router-outlet></router-outlet>
  `
})
export class LMMComponent extends BasePageComponent {
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('class.lmm-module')
  public getContentClass(): boolean {
    return true;
  }
}
