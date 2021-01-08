import { CommonModule } from '@angular/common';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { Injectable, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import {
  CxCommonModule,
  CxFormModal,
  CxGlobalLoaderService,
  CxLoaderModule,
  CxLoaderUI,
} from '@conexus/cx-angular-common';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import {
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore,
  USE_DEFAULT_LANG,
  USE_STORE,
} from '@ngx-translate/core';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { AuthService } from 'app-auth/auth.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { StaffListService } from '../staff-list.service';
import { HttpHelpers } from './../../../shared/utilities/httpHelpers';
import { LearningNeedAnalysisReminderService } from './reminder-dialog/reminder.service';
import { StaffListComponent } from './staff-list.component';

@Injectable()
export class ActivatedRouteStub {
  private subject = new BehaviorSubject({ params: { searchValue: '' } });

  get queryParams() {
    return this.subject;
  }
}

@NgModule({
  declarations: [StaffListComponent],
  imports: [
    NgbModalModule,
    CommonModule,
    NoopAnimationsModule,
    CxLoaderModule.forRoot({
      loaderUi: new CxLoaderUI(),
    }),
  ],
  exports: [StaffListComponent],
  entryComponents: [StaffListComponent],
  schemas: [NO_ERRORS_SCHEMA],
})
class TestModule {}
describe('UserAccountComponent', () => {
  let fixture: ComponentFixture<StaffListComponent>;
  let component: StaffListComponent;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        RouterTestingModule,
        ToastrModule.forRoot(),
        CxCommonModule,
        CommonModule,
        TestModule,
      ],
      providers: [
        CxGlobalLoaderService,
        StaffListService,
        LearningNeedAnalysisReminderService,
        {
          provide: AuthService,
          useValue: {
            userData: () => new BehaviorSubject(null),
          },
        },
        CxFormModal,
        UserService,
        HttpHelpers,
        [{ provide: USE_STORE, useValue: {} }],
        [{ provide: USE_DEFAULT_LANG, useValue: {} }],
        MissingTranslationHandler,
        TranslateParser,
        TranslateCompiler,
        TranslateLoader,
        TranslateStore,
        TranslateService,
        [
          {
            provide: ActivatedRoute,
            useValue: new ActivatedRouteStub(),
          },
        ],
        [
          {
            provide: Router,
            useValue: { navigate: () => {} },
          },
        ],
        ToastrService,
      ],
      schemas: [NO_ERRORS_SCHEMA],
    });
    TestBed.overrideTemplate(StaffListComponent, '<div></div>');
  }));

  describe('onSelectedDepartmentClick:', () => {
    function setup() {
      const staffListService = TestBed.get(StaffListService);
      const reminderService = TestBed.get(LearningNeedAnalysisReminderService);
      const httpMock = TestBed.get(HttpTestingController);
      const formModal = TestBed.get(CxFormModal);
      const translateService = TestBed.get(TranslateAdapterService);

      return {
        fixture,
        component,
        staffListService,
        reminderService,
        httpMock,
        formModal,
        translateService,
      };
    }
    beforeEach(() => {
      fixture = TestBed.createComponent(StaffListComponent);
      component = fixture.componentInstance;
    });

    it('should create StaffList component', () => {
      expect(component).toBeTruthy();
    });
  });
});
