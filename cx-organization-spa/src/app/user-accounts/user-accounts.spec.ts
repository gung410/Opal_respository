import { CommonModule } from '@angular/common';
import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';
import {
  EventEmitter,
  Injectable,
  NgModule,
  NO_ERRORS_SCHEMA
} from '@angular/core';
import {
  async,
  ComponentFixture,
  fakeAsync,
  TestBed,
  tick
} from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import {
  CxCommonModule,
  CxDialogTemplateComponent,
  CxFormModal,
  CxLoaderModule,
  CxLoaderUI,
  CxTableContainersComponent
} from '@conexus/cx-angular-common';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import {
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore,
  USE_DEFAULT_LANG,
  USE_STORE
} from '@ngx-translate/core';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { AuthService } from 'app-auth/auth.service';
import { NotificationDataService } from 'app-services/notification-data.service';
import { NotificationService } from 'app-services/notification.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { CareerPathsDataService } from 'app/core/store-data-services/career-paths-data.service';
import { DevelopmentalRolesDataService } from 'app/core/store-data-services/developmental-roles-data';
import { LearningFrameWorksDataService } from 'app/core/store-data-services/learning-frame-works-data';
import { PersonnelGroupsDataService } from 'app/core/store-data-services/personnel-groups-data.service';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { CareerPathsStoreService } from 'app/core/store-services/career-paths-data.service';
import { DevelopmentalRolesStoreService } from 'app/core/store-services/developmental-roles-store.service';
import { LearningFrameWorksStoreService } from 'app/core/store-services/learning-frame-works-store.service';
import { PDCatalogueStoreService } from 'app/core/store-services/pd-catalogue-store.service';
import { PersonnelGroupsStoreService } from 'app/core/store-services/personnel-groups-store.service';
import { SystemRolesStoreService } from 'app/core/store-services/system-roles-store.service';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import {
  StatusReasonTypeConstant,
  StatusTypeEnum
} from 'app/shared/constants/user-status-type.enum';
import * as moment from 'moment';
import { ToastrModule } from 'ngx-toastr';
import { BehaviorSubject, identity, Observable, of } from 'rxjs';

import { AppConstant } from 'app/shared/app.constant';
import { EditUserDialogComponent } from './edit-user-dialog/edit-user-dialog.component';
import {
  pagingUserAccount,
  UserAccountMockData
} from './mock-data/user-account-mock-data';
import { UserManagementQueryModel } from './models/user-management.model';
import { PDCatalogueDataService } from './services/pd-catalouge-data.service';
import { StatusHistoricalDataPanelComponent } from './status-historical-data-panel/status-historical-data-panel.component';
import { UserAccountConfirmationDialogComponent } from './user-account-confirmation-dialog/user-account-confirmation-dialog.component';
import { UserAccountsDataService } from './user-accounts-data.service';
import { UserAccountsComponent } from './user-accounts.component';
import { CheckingUserRolesService } from './services/checking-user-roles.service';

@Injectable()
export class ActivatedRouteStub {
  private subject: BehaviorSubject<{
    params: { searchValue: string };
  }> = new BehaviorSubject({ params: { searchValue: '' } });

  get queryParams(): BehaviorSubject<{ params: { searchValue: string } }> {
    return this.subject;
  }
}

// tslint:disable:max-classes-per-file
export class TranslateServiceStub {
  get(key: any): any {
    return Observable.of(key);
  }

  getCurrentLanguage(): string {
    return 'Norsk';
  }

  getValueBasedOnKey(key: string | string[]): Observable<any> {
    return Observable.create();
  }

  getValueImmediately(key: string): string {
    return '';
  }
}

