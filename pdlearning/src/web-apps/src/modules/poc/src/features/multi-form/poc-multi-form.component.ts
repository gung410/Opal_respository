import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'multi-form',
  template: `
    <h3>Multi Form Component</h3>
    We can share common forms between module. Updating...
  `
})
export class POCMultiFormComponent extends BaseFormComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
