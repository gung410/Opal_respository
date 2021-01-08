import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'common-app',
  template: `
    <router-outlet></router-outlet>
  `
})
export class CommonComponent {
  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('class.overflow-initial')
  public getOverflowInitiallass(): boolean {
    return true;
  }
}
