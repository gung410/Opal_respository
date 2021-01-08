import { AgGridModule } from '@ag-grid-community/angular';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { IdpStatusBlockModule } from 'app/individual-development/shared/idp-status-block/idp-status-block.module';
import { NominationMemberList } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/sub-components/nomination-member-list/nomination-member-list.component';
import { SubComponentModule } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/sub-components/sub-components.module';
import { CxSelectModule } from 'app/shared/components/cx-select/cx-select.module';
import { SharedModule } from 'app/shared/shared.module';
import { ApplicationDateCellRendererComponent } from './ag-grid-renderer/application-date-cell-renderer.component';
import { AssessmentStatusCellRendererComponent } from './ag-grid-renderer/assessment-status-cell-renderer.component';
import { ClassRunCellRendererComponent } from './ag-grid-renderer/class-run-cell-renderer.component';
import { CourseCellRendererComponent } from './ag-grid-renderer/course-cell-renderer.component';
import { DepartmentCellRendererComponent } from './ag-grid-renderer/department-cell-renderer.component';
import { GroupActionRendererComponent } from './ag-grid-renderer/group-action-renderer.component';
import { GroupCellRendererComponent } from './ag-grid-renderer/group-cell-renderer.component';
import { LearnerCellRendererComponent } from './ag-grid-renderer/learner-cell-renderer.component';
import { MassNominationCellRendererComponent } from './ag-grid-renderer/mass-nomination-cell-renderer.component';
import { ReasonCellRendererComponent } from './ag-grid-renderer/reason-cell-renderer.component';
import { StatusCellRendererComponent } from './ag-grid-renderer/status-cell-renderer.component';
import { ApprovalBarComponent } from './approval-bar/approval-bar.component';
import { ApprovalPageComponent } from './approval-page.component';
import { FilterFormPlaceholderDirective } from './directives/filter-form-placeholder.derective';
import { ClassRegistrationFilterFormComponent } from './filter-form/class-registration/class-registration-filter-form.component';
import { ClassrunChangeRequestFilterFormComponent } from './filter-form/classrun-change/classrun-change-filter-form.component';
import { WithrawalRequestFilterFormComponent } from './filter-form/withrawal-request/withrawal-request-filter-form.component';
import { FilterSlidebarContainerComponent } from './filter-slidebar/filter-slidebar-container.component';
import { CourseDetailModalComponent } from './modals/course-detail-modal/course-detail-modal.component';
import { PendingLearnerListingModalComponent } from './modals/pending-learner-listing-modal.component';
import { ApprovalPageService } from './services/approval-page.service';
import { CourseFilterService } from './services/course-filter.service';
import { FilterSlidebarService } from './services/filter-slidebar.service';
import { IdpApprovalPageService } from './services/idp-approval-page.service';
import { OdpApprovalPageService } from './services/odp-approval-page.services';
import { StatusMapperService } from './services/status-mapper.service';
import { UserFilterService } from './services/user-filter.services';

@NgModule({
  declarations: [
    ApprovalPageComponent,
    ApprovalBarComponent,
    LearnerCellRendererComponent,
    GroupCellRendererComponent,
    GroupActionRendererComponent,
    DepartmentCellRendererComponent,
    CourseCellRendererComponent,
    ClassRunCellRendererComponent,
    ApplicationDateCellRendererComponent,
    ReasonCellRendererComponent,
    MassNominationCellRendererComponent,
    StatusCellRendererComponent,
    AssessmentStatusCellRendererComponent,
    PendingLearnerListingModalComponent,
    FilterSlidebarContainerComponent,
    FilterFormPlaceholderDirective,
    ClassRegistrationFilterFormComponent,
    WithrawalRequestFilterFormComponent,
    ClassrunChangeRequestFilterFormComponent,
    CourseDetailModalComponent,
  ],
  imports: [
    CommonModule,
    CxCommonModule,
    SharedModule,
    NgbModule,
    SubComponentModule,
    CxSelectModule,
    IdpStatusBlockModule,
    RouterModule.forChild([{ path: '**', component: ApprovalPageComponent }]),
    AgGridModule.withComponents([
      LearnerCellRendererComponent,
      GroupCellRendererComponent,
      GroupActionRendererComponent,
      DepartmentCellRendererComponent,
      CourseCellRendererComponent,
      ClassRunCellRendererComponent,
      ApplicationDateCellRendererComponent,
      ReasonCellRendererComponent,
      MassNominationCellRendererComponent,
      StatusCellRendererComponent,
      AssessmentStatusCellRendererComponent,
    ]),
  ],
  providers: [
    ApprovalPageService,
    IdpApprovalPageService,
    OdpApprovalPageService,
    AssignPDOService,
    UserFilterService,
    CourseFilterService,
    FilterSlidebarService,
    StatusMapperService,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [
    NominationMemberList,
    PendingLearnerListingModalComponent,
    ClassRegistrationFilterFormComponent,
    WithrawalRequestFilterFormComponent,
    ClassrunChangeRequestFilterFormComponent,
    CourseDetailModalComponent,
  ],
})
export class ApprovalPageModule {}
