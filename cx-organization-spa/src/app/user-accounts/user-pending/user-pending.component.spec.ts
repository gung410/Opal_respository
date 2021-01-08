import 'rxjs/add/observable/of';

import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import {
  createDefaultStorage,
  CxTableComponent,
  OAuthStorage,
  UrlHelperService
} from '@conexus/cx-angular-common';
import { NgbDropdownModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateModule,
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
import * as $ from 'jquery';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

import { UserAccountMockData } from '../mock-data/user-account-mock-data';
import { UserManagement } from '../models/user-management.model';
import { StatusHistoricalDataPanelComponent } from '../status-historical-data-panel/status-historical-data-panel.component';
import { StatusHistoricalRowComponent } from '../status-historical-row/status-historical-row.component';
import { UserAccountsDataService } from '../user-accounts-data.service';
import { UserPendingActionDialogComponent } from './user-pending-action-dialog/user-pending-action-dialog.component';
import { UserPendingComponent } from './user-pending.component';

(window as any).$ = $;

describe('UserPendingComponent', () => {
  const initItems: UserManagement[] =
    UserAccountMockData.mockEmployeesData.items;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        NgbDropdownModule,
        FormsModule,
        TranslateModule,
        HttpClientModule,
        RouterTestingModule,
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        ToastrModule.forRoot()
      ],
      declarations: [
        UserPendingComponent,
        CxTableComponent,
        UserPendingActionDialogComponent,
        StatusHistoricalRowComponent,
        StatusHistoricalDataPanelComponent
      ],
      providers: [
        UserAccountsDataService,
        NotificationDataService,
        NotificationService,
        AuthService,
        NgbModal,
        TranslateService,
        ToastrService,
        MissingTranslationHandler,
        [{ provide: USE_STORE, useValue: {} }],
        [{ provide: USE_DEFAULT_LANG, useValue: {} }],
        { provide: OAuthStorage, useFactory: createDefaultStorage },
        TranslateParser,
        TranslateCompiler,
        TranslateLoader,
        TranslateStore,
        TranslateAdapterService,
        UrlHelperService,
        HttpHelpers
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    TestBed.overrideTemplate(UserPendingComponent, '<div></div>');
    TestBed.overrideTemplate(CxTableComponent, '<div></div>');
  }));

  describe(':', () => {
    function setup(): any {
      const fixture = TestBed.createComponent(UserPendingComponent);
      const component = fixture.componentInstance;
      const userAccountsDataService = TestBed.get(UserAccountsDataService);
      component.items = initItems;

      const fixturePopup = TestBed.createComponent(
        UserPendingActionDialogComponent
      );
      const componentPopup = fixturePopup.componentInstance;

      return {
        fixture,
        component,
        UserAccountsDataService: userAccountsDataService,
        fixturePopup,
        componentPopup
      };
    }

    it('should create an user list component', () => {
      const { component } = setup();
      expect(component).toBeTruthy();
    });

    it('should init component with item equal object', () => {
      const { component, fixture } = setup();
      fixture.whenStable().then(() => {
        expect(component.items).toEqual(initItems);
      });
    });

    it('should call onAcceptClicked with single user', () => {
      const { component, componentPopup } = setup();
      const returnValue = initItems[0];
      componentPopup.items = [returnValue];
      spyOn(component, 'onAcceptClicked');
      spyOn(componentPopup.doneAction, 'emit').and.returnValue(
        Observable.of(returnValue)
      );
      component.onAcceptClicked(initItems[0]);
      componentPopup.done();

      expect(componentPopup.doneAction.emit).toHaveBeenCalled();
    });

    it('should call onRejectClicked with single user', () => {
      const { component, componentPopup } = setup();
      const returnValue = initItems[0];
      componentPopup.items = [returnValue];
      spyOn(component, 'onRejectClicked');
      spyOn(componentPopup.doneAction, 'emit').and.returnValue(
        Observable.of(returnValue)
      );
      component.onRejectClicked(initItems[0]);
      componentPopup.done();

      expect(componentPopup.doneAction.emit).toHaveBeenCalled();
    });

    it('should call onRequestSpecialApprovalClicked with single user', () => {
      const { component, componentPopup } = setup();
      const returnValue = initItems[0];
      componentPopup.items = [returnValue];
      spyOn(component, 'onRequestSpecialApprovalClicked');
      spyOn(componentPopup.doneAction, 'emit').and.returnValue(
        Observable.of(returnValue)
      );
      component.onRequestSpecialApprovalClicked(initItems[0]);
      componentPopup.done();

      expect(componentPopup.doneAction.emit).toHaveBeenCalled();
    });
  });
});
