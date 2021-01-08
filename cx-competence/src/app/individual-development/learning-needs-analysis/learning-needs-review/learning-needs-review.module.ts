import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { LearningNeedsReviewComponent } from './learning-needs-review.component';
import { LearningAreaChartModule } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.module';

@NgModule({
  declarations: [LearningNeedsReviewComponent],
  exports: [LearningNeedsReviewComponent],
  imports: [SharedModule, LearningAreaChartModule],
})
export class LearningNeedsReviewModule {}
