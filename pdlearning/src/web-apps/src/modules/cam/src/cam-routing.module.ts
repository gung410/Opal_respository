import { RouterModule, Routes } from '@angular/router';

import { BlockoutDateDetailPageComponent } from './components/blockout-date-detail-page.component';
import { CAMRoutePaths } from '@opal20/domain-components';
import { ClassRunDetailPageComponent } from './components/classrun-detail-page.component';
import { CourseDetailPageComponent } from './components/course-detail-page.component';
import { CourseManagementPageComponent } from './components/course-management-page.component';
import { CoursePlanningCycleDetailPageComponent } from './components/course-planning-cycle-detail-page.component';
import { CoursePlanningPageComponent } from './components/course-planning-page.component';
import { ECertificateManagementPageComponent } from './components/ecertificate-management-page.component';
import { ECertificateTemplateDetailPageComponent } from './components/ecertificate-template-detail-page.component';
import { LearnerProfilePageComponent } from './components/learner-profile-page.component';
import { NgModule } from '@angular/core';
import { ReportsPageComponent } from './components/reports-page.component';
import { SessionDetailPageComponent } from './components/session-detail-page.component';
import { VenueManagementPageComponent } from './components/venue-management-page.component';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
const routes: Routes = [
  {
    path: '',
    // If you update the page for default route, please update the default route in navigation page service
    component: CoursePlanningPageComponent
  },
  {
    path: CAMRoutePaths.CoursePlanningPage,
    component: CoursePlanningPageComponent
  },
  {
    path: CAMRoutePaths.CourseManagementPage,
    component: CourseManagementPageComponent
  },
  {
    path: CAMRoutePaths.VenueManagementPage,
    component: VenueManagementPageComponent
  },
  {
    path: CAMRoutePaths.CourseDetailPage,
    component: CourseDetailPageComponent
  },
  {
    path: CAMRoutePaths.ClassRunDetailPage,
    component: ClassRunDetailPageComponent
  },
  {
    path: CAMRoutePaths.SessionDetailPage,
    component: SessionDetailPageComponent
  },
  {
    path: CAMRoutePaths.LearnerProfilePage,
    component: LearnerProfilePageComponent
  },
  {
    path: CAMRoutePaths.CoursePlanningCycleDetailPage,
    component: CoursePlanningCycleDetailPageComponent
  },
  {
    path: CAMRoutePaths.ReportsPage,
    component: ReportsPageComponent
  },
  {
    path: CAMRoutePaths.BlockoutDateDetailPage,
    component: BlockoutDateDetailPageComponent
  },
  {
    path: CAMRoutePaths.ECertificateManagementPage,
    component: ECertificateManagementPageComponent
  },
  {
    path: CAMRoutePaths.ECertificateTemplateDetailPage,
    component: ECertificateTemplateDetailPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class CAMRoutingModule {}
