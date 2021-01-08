import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {
  ChangeDetectionStrategy,
  EventEmitter,
  Injectable,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { CxCommonModule, OAuthService } from '@conexus/cx-angular-common';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { UserIdleService } from 'angular-user-idle';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { AuthService } from 'app-auth/auth.service';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { DeviceDetectorService } from 'ngx-device-detector';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { AppComponent } from './app.component';
import { AppService } from './app.service';
@Injectable()
export class ActivatedRouteStub {
  private subject = new BehaviorSubject({ params: { searchValue: '' } });

  get queryParams() {
    return this.subject;
  }
}

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        RouterTestingModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot(),
        CxCommonModule,
        CommonModule,
        NgbModalModule,
      ],
      providers: [
        HttpHelpers,
        AppService,
        {
          provide: DeviceDetectorService,
          useValue: {},
        },
        {
          provide: TranslateAdapterService,
          useValue: {},
        },
        {
          provide: AuthService,
          useValue: {
            userData: new BehaviorSubject(null),
            loadDiscoveryDocumentAndTryLogin: () => new Promise(() => {}),
          },
        },
        {
          provide: OAuthService,
          useValue: {},
        },
        {
          provide: TranslateService,
          useValue: {},
        },
        {
          provide: UserIdleService,
          useValue: {},
        },
        {
          provide: BreadcrumbSettingService,
          useValue: {
            changeBreadcrumbEvent: new EventEmitter(),
          },
        },
        {
          provide: CxSurveyjsExtendedService,
          useValue: {
            setAPIVariables: () => {},
            setCurrentUserVariables: () => {},
            setCurrentDepartmentVariables: () => {},
            setPDCatalogueVariables: () => {},
          },
        },
        ToastrService,
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).overrideComponent(AppComponent, {
      set: { changeDetection: ChangeDetectionStrategy.Default },
    });
    TestBed.overrideTemplate(AppComponent, '<div></div>');
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.debugElement.componentInstance;
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });
  // describe('getUserInfo', () => {
  //   it('should start listening notification if user data is available', () => {
  //     const mockCurrentUser = { id: 1 };
  //     (component as any).finishedInitFirebaseCloudMessaging = false;
  //     const userDataSpy = spyOn(
  //       (component as any).authService,
  //       'userData'
  //     ).and.returnValue(new BehaviorSubject(mockCurrentUser));
  //     const initFirebaseCloudMessageServiceSpy = spyOn(
  //       component as any,
  //       'initFirebaseCloudMessageService'
  //     ).and.returnValue(new Promise(() => {}));
  //     const initDataSpy = spyOn(component as any, 'initData');
  //     const setupUserIdleDetectionSpy = spyOn(
  //       component as any,
  //       'setupUserIdleDetection'
  //     );
  //     const getAllNotificationDataSpy = spyOn(
  //       component as any,
  //       'getAllNotificationData'
  //     );
  //     (component as any).getUserInfo();
  //     expect(userDataSpy).toHaveBeenCalled();
  //     expect(initDataSpy).toHaveBeenCalled();
  //     expect(setupUserIdleDetectionSpy).toHaveBeenCalled();
  //     expect(initFirebaseCloudMessageServiceSpy).toHaveBeenCalled();
  //     expect(getAllNotificationDataSpy).toHaveBeenCalled();
  //   });

  //   it('should load document and try login is not available', () => {
  //     const mockCurrentUser = undefined;
  //     spyOn((component as any).authService, 'userData').and.returnValue(
  //       new BehaviorSubject(mockCurrentUser)
  //     );
  //     const loadDiscoveryDocumentAndTryLoginSpy = spyOn(
  //       (component as any).authService,
  //       'loadDiscoveryDocumentAndTryLogin'
  //     ).and.returnValue(Promise.resolve());

  //     (component as any).getUserInfo();
  //     loadDiscoveryDocumentAndTryLoginSpy.calls
  //       .mostRecent()
  //       .returnValue.then(() => {
  //         expect(loadDiscoveryDocumentAndTryLoginSpy).toHaveBeenCalled();
  //       });
  //   });
  // });
});
