import { NgModule } from '@angular/core';
import { ChartModule, HIGHCHARTS_MODULES } from 'angular-highcharts';
import { SharedModule } from 'app/shared/shared.module';
import * as more from 'highcharts/highcharts-more.src';
import { LearningAreaChartComponent } from './learning-area-chart.component';

@NgModule({
  declarations: [LearningAreaChartComponent],
  exports: [LearningAreaChartComponent],
  imports: [SharedModule, ChartModule],
  providers: [{ provide: HIGHCHARTS_MODULES, useFactory: () => [more] }],
})
export class LearningAreaChartModule {}
