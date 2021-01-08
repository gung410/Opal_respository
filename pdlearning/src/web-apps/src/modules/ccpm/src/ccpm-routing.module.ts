import { RouterModule, Routes } from '@angular/router';

import { CCPMRoutePaths } from './ccpm.config';
import { DigitalContentDetailPageComponent } from './components/digital-content-detail-page.component';
import { DigitalContentRepositoryPageComponent } from './components/digital-content-repository-page.component';
import { DigitalLearningContentEditorPageComponent } from './components/digital-learning-content-editor-page.component';
import { DigitalUploadContentEditorComponent } from './components/digital-upload-content-editor-page.component';
import { FormDetailPageComponent } from './components/form-detail-page.component';
import { FormEditorPageComponent } from './components/form-editor-page.component';
import { FormRepositoryPageComponent } from './components/form-repository-page.component';
import { NgModule } from '@angular/core';
import { PersonalSpaceRepositoryPageComponent } from './components/personal-space-repository-page.component';
import { QuestionBankRepositoryPageComponent } from './components/question-bank-repository-page.component';
import { ReportsPageComponent } from './components/reports-page.component';
import { StandaloneSurveyDetailPageComponent } from './components/standalone-survey-detail-page.component';
import { StandaloneSurveyRepositoryPageComponent } from './components/standalone-survey-repository-page.component';

const routes: Routes = [
  {
    path: CCPMRoutePaths.DigitalContentRepository,
    component: DigitalContentRepositoryPageComponent
  },
  {
    path: CCPMRoutePaths.DigitalLearningContentEditor,
    component: DigitalLearningContentEditorPageComponent
  },
  {
    path: CCPMRoutePaths.DigitalUploadContentEditor,
    component: DigitalUploadContentEditorComponent
  },
  {
    path: CCPMRoutePaths.FormRepository,
    component: FormRepositoryPageComponent
  },
  {
    path: CCPMRoutePaths.FormDetail,
    component: FormDetailPageComponent
  },
  {
    path: CCPMRoutePaths.FormEditor,
    component: FormEditorPageComponent
  },
  {
    path: CCPMRoutePaths.DigitalContentDetailPage,
    component: DigitalContentDetailPageComponent
  },
  {
    path: CCPMRoutePaths.ReportsPage,
    component: ReportsPageComponent
  },
  {
    path: CCPMRoutePaths.StandaloneSurveyRepository,
    component: StandaloneSurveyRepositoryPageComponent
  },
  {
    path: CCPMRoutePaths.StandaloneSurveyDetailPage,
    component: StandaloneSurveyDetailPageComponent
  },
  {
    path: CCPMRoutePaths.PersonalSpaceRepository,
    component: PersonalSpaceRepositoryPageComponent
  },
  {
    path: CCPMRoutePaths.QuestionBankRepository,
    component: QuestionBankRepositoryPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class CCPMRoutingModule {}
