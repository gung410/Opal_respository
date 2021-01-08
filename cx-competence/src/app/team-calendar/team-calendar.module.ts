import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { SharedModule } from 'app/shared/shared.module';
import { TeamCalendarPageComponent } from './team-calendar.component';

@NgModule({
  declarations: [TeamCalendarPageComponent],
  imports: [
    CommonModule,
    CxCommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: TeamCalendarPageComponent }]),
  ],
})
export class TeamCalendarModule {}
