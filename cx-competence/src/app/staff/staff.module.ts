import { AgGridModule } from '@ag-grid-community/angular';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MomentModule } from 'angular2-moment';
import { IdpService } from 'app-services/idp.service';
import { IndividualDevelopmentModule } from 'app/individual-development/individual-development.module';
import { IdpStatusBlockModule } from 'app/individual-development/shared/idp-status-block/idp-status-block.module';
import { ExportLearningNeedsAnalysisDialogComponent } from 'app/learning-needs-analysis/export-learning-needs-analysis-dialog/export-learning-needs-analysis-dialog.component';
import { DownloadFileComponent } from 'app/shared/components/download-file/download-file.component';
import { SharedModule } from 'app/shared/shared.module';
import { ProfessionalDevelopmentPlanComponent } from './professional-development-plan/professional-development-plan.component';
import { StaffDetailComponent } from './staff-detail/staff-detail.component';
import { StaffContainerComponent } from './staff.container/staff-container.component';
import { SLApprovingOfficerRendererComponent } from './staff.container/staff-list/ag-grid-renderer/approving-officer-renderer.component';
import { SLDueDateRendererComponent } from './staff.container/staff-list/ag-grid-renderer/due-date-renderer.component';
import { LNAHeaderComponent } from './staff.container/staff-list/ag-grid-renderer/LNA-header.component';
import { SLNameRendererComponent } from './staff.container/staff-list/ag-grid-renderer/name-renderer.component';
import { SLServiceSchemeRendererComponent } from './staff.container/staff-list/ag-grid-renderer/service-scheme-renderer.component';
import { SLUserGroupsRendererComponent } from './staff.container/staff-list/ag-grid-renderer/user-groups-renderer.component';
import { AssignLnAssessmentsDialogComponent } from './staff.container/staff-list/assign-ln-assessments-dialog/assign-ln-assessments-dialog.component';
import { ReminderDialogComponent } from './staff.container/staff-list/reminder-dialog/reminder-dialog.component';
import { StaffExportComponent } from './staff.container/staff-list/staff-export/staff-export.component';
import { StaffFilterTagGroupComponent } from './staff.container/staff-list/staff-filter-tag-group/staff-filter-tag-group.component';
import { StaffFilterComponent } from './staff.container/staff-list/staff-filter/staff-filter.component';
import { StaffListComponent } from './staff.container/staff-list/staff-list.component';
import { StaffShowHideComponent } from './staff.container/staff-list/staff-show-hide-column/staff-show-hide-column.component';

@NgModule({
  declarations: [
    StaffListComponent,
    StaffDetailComponent,
    StaffContainerComponent,
    ProfessionalDevelopmentPlanComponent,
    AssignLnAssessmentsDialogComponent,
    SLNameRendererComponent,
    SLApprovingOfficerRendererComponent,
    SLServiceSchemeRendererComponent,
    LNAHeaderComponent,
    SLDueDateRendererComponent,
    StaffFilterComponent,
    StaffFilterTagGroupComponent,
    SLUserGroupsRendererComponent,
    StaffExportComponent,
    ReminderDialogComponent,
    StaffShowHideComponent,
    ExportLearningNeedsAnalysisDialogComponent,
  ],
  imports: [
    CommonModule,
    CxCommonModule,
    MomentModule,
    NgbModule,
    NgbDropdownModule,
    SharedModule,
    IndividualDevelopmentModule,
    RouterModule.forChild([
      { path: '', component: StaffContainerComponent },
      { path: 'detail/:id', component: StaffDetailComponent },
      { path: 'detail/:id/:target', component: StaffDetailComponent },
      { path: 'lna/export', component: DownloadFileComponent },
    ]),
    AgGridModule.withComponents([
      SLNameRendererComponent,
      SLApprovingOfficerRendererComponent,
      SLServiceSchemeRendererComponent,
      LNAHeaderComponent,
      SLDueDateRendererComponent,
      SLUserGroupsRendererComponent,
    ]),
    IdpStatusBlockModule,
  ],
  providers: [IdpService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [
    AssignLnAssessmentsDialogComponent,
    StaffExportComponent,
    StaffFilterComponent,
    ReminderDialogComponent,
    StaffShowHideComponent,
    ExportLearningNeedsAnalysisDialogComponent,
  ],
})
export class StaffModule {}
