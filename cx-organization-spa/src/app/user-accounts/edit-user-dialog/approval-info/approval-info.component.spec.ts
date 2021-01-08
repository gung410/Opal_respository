import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA, Pipe, PipeTransform } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgSelectModule } from '@ng-select/ng-select';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { ApprovalGroupTypeEnum } from 'app/user-accounts/constants/approval-group.enum';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { ApprovalDataService } from 'app/user-accounts/services/approval-data.service';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';

import {
  MemberApprovalGroupModel,
  ApprovalInfoTabModel
} from '../edit-user-dialog.model';
import { ApprovalInfoComponent } from './approval-info.component';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';

@Pipe({ name: 'translate' })
class MockTranslatePipe implements PipeTransform {
  transform(value: number): number {
    return value;
  }
}
describe('ApprovalInfoComponent', () => {
  let fixture: ComponentFixture<ApprovalInfoComponent>;
  let component: ApprovalInfoComponent;
  const mockEditingUser = {
    identity: {
      id: 0
    },
    groups: [{}, {}]
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      providers: [
        ToastrAdapterService,
        ApprovalDataService,
        UserAccountsDataService,
        { provide: ToastrService, useValue: { error: () => {} } },
        HttpHelpers
      ],
      imports: [
        HttpClientTestingModule,
        CxCommonModule,
        CommonModule,
        FormsModule,
        NgSelectModule,
        NoopAnimationsModule
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      declarations: [ApprovalInfoComponent, MockTranslatePipe]
    }).compileComponents();
  }));

  beforeEach(() => {
    TestBed.overrideTemplate(ApprovalInfoComponent, '');
    fixture = TestBed.createComponent(ApprovalInfoComponent);
    component = fixture.componentInstance;
    component.user = {
      identity: { id: 0 },
      systemRoles: [{ identity: { extId: UserRoleEnum.ReportingOfficer } }]
    } as UserManagement;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('loadEmployeesForApprovalData', () => {
    let approvalDataService: ApprovalDataService;
    beforeEach(() => {
      approvalDataService = fixture.debugElement.injector.get(
        ApprovalDataService
      );
      component.user = mockEditingUser as UserManagement;
      component.approvalData = new ApprovalInfoTabModel();
    });

    it('Should get approval groups members user approve for', async(() => {
      component.approvalData.memberOfPrimaryApprovalGroup = new MemberApprovalGroupModel();
      component.approvalData.memberOfAlternateApprovalGroup = new MemberApprovalGroupModel();
      spyOn(
        approvalDataService,
        'getApprovalGroupsUserApprovesFor'
      ).and.returnValue(
        of([
          {
            identity: { id: 0 },
            type: ApprovalGroupTypeEnum.PrimaryApprovalGroup
          },
          {
            identity: { id: 0 },
            type: ApprovalGroupTypeEnum.AlternativeApprovalGroup
          }
        ])
      );
      spyOn(approvalDataService, 'getApprovalGroupMembers').and.returnValue(
        of([{ identity: { id: 0 } }])
      );
      (component as any).loadEmployeesForApprovalData();
      expect(approvalDataService.getApprovalGroupMembers).toHaveBeenCalledTimes(
        2
      );
    }));

    it('Should show toastr error when loading approval groups failed', async(() => {
      const toastrService = fixture.debugElement.injector.get(ToastrService);
      spyOn(toastrService, 'error');
      spyOn(
        approvalDataService,
        'getApprovalGroupsUserApprovesFor'
      ).and.callFake(() => {
        return throwError({ error: '' });
      });
      (component as any).loadEmployeesForApprovalData();
      fixture.whenStable().then(() => {
        expect(toastrService.error).toHaveBeenCalled();
      });
    }));

    it('Should show toastr error when loading approval groups members failed', async(() => {
      const toastrService = fixture.debugElement.injector.get(ToastrService);
      component.approvalData.memberOfPrimaryApprovalGroup = new MemberApprovalGroupModel();
      component.approvalData.memberOfAlternateApprovalGroup = new MemberApprovalGroupModel();
      spyOn(toastrService, 'error');
      spyOn(
        approvalDataService,
        'getApprovalGroupsUserApprovesFor'
      ).and.returnValue(
        of([
          {
            identity: { id: 0 },
            type: ApprovalGroupTypeEnum.PrimaryApprovalGroup
          },
          {
            identity: { id: 0 },
            type: ApprovalGroupTypeEnum.AlternativeApprovalGroup
          }
        ])
      );
      spyOn(approvalDataService, 'getApprovalGroupMembers').and.callFake(() => {
        return throwError({ error: '' });
      });
      (component as any).loadEmployeesForApprovalData();
      fixture.whenStable().then(() => {
        expect(toastrService.error).toHaveBeenCalled();
      });
    }));

    it('Should not get approval group member when Editing user has no Approval group', () => {
      spyOn(
        approvalDataService,
        'getApprovalGroupsUserApprovesFor'
      ).and.returnValue(of([]));
      spyOn(approvalDataService, 'getApprovalGroupMembers');
      (component as any).loadEmployeesForApprovalData();
      expect(approvalDataService.getApprovalGroupMembers).toHaveBeenCalledTimes(
        0
      );
    });
  });
});
