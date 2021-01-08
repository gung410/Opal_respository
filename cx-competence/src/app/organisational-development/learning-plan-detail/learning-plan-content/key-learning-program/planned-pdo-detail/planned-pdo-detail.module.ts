import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA
} from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { KlpNominationService } from 'app-services/idp/assign-pdo/klp-nomination.service';
import { RecommendationService } from 'app-services/idp/assign-pdo/recommendation.service';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { CxSelectModule } from 'app/shared/components/cx-select/cx-select.module';
import { SharedModule } from 'app/shared/shared.module';
import { AdhocNominationsModule } from './adhoc-nominations/adhoc-nominattions.module';
import { AssignPDOResultDialogComponent } from './assign-pdo-result-dialog/assign-pdo-result-dialog.component';
import { InvalidNominationRecordDialogComponent } from './invalid-nomination-record-dialog/invalid-nomination-record-dialog.component';
import { PlannedPDOContentComponent } from './planned-pdo-content/planned-pdo-content.component';
import { PlannedPDOCostComponent } from './planned-pdo-cost/planned-pdo-cost.component';
import { PlannedPDODetailComponent } from './planned-pdo-detail.component';
import { PlannedPdoMassNominationComponent } from './planned-pdo-mass-nomination/planned-pdo-mass-nomination.component';
import { PlannedPDONominationsComponent } from './planned-pdo-nominations/planned-pdo-nominations.component';
import { PlannedPDORecommendationsComponent } from './recommendations/planned-pdo-recommendations.component';
import { SubComponentModule } from './sub-components/sub-components.module';

@NgModule({
  declarations: [
    PlannedPDODetailComponent,
    PlannedPDOContentComponent,
    PlannedPDONominationsComponent,
    PlannedPDORecommendationsComponent,
    PlannedPDOCostComponent,
    AssignPDOResultDialogComponent,
    PlannedPdoMassNominationComponent,
    InvalidNominationRecordDialogComponent
  ],
  imports: [
    SharedModule,
    CxCommonModule,
    CxSelectModule,
    MatRadioModule,
    SubComponentModule,
    AdhocNominationsModule
  ],
  providers: [
    AssignPDOService,
    PDOpportunityService,
    KlpNominationService,
    RecommendationService,
  ],
  exports: [
    PlannedPDODetailComponent,
    PlannedPdoMassNominationComponent
  ],
  entryComponents: [
    AssignPDOResultDialogComponent,
    InvalidNominationRecordDialogComponent,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class PlannedPDODetailModule { }
