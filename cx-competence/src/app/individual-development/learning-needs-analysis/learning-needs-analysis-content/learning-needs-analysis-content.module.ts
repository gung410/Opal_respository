import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { LearningNeedsAnalysisContentComponent } from './learning-needs-analysis-content.component';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { LearningNeedsPreviewModule } from '../learning-needs-preview/learning-needs-preview.module';
import { LearningNeedsReviewModule } from '../learning-needs-review/learning-needs-review.module';
import { LearningNeedsPreviewAnswerModule } from '../learning-needs-preview-answer/learning-needs-preview-answer-module';

@NgModule({
  declarations: [LearningNeedsAnalysisContentComponent],
  exports: [LearningNeedsAnalysisContentComponent],
  imports: [
    SharedModule,
    CxCommonModule,
    LearningNeedsReviewModule,
    LearningNeedsPreviewModule,
    LearningNeedsPreviewAnswerModule,
  ],
})
export class LearningNeedsAnalysisContentModule {}
