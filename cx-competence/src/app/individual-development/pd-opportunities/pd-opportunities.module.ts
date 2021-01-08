import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { IsoToSurveyDatePipe } from 'app-utilities/iso-to-date.pipe';
import { SharedModule } from 'app/shared/shared.module';
import { CxCommentModule } from '../cx-comment/cx-comment.module';
import { IdpStatusBlockModule } from '../shared/idp-status-block/idp-status-block.module';
import { IdpToolbarModule } from '../shared/idp-toolbar/idp-toolbar.module';
import { OpportunityTagComponent } from '../shared/opportunity-tag/opportunity-tag.component';
import { PdEvaluationDialogComponent } from '../shared/pd-evalution-dialog/pd-evaluation-dialog.component';
import { PdEvaluationDialogModule } from '../shared/pd-evalution-dialog/pd-evaluation-dialog.module';
import { ActionItemComponent } from './action-item/action-item.component';
import { PdOpportunitiesComponent } from './pd-opportunities.component';

@NgModule({
  declarations: [
    PdOpportunitiesComponent,
    OpportunityTagComponent,
    IsoToSurveyDatePipe,
    ActionItemComponent,
  ],
  exports: [PdOpportunitiesComponent],
  imports: [
    SharedModule,
    CxCommonModule,
    CxCommentModule,
    IdpToolbarModule,
    MatTabsModule,
    MatSelectModule,
    MatRadioModule,
    MatTabsModule,
    PdEvaluationDialogModule,
    IdpStatusBlockModule,
  ],
  entryComponents: [PdEvaluationDialogComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class PdOpportunitiesModule {}
