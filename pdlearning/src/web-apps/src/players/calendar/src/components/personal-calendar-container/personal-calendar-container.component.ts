import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { TeamCalendarApiResolverService, TeamCalendarRepository, TeamCalendarViewMode } from '@opal20/domain-api';

@Component({
  selector: 'personal-calendar-container',
  templateUrl: './personal-calendar-container.component.html'
})
export class PersonalCalendarContainerComponent extends BasePageComponent {
  @Input() public calendarHeight: string = '100%';
  public existSharedTeams: boolean = false;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private teamCalendarRepository: TeamCalendarRepository,
    private teamCalendarApiResolverService: TeamCalendarApiResolverService
  ) {
    super(moduleFacadeService);
    this.checkSharedTeamsExist();
  }

  private checkSharedTeamsExist(): void {
    this.subscribe(this.teamCalendarRepository.getMySharedTeams(), res => {
      this.existSharedTeams = res && res.length > 0;
      this.teamCalendarApiResolverService.initViewMode(TeamCalendarViewMode.LearnerView, res[0].accessShareId);
    });
  }
}
