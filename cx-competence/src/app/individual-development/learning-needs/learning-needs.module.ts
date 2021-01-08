import {
  NgModule,
  CUSTOM_ELEMENTS_SCHEMA,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { CxCommentModule } from '../cx-comment/cx-comment.module';
import { LearningAreaChartModule } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.module';
import { LearningNeedsComponent } from './learning-needs.component';
import { IdpToolbarModule } from '../shared/idp-toolbar/idp-toolbar.module';
import { MatSelectModule } from '@angular/material/select';
import { LearningNeedsHeaderComponent } from './learning-needs-header/learning-needs-header.component';
import { PdEvaluationDialogModule } from '../shared/pd-evalution-dialog/pd-evaluation-dialog.module';
import { IdpStatusBlockModule } from '../shared/idp-status-block/idp-status-block.module';

@NgModule({
  declarations: [LearningNeedsComponent, LearningNeedsHeaderComponent],
  exports: [LearningNeedsComponent],
  imports: [
    SharedModule,
    CxCommonModule,
    CxCommentModule,
    LearningAreaChartModule,
    IdpToolbarModule,
    MatSelectModule,
    PdEvaluationDialogModule,
    IdpStatusBlockModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class LearningNeedsModule {}
