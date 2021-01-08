import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { AgGridModule } from 'ag-grid-angular';
import { MomentModule } from 'angular2-moment';
import { CellDropdownActionComponent } from 'app/shared/components/cell-dropdown-action/cell-dropdown-action.component';
import { SharedModule } from 'app/shared/shared.module';
import {
  PERFECT_SCROLLBAR_CONFIG,
  PerfectScrollbarConfigInterface,
  PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';
import { MassCreateUserImportPanelComponent } from './mass-create-user-import-panel/mass-create-user-import-panel.component';

import { AssignAODialogComponent } from './assign-ao-dialog/assign-ao-dialog.component';
import { AuditHistoryComponent } from './audit-history/audit-history.component';
import { ApprovalInfoComponent } from './edit-user-dialog/approval-info/approval-info.component';
import { EditUserDialogComponent } from './edit-user-dialog/edit-user-dialog.component';
import { InvalidUsersCreationRecordDialogComponent } from './invalid-users-ceation-record-dialog/invalid-users-creation-record-dialog.component';
import { MassUserCreationUploadedFilesResultComponent } from './mass-create-user-import-panel/mass-user-creation-uploaded-files-result/mass-user-creation-uploaded-files-result.component';
import { MassUserCreationFileNameRendererComponent } from './mass-create-user-import-panel/mass-user-creation-uploaded-files-result/renderer-components/mass-user-creation-file-name-renderer.component';
import { UploadedFilesListComponent } from './mass-create-user-import-panel/mass-user-creation-uploaded-files-result/uploaded-files-list/uploaded-files-list.component';
import { ApprovalDataService } from './services/approval-data.service';
import { FileInfoApiService } from './services/file-info-api.service';
import { FileInfoListService } from './services/file-info-list.service';
import { StatusHistoricalDataPanelComponent } from './status-historical-data-panel/status-historical-data-panel.component';
import { StatusHistoricalRowComponent } from './status-historical-row/status-historical-row.component';
import { UserAccountConfirmationDialogComponent } from './user-account-confirmation-dialog/user-account-confirmation-dialog.component';
import { UserAccountsDataService } from './user-accounts-data.service';
import { UserAccountsComponent } from './user-accounts.component';
import { UserAuditHistoryComponent } from './user-audit-history/user-audit-history.component';
import { UserExportComponent } from './user-export/user-export.component';
import { UserFilterTagGroupComponent } from './user-filter-tag-group/user-filter-tag-group.component';
import { UserFilterComponent } from './user-filter/user-filter.component';
import { CellApprovingOfficerComponent } from './user-list/cell-components/cell-approving-officer/cell-approving-officer.component';
import { CellExpandableListComponent } from './user-list/cell-components/cell-expandable-list/cell-expandable-list.component';
import { CellUserInfoComponent } from './user-list/cell-components/cell-user-info/cell-user-info.component';
import { CellUserStatusComponent } from './user-list/cell-components/cell-user-status/cell-user-status.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserOtherPlaceListComponent } from './user-other-place-list/user-other-place-list.component';
import { CellUserPendingInfoComponent } from './user-pending-list/cell-components/cell-user-pending-info/cell-user-pending-info.component';
import { UserPendingListComponent } from './user-pending-list/user-pending-list.component';
import { UserPendingActionDialogComponent } from './user-pending/user-pending-action-dialog/user-pending-action-dialog.component';
import { UserPendingComponent } from './user-pending/user-pending.component';
import { UserReportingTemplateComponent } from './user-reporting-template/user-reporting-template.component';
import { UserShowHideComponent } from './user-show-hide-column/user-show-hide-column.component';
import { UserStatusComponent } from './user-status/user-status.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};
@NgModule({
  declarations: [
    StatusHistoricalRowComponent,
    UserAccountsComponent,
    UserAccountConfirmationDialogComponent,
    StatusHistoricalDataPanelComponent,
    UserPendingActionDialogComponent,
    UserStatusComponent,
    UserPendingComponent,
    AssignAODialogComponent,
    UserExportComponent,
    EditUserDialogComponent,
    AuditHistoryComponent,
    UserAuditHistoryComponent,
    UserFilterComponent,
    UserFilterTagGroupComponent,
    ApprovalInfoComponent,
    UserListComponent,
    CellUserInfoComponent,
    CellExpandableListComponent,
    CellUserInfoComponent,
    CellApprovingOfficerComponent,
    CellUserStatusComponent,
    UserPendingListComponent,
    CellUserPendingInfoComponent,
    UserShowHideComponent,
    UserReportingTemplateComponent,
    MassCreateUserImportPanelComponent,
    UserOtherPlaceListComponent,
    InvalidUsersCreationRecordDialogComponent,
    MassUserCreationUploadedFilesResultComponent,
    MassUserCreationFileNameRendererComponent,
    UploadedFilesListComponent
  ],
  imports: [
    CommonModule,
    NgbDropdownModule,
    CxCommonModule,
    NgSelectModule,
    NgbModule,
    SharedModule,
    MomentModule,
    AgGridModule.withComponents([
      CellUserInfoComponent,
      CellDropdownActionComponent,
      CellUserPendingInfoComponent,
      CellExpandableListComponent,
      CellApprovingOfficerComponent,
      CellUserStatusComponent,
      UserStatusComponent,
      MassUserCreationFileNameRendererComponent
    ]),
    RouterModule.forChild([
      { path: '', component: UserAccountsComponent },
      { path: 'report', component: UserReportingTemplateComponent },
      {
        path: 'mass-users-creation',
        component: MassCreateUserImportPanelComponent
      }
    ]),
    MatTabsModule,
    PerfectScrollbarModule
  ],
  exports: [UserAccountsComponent],
  entryComponents: [
    UserAccountConfirmationDialogComponent,
    UserPendingActionDialogComponent,
    AssignAODialogComponent,
    EditUserDialogComponent,
    MassCreateUserImportPanelComponent,
    InvalidUsersCreationRecordDialogComponent,
    UserExportComponent,
    UserFilterComponent,
    UserShowHideComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    UserAccountsDataService,
    FileInfoApiService,
    FileInfoListService,
    ApprovalDataService,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class UserAccountsModule {}
