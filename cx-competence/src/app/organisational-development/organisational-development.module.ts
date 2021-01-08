import { AgGridModule } from '@ag-grid-community/angular';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { KeyLearningProgramHelper } from 'app-services/odp/learning-plan-services/key-learning-program.helper';
import { KeyLearningProgramService } from 'app-services/odp/learning-plan-services/key-learning-program.service';
import { CxPeoplePickerModule } from 'app/cx-people-picker/cx-people-picker.module';
import { CxCommentModule } from 'app/individual-development/cx-comment/cx-comment.module';
import { AvatarComponent } from 'app/shared/components/avatar/avatar.component';
import { DownloadFileComponent } from 'app/shared/components/download-file/download-file.component';
import { OpportunitiesCatalogDialogModule } from 'app/shared/components/opportunities-catalog-dialog/opportunities-catalog-dialog.module';
import { SharedModule } from 'app/shared/shared.module';
import { DuplicateLearningDirectionDialogComponent } from './learning-plan-detail/duplicate-learning-direction-dialog/duplicate-learning-direction-dialog.component';
import { PlannedAreaComponent } from './learning-plan-detail/learning-plan-content/key-learning-program/planned-area/planned-area.component';
import { PlannedListAreaComponent } from './learning-plan-detail/learning-plan-content/key-learning-program/planned-list-area/planned-list-area.component';
import { PlannedListPDOComponent } from './learning-plan-detail/learning-plan-content/key-learning-program/planned-list-pdo/planned-list-pdo.component';
import { PlannedPDODetailModule } from './learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/planned-pdo-detail.module';
import { SubComponentModule } from './learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/sub-components/sub-components.module';
import { LearningPlanContentComponent } from './learning-plan-detail/learning-plan-content/learning-plan-content.component';
import { LearningPlanDetailComponent } from './learning-plan-detail/learning-plan-detail.component';
import { LearningPlanListComponent } from './learning-plan-list/learning-plan-list.component';
import { LPLNameRendererComponent } from './learning-plan-list/name-renderer.component';
import { MetadataModificationInfoComponent } from './metadata-modification-info/metadata-modification-info.component';
import { OdpDepartmentBrowserComponent } from './odp-department-browser/odp-department-browser.component';
import { OdpService } from './odp.service';
import { OrganisationalDevelopmentRoutingModule } from './organisational-development-routing.module';
import { OverallClassrunInfoComponent } from './overall-organizational-development/overall-classrun-info/overall-classrun-info.component';
import { OverallOrganizationalDevelopmentContentComponent } from './overall-organizational-development/overall-organizational-development-content/overall-organizational-development-content.component';
import { OverallOrganizationalDevelopmentComponent } from './overall-organizational-development/overall-organizational-development.component';
import { OverallPlannedListPDOComponent } from './overall-organizational-development/overall-planned-list-pdo/overall-planned-list-pdo.component';
import { OverallPlannedPDONominationsComponent } from './overall-organizational-development/overall-planned-pdo-nominations/overall-planned-pdo-nominations.component';
import { CodeRendererComponent } from './strategic-thrusts/code-render.component';
import { CreateNewStrategicThrustComponent } from './strategic-thrusts/create-new-strategic-thrust/create-new-strategic-thrust.component';
import { StrategicThrustsComponent } from './strategic-thrusts/strategic-thrusts.component';

@NgModule({
  declarations: [
    StrategicThrustsComponent,
    LearningPlanDetailComponent,
    LearningPlanListComponent,
    LearningPlanContentComponent,
    OverallOrganizationalDevelopmentComponent,
    OverallOrganizationalDevelopmentContentComponent,
    OverallPlannedPDONominationsComponent,
    OverallPlannedListPDOComponent,
    OverallClassrunInfoComponent,
    LPLNameRendererComponent,
    CodeRendererComponent,
    AvatarComponent,
    MetadataModificationInfoComponent,
    CreateNewStrategicThrustComponent,
    PlannedAreaComponent,
    DuplicateLearningDirectionDialogComponent,
    OdpDepartmentBrowserComponent,
    PlannedListAreaComponent,
    PlannedListPDOComponent,
  ],
  imports: [
    SharedModule,
    OrganisationalDevelopmentRoutingModule,
    CxCommonModule,
    RouterModule.forChild([
      {
        path: 'massnominationresult/download',
        component: DownloadFileComponent,
      },
    ]),
    AgGridModule.withComponents([
      LPLNameRendererComponent,
      StrategicThrustsComponent,
      CodeRendererComponent,
    ]),
    NgbModule,
    CxCommentModule,
    PlannedPDODetailModule,
    CxPeoplePickerModule,
    SubComponentModule,
    OpportunitiesCatalogDialogModule,
  ],
  entryComponents: [
    CreateNewStrategicThrustComponent,
    DuplicateLearningDirectionDialogComponent,
  ],
  providers: [OdpService, KeyLearningProgramHelper, KeyLearningProgramService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class OrganisationalDevelopmentModule {}
