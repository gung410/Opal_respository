import { RouterModule, Routes } from '@angular/router';

import { AssignmentDetailPageComponent } from './components/assignment-detail-page.component';
import { ClassRunDetailPageComponent } from './components/classrun-detail-page.component';
import { CourseDetailPageComponent } from './components/course-detail-page.component';
import { CourseManagementPageComponent } from './components/course-management-page.component';
import { DigitalBadgesManagementPageComponent } from './components/digital-badges-management-page.component';
import { LMMRoutePaths } from '@opal20/domain-components';
import { LearnerProfilePageComponent } from './components/learner-profile-page.component';
import { LearningPathDetailPageComponent } from './components/learning-path-detail-page.component';
import { LearningPathManagementPageComponent } from './components/learning-path-management-page.component';
import { NgModule } from '@angular/core';
import { ParticipantAssignmentTrackDetailPageComponent } from './components/participant-assignment-track-page.component';
import { ReportsPageComponent } from './components/reports-page.component';
import { SessionDetailPageComponent } from './components/session-detail-page.component';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
const routes: Routes = [
  {
    path: '',
    // If you update the page for default route, please update the default route in navigation page service
    component: CourseManagementPageComponent
  },
  {
    path: LMMRoutePaths.CourseManagementPage,
    component: CourseManagementPageComponent
  },
  {
    path: LMMRoutePaths.LearningPathManagementPage,
    component: LearningPathManagementPageComponent
  },
  {
    path: LMMRoutePaths.CourseDetailPage,
    component: CourseDetailPageComponent
  },
  {
    path: LMMRoutePaths.SessionDetailPage,
    component: SessionDetailPageComponent
  },
  {
    path: LMMRoutePaths.ClassRunDetailPage,
    component: ClassRunDetailPageComponent
  },
  {
    path: LMMRoutePaths.AssignmentDetailPage,
    component: AssignmentDetailPageComponent
  },
  {
    path: LMMRoutePaths.LearningPathDetailPage,
    component: LearningPathDetailPageComponent
  },
  {
    path: LMMRoutePaths.LearnerProfilePage,
    component: LearnerProfilePageComponent
  },
  {
    path: LMMRoutePaths.ReportsPage,
    component: ReportsPageComponent
  },
  {
    path: LMMRoutePaths.ParticipantAssignmentTrackPage,
    component: ParticipantAssignmentTrackDetailPageComponent
  },
  {
    path: LMMRoutePaths.DigitalBadgesManagementPage,
    component: DigitalBadgesManagementPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class LMMRoutingModule {}
