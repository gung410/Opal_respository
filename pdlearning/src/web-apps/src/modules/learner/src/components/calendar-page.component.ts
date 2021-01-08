import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { CalendarIntergrationService } from '@opal20/domain-api';
import { Component } from '@angular/core';
import { LearnerRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'calendar-page',
  templateUrl: './calendar-page.component.html'
})
export class CalendarPageComponent extends BasePageComponent {
  constructor(public moduleFacadeService: ModuleFacadeService, private calendarIntergrationService: CalendarIntergrationService) {
    super(moduleFacadeService);
    this.calendarIntergrationService.setInternalMode(true);
  }

  public ngOnInit(): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Calendar}`);
  }
}
