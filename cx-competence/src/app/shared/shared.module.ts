import { AgGridModule } from '@ag-grid-community/angular';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { ChartModule, HIGHCHARTS_MODULES } from 'angular-highcharts';
import { AuthGuardService } from 'app-authguards/auth-guard.service';
import { CommentService } from 'app-services/comment.service';
import { IdpService } from 'app-services/idp.service';
import { IDPService } from 'app-services/idp/idp.service';
import { ExternalPDOService } from 'app-services/idp/pd-catalogue/external-pdo.service';
import { PdCatalogueService } from 'app-services/idp/pd-catalogue/pd-catalogue.service';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { TitleAsKeyPipe } from 'app-utilities/titleAsKey.pipe';
import { NominationFileNameRendererComponent } from 'app/approval-page/ag-grid-renderer/nomination-file-name-renderer.component';
import { MobileAuthService } from 'app/mobile/services/mobile-auth.service';
import { MassNominationResultListComponent } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/adhoc-mass-nomination/mass-nomination-list/mass-nomination-result-list.component';
import { MassNominationPanelComponent } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/adhoc-mass-nomination/mass-nomination-panel/mass-nomination-panel.component';
import { MassNominationResultsComponent } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/adhoc-mass-nomination/mass-nomination-results/mass-nomination-results.component';
import * as more from 'highcharts/highcharts-more.src';
import { CourseDetailModule } from './components/course-detail/course-detail.module';
import { CxBackButtonComponent } from './components/cx-back-button/cx-back-button.component';
import { CxGanttChartModule } from './components/cx-gantt-chart/cx-gantt-chart.module';
import { CxLazySurveyjsComponent } from './components/cx-lazy-surveyjs/cx-lazy-surveyjs.component';
import { CxSelectModule } from './components/cx-select/cx-select.module';
import { DownloadFileComponent } from './components/download-file/download-file.component';
import { IconButtonComponent } from './components/icon-button/icon-button.component';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { MeatBallActionDropdownComponent } from './components/meat-ball-action-dropdown/meat-ball-action-dropdown.component';
import { NominateStatusComponent } from './components/nominate-status/nominate-status.component';
import { PDOLongCardModule } from './components/pdo-long-card/pdo-long-card.module';
import { StarRatingWidgetComponent } from './components/star-rating-widget/star-rating-widget.component';
import { TabComponent } from './components/tab/tab.component';

@NgModule({
  declarations: [
    IconButtonComponent,
    LanguageSelectorComponent,
    TabComponent,
    TitleAsKeyPipe,
    StarRatingWidgetComponent,
    NominateStatusComponent,
    DownloadFileComponent,
    MassNominationPanelComponent,
    MassNominationResultsComponent,
    MassNominationResultListComponent,
    NominationFileNameRendererComponent,
    CxLazySurveyjsComponent,
    MeatBallActionDropdownComponent,
    CxBackButtonComponent,
  ],
  imports: [
    CommonModule,
    CxCommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    RouterModule,
    ChartModule,
    CxSelectModule,
    MatRadioModule,
    CxGanttChartModule,
    PDOLongCardModule,
    CourseDetailModule,
    AgGridModule.withComponents([NominationFileNameRendererComponent]),
    NgbModule,
  ],
  providers: [
    AuthGuardService,
    TranslateAdapterService,
    UserService,
    ImageHelpers,
    IdpService,
    IDPService,
    PdPlannerService,
    PdCatalogueService,
    ExternalPDOService,
    CommentService,
    MobileAuthService,
    { provide: HIGHCHARTS_MODULES, useFactory: () => [more] },
  ],
  exports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    RouterModule,
    CxGanttChartModule,
    PDOLongCardModule,
    CourseDetailModule,
    IconButtonComponent,
    LanguageSelectorComponent,
    NominateStatusComponent,
    TabComponent,
    StarRatingWidgetComponent,
    MassNominationPanelComponent,
    MassNominationResultsComponent,
    CxLazySurveyjsComponent,
    MeatBallActionDropdownComponent,
    CxBackButtonComponent,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SharedModule {}
