import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { LearningAreaChartModule } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.module';
import { LearningNeedsPreviewComponent } from './learning-needs-preview.component';

@NgModule({
  declarations: [LearningNeedsPreviewComponent],
  exports: [LearningNeedsPreviewComponent],
  imports: [SharedModule, LearningAreaChartModule],
})
export class LearningNeedsPreviewModule {}
