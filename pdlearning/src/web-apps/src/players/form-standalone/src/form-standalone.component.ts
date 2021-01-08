import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'form-standalone',
  template: `
    <router-outlet></router-outlet>
  `
})
export class FormStandaloneComponent extends BaseComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
