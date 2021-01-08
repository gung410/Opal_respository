import { ClientSideRowModelModule } from '@ag-grid-community/client-side-row-model';
import { ModuleRegistry } from '@ag-grid-community/core';
import {
  HttpClient,
  HttpClientModule,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  CxCommonModule,
  CxInformationDialogService,
  CxLoaderModule,
  CxLoaderModuleConfig,
  CxLoaderUI,
  CxSurveyjsService,
} from '@conexus/cx-angular-common';
import { NgbModalModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { UserIdleModule } from 'angular-user-idle';
import { AngularFireAuth } from 'angularfire2/auth';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { environment } from 'app-environments/environment';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { LocalScheduleService } from 'app-services/local-schedule.service';
import { ScheduleService } from 'app-services/schedule.service';
import { AuthHttpInterceptorService } from 'app-utilities/auth-http-interceptor.service';
import { BrowserIdleHandler } from 'app-utilities/browser-idle.handler';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { ToastrModule } from 'ngx-toastr';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppCoreModule } from './core/app-core.module';
import { ErrorPageModule } from './error-page/error-page.module';
import { LoginModule } from './login/login.module';
import { ManagementModule } from './management/management.module';
import { PDPlannerModule } from './mobile/pd-planner/pd-planner.module';
import { MonitorModule } from './monitor/monitor.module';
import { OrganisationalDevelopmentModule } from './organisational-development/organisational-development.module';
import { ReviewMyProfessionalDevelopmentJourneyComponent } from './professional-growth/review-my-professional-development-journey/review-my-professional-development-journey.component';
import { ProfileComponent } from './profile/profile.component';
import { ReportIframeComponent } from './report-iframe/report-iframe.component';
import { SessionTimeoutPageComponent } from './session-timeout-page/session-timeout-page.component';
import { AppConstant } from './shared/app.constant';
import { FixedHeaderComponent } from './shared/components/fixed-header/fixed-header.component';
import { StickyNavbarComponent } from './shared/components/sticky-navbar/sticky-navbar.component';
import { CommentServiceHelpers } from './shared/helpers/comment-service.helpers';
import { AppSettingService } from './shared/services/app-setting.service';
import { SharedModule } from './shared/shared.module';
import { StaffListService } from './staff/staff.container/staff-list.service';
import { LearningNeedAnalysisReminderService } from './staff/staff.container/staff-list/reminder-dialog/reminder.service';
import { MyCalendarComponent } from './training-calendar/my-calendar/my-calendar.component';
import { TeamCalendarComponent } from './training-calendar/team-calendar/team-calendar.component';
// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient): any {
  return new TranslateHttpLoader(http);
}
export function loaderConfigFactory(): any {
  const loaderModuleConfig = new CxLoaderModuleConfig();
  loaderModuleConfig.loaderUi = new CxLoaderUI({
    circleBackgroundColor: AppConstant.theme.mainBackgroundColor,
    overlayTopPositionInPx: 0,
  });

  return loaderModuleConfig;
}

@NgModule({
  declarations: [
    AppComponent,
    StickyNavbarComponent,
    FixedHeaderComponent,
    MyCalendarComponent,
    TeamCalendarComponent,
    ReviewMyProfessionalDevelopmentJourneyComponent,
    SessionTimeoutPageComponent,
    ProfileComponent,
    ReportIframeComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    LoginModule,
    SharedModule,
    CxCommonModule,
    MonitorModule,
    ManagementModule,
    OrganisationalDevelopmentModule,
    PDPlannerModule,
    ErrorPageModule,
    HttpClientModule,
    CxLoaderModule,
    OAuthAdapterModule.forRoot(),
    NgbModule,
    AppCoreModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),
    NgbModalModule,
    MatTabsModule,
    ToastrModule.forRoot({
      maxOpened: 4,
      autoDismiss: true,
    }),
    UserIdleModule.forRoot({
      idle: environment.userIdleTimeOut,
      timeout: environment.sessionTimeoutCountdown,
      ping: 0,
    }),
    NgIdleKeepaliveModule,
  ],
  providers: [
    HttpHelpers,
    CxInformationDialogService,
    CxSurveyjsService,
    AppSettingService,
    StaffListService,
    LearningNeedAnalysisReminderService,
    BreadcrumbSettingService,
    {
      provide: CxLoaderModuleConfig,
      useFactory: loaderConfigFactory,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptorService,
      multi: true,
    },
    AngularFireAuth,
    CommentServiceHelpers,
    BrowserIdleHandler,
    {
      provide: LocalScheduleService,
      deps: [BrowserIdleHandler],
      useValue: undefined,
    },
    {
      provide: ScheduleService,
      useExisting: LocalScheduleService,
    },
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule {
  constructor() {
    ModuleRegistry.register(ClientSideRowModelModule);
  }
}
