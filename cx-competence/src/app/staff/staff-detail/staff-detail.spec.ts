/* tslint:disable:no-unused-variable */
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ChangeDetectionStrategy, NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute, Router } from '@angular/router';
import { CxLoaderModule, CxLoaderUI } from '@conexus/cx-angular-common';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { AuthService } from 'app-auth/auth.service';
import { LnaResultModel } from 'app-models/mpj/idp.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { IdpService } from 'app-services/idp.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { LearningNeedService } from 'app/core/store-data-services/learning-need.services';
import { LearningNeedStoreService } from 'app/core/store-services/learning-need-store.service';
import { ToastrModule } from 'ngx-toastr';
import { BehaviorSubject, of } from 'rxjs';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import { StaffDetailDataService } from './staff-detail-data.service';
import { StaffDetailComponent } from './staff-detail.component';

export class TranslateAdapterServiceStub {
  getValueImmediately(any?: string) {
    return '';
  }
}

describe('StaffDetailComponent', () => {
  let component: StaffDetailComponent;
  let fixture: ComponentFixture<StaffDetailComponent>;
  let userService, staffDetailDataService;
  const fakeActivatedRoute = {
    snapshot: { data: {} },
    params: of({ id: '0' }),
  };
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        ToastrModule.forRoot(),
        NgbModalModule,
        TranslateModule.forRoot(),
        BrowserAnimationsModule,
        CxLoaderModule.forRoot({
          loaderUi: new CxLoaderUI(),
        }),
      ],
      declarations: [StaffDetailComponent],
      providers: [
        CxSurveyjsExtendedService,
        {
          provide: AuthService,
          useValue: {
            userData: () => new BehaviorSubject({}),
          },
        },
        {
          provide: TranslateAdapterService,
          useClass: TranslateAdapterServiceStub,
        },
        [{ provide: ActivatedRoute, useValue: fakeActivatedRoute }],
        HttpHelpers,
        UserService,
        ImageHelpers,
        StaffDetailDataService,
        LearningNeedService,
        LearningNeedStoreService,
        {
          provide: Router,
          useClass: class {
            navigate = jasmine.createSpy('navigate');
          },
        },
        IdpService,
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).overrideComponent(StaffDetailComponent, {
      set: { changeDetection: ChangeDetectionStrategy.Default },
    });
    TestBed.overrideTemplate(StaffDetailComponent, '<div></div>');
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffDetailComponent);
    component = fixture.componentInstance;

    userService = fixture.debugElement.injector.get(UserService);
    staffDetailDataService = fixture.debugElement.injector.get(
      StaffDetailDataService
    );
  });

  describe('Component', () => {
    it('should be created', () => {
      expect(component).toBeTruthy();
    });
  });
  describe('getLnaResultWithAnswer', () => {
    let mockLnaResultParam: LnaResultModel;

    // TODO: Update this test script to prevent test error.
    // it('should not load lna result answer and learning opportunities', () => {
    //     const staffDetailDataServiceSpy = spyOn(staffDetailDataService, 'getLnaResultWithAnswer');
    //     mockLnaResultParam = new LnaResultModel();
    //     mockLnaResultParam.assessmentStatusInfo = new AssessmentStatusInfo();
    //     mockLnaResultParam.assessmentStatusInfo.assessmentStatusCode = IdpStatusCodeEnum.NotStarted;
    //     (component as any).getLnaResultWithAnswer(mockLnaResultParam, 0);
    //     mockLnaResultParam.assessmentStatusInfo.assessmentStatusCode = IdpStatusCodeEnum.Started;
    //     (component as any).getLnaResultWithAnswer(mockLnaResultParam, 0);
    //     mockLnaResultParam.assessmentStatusInfo.assessmentStatusCode = IdpStatusCodeEnum.NotAdded;
    //     (component as any).getLnaResultWithAnswer(mockLnaResultParam, 0);
    //     expect(staffDetailDataServiceSpy).toHaveBeenCalledTimes(0);
    // });

    // TODO: Update this test script to prevent test error.
    // it('should load lna result answer and learning opportunities', () => {
    //     const getLnaResultWithAnswerSpy = spyOn(staffDetailDataService, 'getLnaResultWithAnswer')
    //         .and
    //         .returnValue(of({}));
    //     const getListLearningOpportunitiesSpy = spyOn<any>(component, 'getListLearningOpportunities')
    //         .and.returnValue(Promise.resolve());
    //     mockLnaResultParam = new LnaResultModel();
    //     mockLnaResultParam.assessmentStatusInfo = new AssessmentStatusInfo();
    //     mockLnaResultParam.assessmentStatusInfo.assessmentStatusCode = IdpStatusCodeEnum.Approved;
    //     mockLnaResultParam.surveyInfo = {
    //         startDate: new Date().toString()
    //     };
    //     mockLnaResultParam.resultIdentity = { id: 0 } as Identity;
    //     (component as any).getLnaResultWithAnswer(mockLnaResultParam, 0);
    //     fixture.whenStable()
    //         .then(() => {
    //             expect(getLnaResultWithAnswerSpy).toHaveBeenCalled();
    //         });
    // });
  });

  describe('ngOnInit', () => {
    let activatedRoute: ActivatedRoute;
    let authService: AuthService;
    beforeEach(() => {
      activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
      authService = fixture.debugElement.injector.get(AuthService);
    });

    // TODO: Update this test script to prevent test error.
    // it('should get no answer Lna result', () => {
    //     spyOn<any>(component, 'getData').and.callFake(() => {
    //         const getNoAnswerLnaResultsSpy = spyOn<any>(staffDetailDataService, 'getNoAnswerLnaResults')
    //             .and
    //             .returnValue(of([]));
    //         component.ngOnInit();
    //         expect(getNoAnswerLnaResultsSpy).toHaveBeenCalled();
    //     });
    // });

    // TODO: Update this test script to prevent test error.
    // it('should get Lna result with answer', () => {
    //     spyOn<any>(component, 'getData').and.callFake(() => {
    //         const getNoAnswerLnaResultsSpy = spyOn<any>(staffDetailDataService, 'getNoAnswerLnaResults')
    //             .and
    //             .returnValue(of([{}]));
    //         const getLnaResultWithAnswerSpy = spyOn<any>(component, 'getLnaResultWithAnswer');
    //         component.ngOnInit();
    //         expect(getNoAnswerLnaResultsSpy).toHaveBeenCalled();
    //         expect(getLnaResultWithAnswerSpy).toHaveBeenCalled();
    //     });
    // });

    // TODO: Update this test script to prevent test error.
    // it('should get Lna config after getting user data', () => {
    //     spyOn<any>(staffDetailDataService, 'getNoAnswerLnaResults')
    //         .and
    //         .returnValue(of([{}]));
    //     spyOn<any>(component, 'getLnaResultWithAnswer');
    //     spyOn(authService, 'userData')
    //         .and
    //         .returnValue(new BehaviorSubject({}));
    //     const getLnaConfigSpy = spyOn<any>(component, 'getLnaConfig');
    //     component.ngOnInit();
    //     expect(getLnaConfigSpy).toHaveBeenCalled();
    // });
  });

  // TODO: Update this test script to prevent test error.
  // describe('getLnaConfig', () => {
  //     it('should modify the returned configuration', () => {
  //         const learningNeedDataService = fixture.debugElement.injector.get(LearningNeedService);
  //         spyOn(learningNeedDataService, 'getLearningNeedConfig')
  //             .and
  //             .returnValue(of({ configuration: {} }));
  //         component.ngOnInit();
  //         expect(component.configLna.showPageAsTab).toEqual(true);
  //         expect(component.configLna.mode).toEqual('display');
  //     });
  // });

  // TODO: Update this test script to prevent test error.
  // describe('onPeriodChange', () => {
  //     it('should get Lna result with answer when switch period', () => {
  //         const checkedChangeEvent = {
  //             target: {
  //                 checked: true
  //             }
  //         };
  //         const mockChangedLnaResult = {};
  //         component.staffDetail = {
  //             identity: {
  //                 id: 0
  //             }
  //         };
  //         const getLnaResultWithAnswerSpy = spyOn<any>(component, 'getLnaResultWithAnswer');
  //         component.onPeriodChange(checkedChangeEvent, mockChangedLnaResult as LnaResultModel);
  //         expect(component.currentLnaResult).toEqual(mockChangedLnaResult);
  //         expect(getLnaResultWithAnswerSpy).toHaveBeenCalled();
  //     });
  //     it('should not get Lna result with answer', () => {
  //         const checkedChangeEvent = {
  //             target: {
  //                 checked: false
  //             }
  //         };
  //         const mockChangedLnaResult = {};
  //         const getLnaResultWithAnswerSpy = spyOn<any>(component, 'getLnaResultWithAnswer');
  //         component.onPeriodChange(checkedChangeEvent, mockChangedLnaResult as LnaResultModel);
  //         expect(component.currentLnaResult).not.toEqual(mockChangedLnaResult);
  //         expect(getLnaResultWithAnswerSpy).toHaveBeenCalledTimes(0);
  //     });
  // });
  // describe('getStaffDetailContent', () => {
  //     let mockUser;
  //     beforeEach(() => {
  //         mockUser = { id: 1, email: '', assessmentInfo: { identity: { id: 1 } } };
  //     });
  //     it('should get a single user', () => {
  //         const mockLna = { result: {} };
  //         spyOn(userService, 'getListEmployee').and.returnValue(of({ items: [mockUser] }));
  //         const lnaSpy = spyOn(staffDetailDataService, 'getLnaResult').and.returnValue(of(mockLna));
  //         spyOn((component as any), 'getPdPlanPageConfigCombinedWithResultMap').and.returnValue(of({}));
  //         component.getStaffDetailContent(0, {});
  //         expect(userService.getListEmployee).toHaveBeenCalledBefore(
  //             lnaSpy);
  //         expect((component as any).getPdPlanPageConfigCombinedWithResultMap).toHaveBeenCalled();
  //         expect(component.loading).toEqual(false);
  //     });

  //     it('should set loading status to false when getListEmployee failed', () => {
  //         spyOn(userService, 'getListEmployee').and.callFake(() => {
  //             return throwError('Cannot get employees list');
  //         });
  //         component.getStaffDetailContent(0, {});
  //         expect(userService.getListEmployee).toHaveBeenCalled();
  //         expect(component.loading).toEqual(false);
  //     });

  //     it('should set loading status to false when get Lna Result failed', () => {
  //         spyOn(userService, 'getListEmployee').and.returnValue(of({ items: [mockUser] }));

  //         spyOn(staffDetailDataService, 'getLnaResult').and.callFake(() => {
  //             return throwError('Cannot get lna result');
  //         });
  //         component.getStaffDetailContent(0, {});
  //         expect(userService.getListEmployee).toHaveBeenCalled();
  //         expect(staffDetailDataService.getLnaResult).toHaveBeenCalled();
  //         expect(component.loading).toEqual(false);
  //     });
  // });
});
