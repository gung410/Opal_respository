import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { LearningNeedsAnalysisComponent } from './learning-needs-analysis.component';
import { LearningNeedsAnalysisContentModule } from './learning-needs-analysis-content/learning-needs-analysis-content.module';
import { CxCommentModule } from '../cx-comment/cx-comment.module';
import { LearningNeedsAnalysisReviewDialogComponent } from './learning-needs-analysis-review-dialog/learning-needs-analysis-review-dialog.component';

@NgModule({
  declarations: [
    LearningNeedsAnalysisComponent,
    LearningNeedsAnalysisReviewDialogComponent,
  ],
  exports: [LearningNeedsAnalysisComponent],
  imports: [
    SharedModule,
    CxCommonModule,
    CxCommentModule,
    LearningNeedsAnalysisContentModule,
  ],
  entryComponents: [LearningNeedsAnalysisReviewDialogComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LearningNeedsAnalysisModule {}
