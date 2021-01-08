import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppConstant } from 'app/shared/app.constant';
import { LearningPlanDetailComponent } from './learning-plan-detail/learning-plan-detail.component';
import { LearningPlanListComponent } from './learning-plan-list/learning-plan-list.component';
import { OverallOrganizationalDevelopmentComponent } from './overall-organizational-development/overall-organizational-development.component';
import { StrategicThrustsComponent } from './strategic-thrusts/strategic-thrusts.component';

export const routes: Routes = [
  {
    path: AppConstant.siteURL.menus.strategicThrusts,
    component: StrategicThrustsComponent,
  },
  {
    path: '',
    component: LearningPlanListComponent,
  },
  {
    path: 'plan-detail/:extId',
    component: LearningPlanDetailComponent,
  },
  {
    path: 'overall-org-plan/:extId',
    component: OverallOrganizationalDevelopmentComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrganisationalDevelopmentRoutingModule {}
