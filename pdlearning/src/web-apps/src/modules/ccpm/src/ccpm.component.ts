import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { NavigationMenuService } from '@opal20/domain-components';

@Component({
  selector: 'ccpm-app',
  template: `
    <router-outlet></router-outlet>
  `
})
export class CCPMComponent extends BasePageComponent {
  constructor(public moduleFacadeService: ModuleFacadeService, public navigationMenuService: NavigationMenuService) {
    super(moduleFacadeService);
  }

  public openPreviewer(): void {
    this.moduleFacadeService.navigationService.navigateTo('mobile-previewer');
  }
}
