import { AlertDialogComponent } from './alert-dialog/alert-dialog.component';
import { SharedModule } from './../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { PlannedActivitiesComponent } from './planned-activities/planned-activities.component';
import { AppConstant } from 'app/shared/app.constant';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { PDPlannerComponent } from './pd-planner.component';
import { LearningNeedAnalysisMobileComponent } from './learning-need-analysis-mobile/learning-need-analysis-mobile.component';
import {
  CxCommonModule,
  CxInformationDialogService,
} from '@conexus/cx-angular-common';
import { ProgressBarComponent } from './shared/progress-bar/progress-bar.component';
import { ActionDialogComponent } from './action-dialog/action-dialog.component';
import { LearningNeedService } from 'app-services/idp/learning-need/learning-needs.service';
import { StepSubmittedComponent } from './learning-need-analysis-mobile/step-submitted/step-submitted.component';
import { MobileAuthService } from '../services/mobile-auth.service';
import { LearningNeedsAnalysisModule } from 'app/individual-development/learning-needs-analysis/learning-needs-analysis.module';
import { LearningNeedsModule } from 'app/individual-development/learning-needs/learning-needs.module';
import { LearningNeedMobileComponent } from './learning-need-mobile/learning-need-mobile.component';
import { PdOpportunitiesModule } from 'app/individual-development/pd-opportunities/pd-opportunities.module';
import { PDPlannerPageHeaderComponent } from './shared/pd-planner-page-header/pd-planner-page-header.component';
import { IndividualDevelopmentModule } from 'app/individual-development/individual-development.module';
import { AngularResizedEventModule } from 'angular-resize-event';

export const routes: Routes = [
  {
    path: '',
    component: PDPlannerComponent,
    data: { animation: 'PDPlanner' },
  },
  {
    path: AppConstant.mobileUrl.plannedActivities,
    component: PlannedActivitiesComponent,
    data: { animation: 'PlannedActivities' },
  },
  {
    path: AppConstant.mobileUrl.learningNeed,
    component: LearningNeedMobileComponent,
    data: { animation: 'LearningNeed' },
  },
  {
    path: AppConstant.mobileUrl.learningNeedAnalysis,
    component: LearningNeedAnalysisMobileComponent,
    data: {
      animation: 'LearningNeedAnalysis',
    },
  },
];

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}
@NgModule({
  imports: [
    CommonModule,
    CxCommonModule,
    SharedModule,
    LearningNeedsAnalysisModule,
    LearningNeedsModule,
    PdOpportunitiesModule,
    IndividualDevelopmentModule,
    AngularResizedEventModule,
    RouterModule.forChild(routes),
  ],
  declarations: [
    PDPlannerComponent,
    LearningNeedAnalysisMobileComponent,
    LearningNeedMobileComponent,
    PlannedActivitiesComponent,
    StepSubmittedComponent,
    PDPlannerPageHeaderComponent,
    AlertDialogComponent,
    ProgressBarComponent,
    ActionDialogComponent,
  ],
  providers: [LearningNeedService, CxInformationDialogService],
})
export class PDPlannerModule {}
