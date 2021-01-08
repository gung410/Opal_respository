import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'venue-management-page',
  templateUrl: './venue-management-page.component.html'
})
export class VenueManagementPageComponent extends BasePageComponent {
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
