import { ModuleWithProviders, NgModule } from '@angular/core';

import { CalendarIntergrationService } from './services/calendar-intergration.service';
import { CalendarRepositoryContext } from './calendar-repository-context';
import { CalendarSwitcherService } from './services/calendar-switcher-service';
import { CommunityApiService } from './services/community-api.service';
import { CommunityCalendarApiService } from './services/community-calendar-api.service';
import { PersonalCalendarApiService } from './services/personal-calendar-api.service';
import { TeamCalendarAOApiService } from './services/team-calendar-AO-api.service';
import { TeamCalendarApiResolverService } from './services/team-calendar-api-resolver.service';
import { TeamCalendarContextService } from './services/team-calendar-context-service';
import { TeamCalendarLearnerApiService } from './services/team-calendar-learner-api.service';
import { TeamCalendarRepository } from './repositories/team-calendar.repository';
import { TeamCalendarSharingService } from './services/team-calendar-sharing-api.service';
import { TeamCalendarSwitchViewService } from './services/team-calendar-switch-view-service';

@NgModule({
  providers: [
    CalendarIntergrationService,
    TeamCalendarRepository,
    CalendarSwitcherService,
    CommunityApiService,
    CommunityCalendarApiService,
    PersonalCalendarApiService,
    TeamCalendarContextService,
    TeamCalendarSharingService,
    TeamCalendarSwitchViewService,
    TeamCalendarAOApiService,
    TeamCalendarLearnerApiService,
    TeamCalendarApiResolverService
  ]
})
export class CalendarDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: CalendarDomainApiModule,
      providers: [CalendarRepositoryContext]
    };
  }
}