@NgModule({
  declarations: [EditUserDialogComponent],
  imports: [
    NgbModalModule,
    CommonModule,
    NoopAnimationsModule,
    CxLoaderModule.forRoot({
      loaderUi: new CxLoaderUI()
    })
  ],
  exports: [EditUserDialogComponent],
  entryComponents: [
    EditUserDialogComponent,
    UserAccountConfirmationDialogComponent
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
class TestModule {}
describe('UserAccountComponent', () => {
  let fixture: ComponentFixture<UserAccountsComponent>;
  let component: UserAccountsComponent;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        RouterTestingModule,
        TestModule,
        ToastrModule.forRoot(),
        CxCommonModule,
        CommonModule,
        MatTabsModule,
        NgSelectModule,
        FormsModule
      ],
      declarations: [
        UserAccountsComponent,
        UserAccountConfirmationDialogComponent,
        StatusHistoricalDataPanelComponent
      ],
      providers: [
        UserAccountsDataService,
        NotificationDataService,
        NotificationService,
        AuthService,
        CxFormModal,
        HttpHelpers,
        [{ provide: USE_STORE, useValue: {} }],
        [{ provide: USE_DEFAULT_LANG, useValue: {} }],
        MissingTranslationHandler,
        TranslateParser,
        TranslateCompiler,
        TranslateLoader,
        TranslateStore,
        TranslateService,
        CheckingUserRolesService,
        SystemRolesDataService,
        SystemRolesStoreService,
        PersonnelGroupsDataService,
        PersonnelGroupsStoreService,
        CareerPathsDataService,
        CareerPathsStoreService,
        PDCatalogueDataService,
        PDCatalogueStoreService,
        DevelopmentalRolesDataService,
        DevelopmentalRolesStoreService,
        LearningFrameWorksDataService,
        LearningFrameWorksStoreService,
        [
          {
            provide: ActivatedRoute,
            useValue: new ActivatedRouteStub()
          }
        ],
        [
          {
            provide: TranslateAdapterService,
            useValue: new TranslateServiceStub()
          }
        ]
      ],
      schemas: [NO_ERRORS_SCHEMA]
    });
    TestBed.overrideTemplate(UserAccountsComponent, '<div></div>');
    TestBed.overrideTemplate(CxTableContainersComponent, '<div></div>');
    TestBed.overrideTemplate(CxDialogTemplateComponent, '<div></div>');
    TestBed.overrideTemplate(EditUserDialogComponent, '<div></div>');
    TestBed.overrideTemplate(
      UserAccountConfirmationDialogComponent,
      '<div></div>'
    );
  }));

  describe('onSelectedDepartmentClick:', () => {
    function setup(): any {
      const fixturePopup = TestBed.createComponent(
        UserAccountConfirmationDialogComponent
      );
      const componentPopup = fixturePopup.componentInstance;
      const userAccountDataService = TestBed.get(UserAccountsDataService);
      const httpMock = TestBed.get(HttpTestingController);
      const formModal = TestBed.get(CxFormModal);
      const translateService = TestBed.get(TranslateAdapterService);

      return {
        fixture,
        component,
        userAccountDataService,
        httpMock,
        formModal,
        translateService,
        fixturePopup,
        componentPopup
      };
    }
    beforeEach(() => {
      fixture = TestBed.createComponent(UserAccountsComponent);
      component = fixture.componentInstance;
    });

    it('should create UserAccount component', () => {
      expect(component).toBeTruthy();
    });

    it('should call api through service to get employee and departments when clicking department', () => {
      component.breadCrumbNavigation = [];
      component.isDisplayOrganisationNavigation = true;
      const userAccountDataService = fixture.debugElement.injector.get(
        UserAccountsDataService
      );
      const selectedDepartment = {
        identity: {
          extId: 'HRMS001',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'DataOwner',
          id: 14350
        },
        name: 'MOE'
      };
      const paramModel = new UserManagementQueryModel({
        externallyMastered: undefined,
        filterOnParentHd: true,
        orderBy: '',
        pageIndex: 1,
        pageSize: 100,
        parentDepartmentId: [1],
        userEntityStatuses: [],
        searchKey: ''
      });

      spyOn(userAccountDataService, 'getUsers').and.returnValue(
        of(UserAccountMockData.mockEmployeesData)
      );
      component.onSelectedDepartmentClick(selectedDepartment);
      fixture.detectChanges();
      expect(userAccountDataService.getUsers(paramModel));
    });
  });

  describe('edit user', () => {
    let mockSelectedUser;
    let fixture;
    let component: UserAccountsComponent;
    let componentPopup;
    let userAccountsDataService: UserAccountsDataService;
    function setup(): any {
      const fixture = TestBed.createComponent(UserAccountsComponent);
      const component = fixture.componentInstance;
      const fixturePopup = TestBed.createComponent(
        UserAccountConfirmationDialogComponent
      );
      const componentPopup = fixturePopup.componentInstance;

      return { fixture, component, componentPopup };
    }
    beforeEach(() => {
      mockSelectedUser = {
        identity: {
          id: 'selectedUserId'
        },
        firstName: '',
        systemRoles: [],
        emailAddress: '',
        mobileNumber: '',
        mobileCountryCode: '',
        ssn: '',
        gender: 0,
        entityStatus: {
          expirationDate: new Date()
        },
        dateOfBirth: '',
        personnelGroups: [],
        careerPaths: []
      };
      ({ fixture, component, componentPopup } = setup());
      userAccountsDataService = fixture.debugElement.injector.get(
        UserAccountsDataService
      );
    });
    it('should set title for form modal buttons', () => {
      const cxFormModal = fixture.debugElement.injector.get(CxFormModal);
      (component as any).initEditForm();
      expect(cxFormModal.cancelName).toEqual('Cancel');
      expect(cxFormModal.submitName).toEqual('Save');
    });

    it('should open edit user popup when click edit button', () => {
      component.currentUser = {
        ...mockSelectedUser,
        identity: {
          id: 'currentUserId'
        },
        systemRoles: []
      };
      component.userItemsData = pagingUserAccount;
      (component as any).editFormJson =
        UserAccountMockData.editFormJsonMockData;
      (component as any).editFormMoeJson =
        UserAccountMockData.editFormJsonMockData;
      const ngbModal = fixture.debugElement.injector.get(NgbModal);
      const selectedUser = mockSelectedUser;
      component.userItemsData.items = [selectedUser];
      spyOn(userAccountsDataService, 'getUsers').and.returnValue(
        of(UserAccountMockData.mockEmployeesData)
      );
      spyOn(ngbModal, 'hasOpenModals').and.returnValue(false);
      spyOn(ngbModal, 'open').and.returnValue({
        result: new Promise((resolve) => {
          resolve();
        }),
        componentInstance: {
          user: undefined,
          employeeAvatarFromEmailsMap: undefined,
          fullUserInfoJsonData: undefined,
          surveyjsOptions: undefined,
          basicInfoJsonTemplate: undefined,
          advanceInfoJsonTemplate: undefined,
          submit: new EventEmitter(),
          cancel: new EventEmitter()
        }
      });
      component.onEditUserClicked(selectedUser, true);
      expect(ngbModal.open).toHaveBeenCalled();
    });
  });

  describe('create new user', () => {
    let mockSelectedUser;
    // tslint:disable:no-shadowed-variable
    let fixture;
    let component: UserAccountsComponent;
    let userAccountsDataService: UserAccountsDataService;
    beforeEach(() => {
      fixture = TestBed.createComponent(UserAccountsComponent);
      component = fixture.componentInstance;
      userAccountsDataService = fixture.debugElement.injector.get(
        UserAccountsDataService
      );
      mockSelectedUser = {
        identity: {
          id: 'selectedUserId'
        },
        firstName: '',
        systemRoles: [],
        emailAddress: '',
        mobileNumber: '',
        mobileCountryCode: '',
        ssn: '',
        gender: 0,
        entityStatus: {
          expirationDate: new Date()
        },
        dateOfBirth: '',
        personnelGroups: [],
        careerPaths: []
      };
    });
    it('should open create user form when clicking create user button', () => {
      (component as any).createUserFormJSON =
        UserAccountMockData.mockCreateUserFormData;
      component.currentUser = mockSelectedUser;
      const ngbModal = fixture.debugElement.injector.get(NgbModal);
      spyOn(ngbModal, 'open').and.returnValue({
        result: new Promise(() => {
          return;
        }),
        componentInstance: {
          user: undefined,
          employeeAvatarFromEmailsMap: undefined,
          fullUserInfoJsonData: undefined,
          surveyjsOptions: undefined,
          basicInfoJsonTemplate: undefined,
          advanceInfoJsonTemplate: undefined,
          submit: new EventEmitter(),
          cancel: new EventEmitter()
        }
      });
      component.onCreateNewUser();
      expect(ngbModal.open).toHaveBeenCalled();
    });

    describe('create user form submitted successfully then', () => {
      let formModal: CxFormModal;
      const mockCreatedUser = {
        identity: { id: 'newCreatedUserId' },
        emailAddress: 'test@email.com'
      };
      beforeEach(() => {
        component.currentUser = mockSelectedUser;
        component.pending2ndLevelApprovalList = [];
        formModal = fixture.debugElement.injector.get(CxFormModal);
        spyOn(formModal, 'openSurveyJsForm').and.returnValue({
          result: new Promise(() => {
            return;
          }),
          close: () => {
            return;
          }
        });
        spyOn(userAccountsDataService, 'createUser').and.returnValue(
          of(mockCreatedUser)
        );
        spyOn<any>(component, 'buildNewUserDtoFromSubmittedForm');
      });
      it('should build user dto and request for creating', () => {
        (component as any).createUserFormJSON =
          UserAccountMockData.mockCreateUserFormData;
        component.breadCrumbNavigation = [
          {
            identity: {
              extId: 'HRMS001',
              ownerId: 3001,
              customerId: 2052,
              archetype: 'DataOwner',
              id: 14350
            },
            name: 'MOE'
          },
          {
            identity: {
              extId: 'HRMS002',
              ownerId: 3001,
              customerId: 2052,
              archetype: 'DataOwner',
              id: 14351
            },
            name: 'MOE'
          }
        ];
        component.onCreateNewUser();
        formModal.submit.next({
          expirationDate: '',
          personnelGroups: []
        });
        fixture.whenStable(() => {
          expect(
            (component as any).buildNewUserDtoFromSubmittedForm
          ).toHaveBeenCalled();
          expect(formModal.openSurveyJsForm).toHaveBeenCalled();
        });
      });

      it('should create append newly created user to the beginning of pending list when creator is Overall System Admin', () => {
        (component as any).superAdminRole = [
          UserRoleEnum.OverallSystemAdministrator
        ];
        component.onCreateNewUser();
        formModal.submit.next({
          expirationDate: '',
          personnelGroups: []
        });
        fixture.whenStable(() => {
          expect(
            (component as any).buildNewUserDtoFromSubmittedForm
          ).toHaveBeenCalled();
          expect(formModal.openSurveyJsForm).toHaveBeenCalled();
        });
      });

      it('should request for creating Approval group when new created user is Approving Officer', () => {
        const mockCreatedApprovingOfficer = {
          ...mockCreatedUser,
          systemRoles: [
            {
              identity: {
                extId: UserRoleEnum.ReportingOfficer
              }
            }
          ]
        };
        userAccountsDataService.createUser = jasmine
          .createSpy()
          .and.returnValue(of(mockCreatedApprovingOfficer));
        const createApprovalGroupSpy = spyOn(
          component as any,
          'createApprovalGroup'
        );
        const updateCachedMapsSpy = spyOn(component as any, 'updateCachedMaps');
        component.onCreateNewUser();
        formModal.submit.next({
          expirationDate: '',
          personnelGroups: []
        });
        fixture.whenStable(() => {
          expect(updateCachedMapsSpy).toHaveBeenCalled();
          expect(createApprovalGroupSpy).toHaveBeenCalled();
        });
      });
    });
  });

  describe('Approval new user workflow: ', () => {
    let userAccountFixture;
    let userAccountComponent;
    let userAccountComponentPopup;
    let userAccountFixturePopup;
    let mockSelectedUser;
    let eventAction;
    beforeEach(() => {
      userAccountFixture = TestBed.createComponent(UserAccountsComponent);
      userAccountComponent = userAccountFixture.componentInstance;
      userAccountFixturePopup = TestBed.createComponent(
        UserAccountConfirmationDialogComponent
      );
      userAccountComponentPopup = userAccountFixturePopup.componentInstance;
      mockSelectedUser = {
        departmentName: 'MOE',
        departmentId: 14350,
        firstName: 'nhan.conexus.02@gmail.com',
        mobileCountryCode: 65,
        emailAddress: 'nhan.conexus.02@gmail.com',
        gender: 0,
        tag: '',
        created: '2019-07-10T08:53:00',
        forceLoginAgain: false,
        identity: {
          extId: '1ecd80e4-8bdd-42a5-8a05-ccbec0ce15be',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'Employee',
          id: 330872
        },
        entityStatus: {
          externallyMastered: false,
          lastExternallySynchronized: '0001-01-01T00:00:00',
          entityVersion: 'AAAAAAALSws=',
          lastUpdated: '2019-07-11T01:47:38.2486004',
          lastUpdatedBy: 1,
          statusId: 'PendingApproval1st',
          statusReasonId: 'Unknown',
          deleted: false
        },
        systemRoles: [
          {
            identity: {
              extId: 'overallsystemadministrator',
              ownerId: 3001,
              customerId: 0,
              archetype: 'SystemRole',
              id: 112
            },
            entityStatus: {
              externallyMastered: false,
              lastUpdated: '0001-01-01T00:00:00Z',
              lastUpdatedBy: 0,
              statusId: 'Active',
              statusReasonId: 'Unknown',
              deleted: false
            },
            localizedData: [
              {
                id: 2,
                languageCode: 'en-US',
                fields: [
                  {
                    name: 'Name',
                    localizedText: 'System Administrator'
                  },
                  {
                    name: 'Description',
                    localizedText: 'Overall System Administrator'
                  }
                ]
              }
            ]
          }
        ]
      };

      eventAction = {
        action: {
          targetAction: 'Unlock',
          targetIcon: 'unlock-icon',
          isSimpleAction: false,
          allowActionSingle: true,
          message: 'User_Account_Page.User_List.Unlock_User_Warning',
          currentStatus: ['IdentityServerLocked']
        },
        item: {
          departmentName: 'Schools Division',
          departmentAddress: '1 North Buona Vista Drive  ',
          jsonDynamicAttributes: {
            idpLocked: true
          },
          departmentId: 15818,
          firstName: 'Division Learning Coordinator 001',
          mobileCountryCode: 65,
          emailAddress: 'dlc-001@mailinator.com',
          gender: 0,
          tag: '',
          created: '2019-08-06T09:34:00Z',
          forceLoginAgain: false,
          identity: {
            extId: 'deea1d8c-8c4e-40a0-b970-aea5c5302978',
            ownerId: 3001,
            customerId: 2052,
            archetype: 'Employee',
            id: 36396
          },
          entityStatus: {
            externallyMastered: false,
            lastExternallySynchronized: '0001-01-01T00:00:00Z',
            entityVersion: 'AAAAAAACT5I=',
            lastUpdated: '2019-10-08T08:06:46.2849989Z',
            lastUpdatedBy: 1,
            statusId: 'IdentityServerLocked',
            statusReasonId: 'Unknown',
            deleted: false
          },
          systemRoles: [
            {
              localizedData: [
                {
                  id: 2,
                  languageCode: 'en-US',
                  fields: [
                    {
                      name: 'Name',
                      localizedText: 'Divisional Learning Coordinator'
                    },
                    {
                      name: 'Description',
                      localizedText: 'Divisional Learning Coordinator (DLC)'
                    }
                  ]
                }
              ],
              identity: {
                extId: 'divisiontrainingcoordinator',
                ownerId: 3001,
                customerId: 0,
                archetype: 'SystemRole',
                id: 90
              },
              entityStatus: {
                externallyMastered: false,
                lastUpdated: '0001-01-01T00:00:00Z',
                lastUpdatedBy: 0,
                statusId: 'Active',
                statusReasonId: 'Unknown',
                deleted: false
              }
            }
          ]
        }
      };
    });

    it('Unlock user', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Unlock_User_Warning',
          StatusActionTypeEnum.Unlock,
          userAction.item
        );
      });
    });

    it('Reset password', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'RequestSpecialApproval',
        allowActionSingle: undefined,
        icon: 'request-icon',
        message: 'User_Account_Page.User_List.Request_Special_Approval_Warning',
        text: 'RequestSpecialApproval'
      };

      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Reset_Password_Warning',
          StatusActionTypeEnum.ResetPassword,
          userAction.item
        );
      });
    });

    it('Request special approval', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'RequestSpecialApproval',
        allowActionSingle: undefined,
        icon: 'request-icon',
        message: 'User_Account_Page.User_List.Request_Special_Approval_Warning',
        text: 'RequestSpecialApproval'
      };
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Request_Special_Approval_Warning',
          StatusActionTypeEnum.RequestSpecialApproval,
          userAction.item
        );
      });
    });

    it('Accept pending user', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'Accept',
        allowActionSingle: undefined,
        icon: 'accept',
        message: 'User_Account_Page.User_List.Accept_Warning',
        text: 'Accept'
      };
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Accept_Warning',
          StatusActionTypeEnum.Accept,
          userAction.item
        );
      });
    });

    it('Reject pending user', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'Reject',
        allowActionSingle: undefined,
        icon: 'reject',
        message: 'User_Account_Page.User_List.Reject_Warning',
        text: 'Reject'
      };
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Reject_Warning',
          StatusActionTypeEnum.Reject,
          userAction.item
        );
      });
    });

    it('Set expiration date for user', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'SetExpirationDate',
        allowActionSingle: undefined,
        icon: 'date-range',
        message: 'User_Account_Page.User_List.Set_Expiration_Date_Warning',
        text: 'SetExpirationDate'
      };
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Set_Expiration_Date_Warning',
          StatusActionTypeEnum.SetExpirationDate,
          userAction.item
        );
      });
    });

    it('Delete user', () => {
      spyOn(userAccountComponent, 'evaluateActionOnUser');

      const userAction = eventAction;
      userAction.action = {
        actionType: 'Remove',
        allowActionSingle: undefined,
        icon: 'remove-icon',
        message: 'User_Account_Page.User_List.Remove_User_Warning',
        text: 'Remove'
      };
      component.onSingleActionClicked(userAction);
      userAccountFixture.whenStable(() => {
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalled();
        expect(userAccountComponent.evaluateActionOnUser).toHaveBeenCalledWith(
          'User_Account_Page.User_List.Remove_User_Warning',
          StatusActionTypeEnum.Archive,
          userAction.item
        );
      });
    });

    describe('Process change status of the user', () => {
      let formModalService;
      let userAccountDataService;
      let modalRef;
      beforeEach(() => {
        formModalService = TestBed.get(NgbModal);
        modalRef = formModalService.open(
          UserAccountConfirmationDialogComponent
        );
        spyOn(formModalService, 'open').and.returnValue(modalRef);
        spyOn(modalRef, 'close');
        userAccountDataService = userAccountFixture.debugElement.injector.get(
          UserAccountsDataService
        );
        spyOn(userAccountDataService, 'changeEmployeeStatus').and.returnValue(
          of(mockSelectedUser)
        );
        spyOn(userAccountComponent, 'deleteUser').and.returnValue(
          of(mockSelectedUser)
        );
        spyOn(userAccountComponent, 'handleUserResponseData').and.returnValue(
          of(mockSelectedUser)
        );
        spyOn<any>((userAccountComponent as any).toastr, 'success');
      });

      it('Should show confirmation popup', () => {
        userAccountComponent.evaluateActionOnUser(
          'Test confirmation message',
          StatusTypeEnum.Active.code,
          mockSelectedUser
        );
        expect(formModalService.open).toHaveBeenCalled();
      });

      it('Should close popup when data is null', fakeAsync(() => {
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(
          of(null)
        );
        userAccountComponentPopup.done.next(of(null));
        userAccountFixture.whenStable(() => {
          expect(modalRef.close).toHaveBeenCalled();
        });
      }));

      it('Change status of user', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Suspended.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.Active.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Change_User_Status_Warning',
          StatusActionTypeEnum.Active,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Unlock user from locked user', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId =
          StatusTypeEnum.IdentityServerLocked.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            statusId: StatusTypeEnum.IdentityServerLocked.code
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Unlock_User_Warning',
          StatusActionTypeEnum.Unlock,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Reset password', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Active.code;

        const userSelectedChange = {
          ...userSelected,
          resetOtp: true
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Reset_Password_Warning',
          StatusActionTypeEnum.ResetPassword,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Accept pending user level 1st', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.PendingApproval1st.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.PendingApproval2nd.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Accept_Warning',
          StatusActionTypeEnum.RequestSpecialApproval,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Reject pending user level 1st', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.PendingApproval1st.code;

        const statusId = userSelected.entityStatus.statusId;
        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.Deactive.code.toString(),
            statusReasonId:
              statusId === StatusTypeEnum.PendingApproval1st.code
                ? StatusReasonTypeConstant.ManuallyRejectedPending1st.code.toString()
                : statusId === StatusTypeEnum.PendingApproval2nd.code
                ? StatusReasonTypeConstant.ManuallyRejectedPending2nd.code.toString()
                : StatusReasonTypeConstant.ManuallyRejectedPending3rd.code.toString()
          },
          jsonDynamicAttributes: {
            rejectReason: 'rejected'
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Reject_Warning',
          StatusActionTypeEnum.Reject,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Accept pending user level 2nd', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.PendingApproval2nd.code;

        const userSelectedChange = {
          ...userSelected,
          resetOtp: true,
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.New.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Accept_Warning',
          StatusActionTypeEnum.Accept,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Reject pending user level 2nd', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.PendingApproval2nd.code;
        const statusId = userSelected.entityStatus.statusId;
        const userSelectedChange = {
          ...userSelected,
          resetOtp: true,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.Deactive.code.toString(),
            statusReasonId:
              statusId === StatusTypeEnum.PendingApproval1st.code
                ? StatusReasonTypeConstant.ManuallyRejectedPending1st.code.toString()
                : statusId === StatusTypeEnum.PendingApproval2nd.code
                ? StatusReasonTypeConstant.ManuallyRejectedPending2nd.code.toString()
                : StatusReasonTypeConstant.ManuallyRejectedPending3rd.code.toString()
          },
          jsonDynamicAttributes: {
            rejectReason: 'rejected'
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Reject_Warning',
          StatusActionTypeEnum.Reject,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Request special approval', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.PendingApproval2nd.code;

        const userSelectedChange = {
          ...userSelected,
          resetOtp: true,
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusId: StatusTypeEnum.New.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Request_Special_Approval_Warning',
          StatusActionTypeEnum.RequestSpecialApproval,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Set expiration date for user', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Active.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            expirationDate: moment(new Date(), AppConstant.backendDateFormat)
              .toDate()
              .toDateString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Set_Expiration_Date_Warning',
          StatusActionTypeEnum.SetExpirationDate,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Delete user', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Active.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusReasonId: StatusReasonTypeConstant.ManuallyArchived.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Remove_User_Warning',
          StatusActionTypeEnum.Archive,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalled();
          expect(
            userAccountDataService.changeEmployeeStatus
          ).toHaveBeenCalledWith(userSelectedChange);
        });
      });

      it('Call Change status and handle response with delete user', fakeAsync(() => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Active.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            statusReasonId: StatusReasonTypeConstant.ManuallyArchived.code.toString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Remove_User_Warning',
          StatusActionTypeEnum.Archive,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(userAccountComponent.deleteUser).toHaveBeenCalled();
          expect(userAccountComponent.deleteUser).toHaveBeenCalledWith(
            userSelectedChange
          );
        });
      }));

      it('Call Change status and handle response ', () => {
        const userSelected = { ...mockSelectedUser };
        userSelected.identity.statusId = StatusTypeEnum.Active.code;

        const userSelectedChange = {
          ...userSelected,
          entityStatus: {
            ...userSelected.entityStatus,
            expirationDate: moment(new Date(), AppConstant.backendDateFormat)
              .toDate()
              .toDateString()
          }
        };
        spyOn(userAccountComponentPopup.done, 'emit').and.returnValues(of({}));
        userAccountComponent.evaluateActionOnUser(
          'User_Account_Page.User_List.Set_Expiration_Date_Warning',
          StatusActionTypeEnum.SetExpirationDate,
          userSelected
        );
        userAccountComponentPopup.done.next(of({}));
        userAccountFixture.whenStable(() => {
          expect(
            userAccountComponent.handleUserResponseData
          ).toHaveBeenCalled();
          expect(
            userAccountComponent.handleUserResponseData
          ).toHaveBeenCalledWith(
            userSelectedChange,
            StatusActionTypeEnum.SetExpirationDate
          );
          expect(
            (userAccountComponent as any).toastr.success
          ).toHaveBeenCalled();
        });
      });
    });
  });
});
