import { RouterModule, Routes } from '@angular/router';

import { CATALOGUE_TYPE_ENUM } from './constants/catalogue-type.constant';
import { CalendarPageComponent } from './components/calendar-page.component';
import { EPortfolioPageComponent } from './components/e-porfolio-page.component';
import { LearnerCataloguePageComponent } from './components/learner-catalogue-page.component';
import { LearnerHomePageComponent } from './components/learner-home-page.component';
import { LearnerMyAchievementsPageComponent } from './components/learner-my-achievements-page.component';
import { LearnerMyLearningPageComponent } from './components/learner-my-learning-page.component';
import { LearnerPdPlanPageComponent } from './components/learner-pdplan-page.component';
import { LearnerPermissionGuardService } from './user-activities-tracking/learner-permission-guard.service';
import { LearnerRoutePaths } from '@opal20/domain-components';
import { LearningDetailPage } from './components/learning-detail-page.component';
import { MY_ACHIEVEMENT_TYPE_ENUM } from './constants/my-achievement.constant';
import { NgModule } from '@angular/core';
import { ReportsPageComponent } from './components/reports-page.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: LearnerRoutePaths.Home,
    pathMatch: 'full'
  },
  {
    path: LearnerRoutePaths.Home,
    component: LearnerHomePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.MyLearning,
    component: LearnerMyLearningPageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.Catalogue,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  // keep this one for the sub menus of catalogue
  {
    path: `${LearnerRoutePaths.Catalogue}/servicescheme/:serviceschemeid/subject/:subid`,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: `${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.AllCourses}`,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: `${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.Courses}`,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: `${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.Microlearning}`,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: `${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.DigitalContent}`,
    component: LearnerCataloguePageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.Calendar,
    component: CalendarPageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.MyAchievements,
    component: LearnerMyAchievementsPageComponent
  },
  {
    path: `${LearnerRoutePaths.MyAchievements}/${MY_ACHIEVEMENT_TYPE_ENUM.ECertificates}`,
    component: LearnerMyAchievementsPageComponent
  },
  {
    path: `${LearnerRoutePaths.MyAchievements}/${MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges}`,
    component: LearnerMyAchievementsPageComponent
  },
  {
    path: LearnerRoutePaths.PdPlan,
    component: LearnerPdPlanPageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.EPortfolio,
    component: EPortfolioPageComponent,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.Detail,
    component: LearningDetailPage,
    canActivate: [LearnerPermissionGuardService]
  },
  {
    path: LearnerRoutePaths.ReportsPage,
    component: ReportsPageComponent,
    canActivate: [LearnerPermissionGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class LearnerRoutingModule {}
