import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { TeamCalendarApiResolverService, TeamCalendarViewMode } from '@opal20/domain-api';

import { Component } from '@angular/core';

@Component({
  selector: 'team-calendar-container',
  templateUrl: './team-calendar-container.component.html'
})
export class TeamCalendarContainerComponent extends BasePageComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService, private teamCalendarApiResolverService: TeamCalendarApiResolverService) {
    super(moduleFacadeService);
    this.teamCalendarApiResolverService.initViewMode(TeamCalendarViewMode.AOView);
  }
}
