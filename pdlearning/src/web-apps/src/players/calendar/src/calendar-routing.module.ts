import { RouterModule, Routes } from '@angular/router';

import { CalendarPlayerComponent } from './components/calendar-player/calendar-player.component';
import { CalendarRoutePaths } from './calendar.config';
import { CommunityEventDetailFormComponent } from './components/community-event-detail-form/community-event-detail-form.component';
import { NgModule } from '@angular/core';
import { TeamCalendarContainerComponent } from './components/team-calendar-container/team-calendar-container.component';

const routes: Routes = [
  {
    path: CalendarRoutePaths.CommunityEventDetail,
    component: CommunityEventDetailFormComponent
  },
  {
    path: CalendarRoutePaths.CalendarPlayer,
    component: CalendarPlayerComponent
  },
  {
    path: CalendarRoutePaths.TeamCalendar,
    component: TeamCalendarContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class CalendarRoutingModule {}
