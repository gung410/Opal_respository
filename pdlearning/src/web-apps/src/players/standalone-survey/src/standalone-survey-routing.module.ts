import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { StandaloneSurveyDetailPageComponent } from './components/standalone-survey-detail-page.component';
import { StandaloneSurveyLearningPageComponent } from './components/standalone-survey-learning-page.component';
import { StandaloneSurveyPlayerPageComponent } from './components/standalone-survey-player-page.component';
import { StandaloneSurveyRepositoryPageComponent } from './components/standalone-survey-repository-page.component';
import { StandaloneSurveyRoutePaths } from '@opal20/domain-components';

const routes: Routes = [
  {
    path: StandaloneSurveyRoutePaths.RepositoryPage,
    component: StandaloneSurveyRepositoryPageComponent
  },
  {
    path: StandaloneSurveyRoutePaths.LearningPage,
    component: StandaloneSurveyLearningPageComponent
  },
  {
    path: StandaloneSurveyRoutePaths.DetailPage,
    component: StandaloneSurveyDetailPageComponent
  },
  {
    path: StandaloneSurveyRoutePaths.PlayerPage,
    component: StandaloneSurveyPlayerPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class StandaloneSurveyRoutingModule {}
