import { BatchJobsMonitoringModule } from './batch-jobs-monitoring/batch-jobs-monitoring.module';
import {
  HTTP_INTERCEPTORS,
  HttpClient,
  HttpClientModule
} from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { AngularFireModule } from '@angular/fire';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTabsModule } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  CxCommonModule,
  CxInformationDialogService,
  CxLoaderModule,
  CxLoaderModuleConfig,
  CxLoaderUI,
  CxSurveyjsService
} from '@conexus/cx-angular-common';
import {
  NgbDropdownModule,
  NgbModalModule,
  NgbModule
} from '@ng-bootstrap/ng-bootstrap';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { UserIdleModule } from 'angular-user-idle';
import { AngularFireAuthModule } from 'angularfire2/auth';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { environment } from 'app-environments/environment';
import { FcmPushService } from 'app-services/fcm-push.service';
import { NotificationDataService } from 'app-services/notification-data.service';
import { NotificationService } from 'app-services/notification.service';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { UnsupportedFcmPushService } from 'app-services/unsupported-fcm-push.service';
import { AuthHttpInterceptorService } from 'app-utilities/auth-http-interceptor.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import * as firebase from 'firebase/app';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { SystemAuditLogModule } from './system-audit-log/system-audit-log.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BroadcastMessagesModule } from './broadcast-messages/broadcast-messages.module';
import { AppCoreModule } from './core/app-core.module';
import { ErrorPageModule } from './error-page/error-page.module';
import { AppHeaderComponent } from './header/app-header.component';
import { LoginModule } from './login/login.module';
import { ManagementModule } from './management/management.module';
import { MonitorModule } from './monitor/monitor.module';
import { PermissionsModule } from './permissions/permissions.module';
import { ReportIframeComponent } from './report-iframe/report-iframe.component';
import { ReportsModule } from './reports/reports.module';
import { SessionTimeoutPageComponent } from './session-timeout-page/session-timeout-page.component';
import { AppConstant } from './shared/app.constant';
import { AppSettingService } from './shared/services/app-setting.service';
import { TaxonomyManagementModule } from './taxonomy-management/taxonomy-management.module';
import { UserAccountsModule } from './user-accounts/user-accounts.module';
import { UserGroupsModule } from './user-groups/user-groups.module';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}

export function loaderConfigFactory(): CxLoaderModuleConfig {
  const loaderModuleConfig = new CxLoaderModuleConfig();
  loaderModuleConfig.loaderUi = new CxLoaderUI({
    circleBackgroundColor: AppConstant.theme.mainBackgroundColor
  });

  return loaderModuleConfig;
}

@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent,
    SessionTimeoutPageComponent,
    ReportIframeComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    LoginModule,
    CxCommonModule,
    MonitorModule,
    ManagementModule,
    ErrorPageModule,
    HttpClientModule,
    MatNativeDateModule,
    NgbDropdownModule,
    OAuthAdapterModule.forRoot(),
    CxLoaderModule,
    NgbModule,
    AppCoreModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    NgbModalModule,
    MatTabsModule,
    UserAccountsModule,
    ToastrModule.forRoot(),
    BroadcastMessagesModule,
    PermissionsModule,
    SystemAuditLogModule,
    BatchJobsMonitoringModule,
    ReportsModule,
    UserGroupsModule,
    TaxonomyManagementModule,
    AngularFireModule.initializeApp(AppConstant.firebase.fcmConfig),
    AngularFireAuthModule,
    UserIdleModule.forRoot({
      idle: environment.UserIdleTimeOut,
      timeout: environment.SessionTimeoutCountdown,
      ping: 0
    }),
    OAuthAdapterModule.forRoot()
  ],
  providers: [
    HttpHelpers,
    CxInformationDialogService,
    CxSurveyjsService,
    AppSettingService,
    ToastrAdapterService,
    {
      provide: CxLoaderModuleConfig,
      useFactory: loaderConfigFactory
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptorService,
      multi: true
    },
    {
      provide: NotificationService,
      useFactory: (
        toastrService: ToastrService,
        notificationDataService: NotificationDataService
      ) => {
        if (
          'Notification' in window &&
          environment.notification.enableToggleToFireBase
        ) {
          return new FcmPushService(toastrService, notificationDataService);
        }

        return new UnsupportedFcmPushService(toastrService);
      },
      deps: [ToastrService, NotificationDataService]
    }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
  constructor() {
    firebase.initializeApp(AppConstant.firebase.fcmConfig);
  }
}
