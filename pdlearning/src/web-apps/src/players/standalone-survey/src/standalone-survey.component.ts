import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'standalone-survey',
  template: `
    <router-outlet></router-outlet>
  `
})
export class StandaloneSurveyComponent extends BaseComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
