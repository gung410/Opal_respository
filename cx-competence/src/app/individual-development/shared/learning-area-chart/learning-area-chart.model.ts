import { Chart } from 'angular-highcharts';
import { ChartInfo } from 'app-models/pdplan.model';

export class LearningAreaChartModel {
  prioritisationChart: ChartInfo;
  timeSpentCharts: Chart[];
  constructor(data?: Partial<LearningAreaChartModel>) {
    if (!data) {
      return;
    }
    this.prioritisationChart = data.prioritisationChart
      ? data.prioritisationChart
      : undefined;
    this.timeSpentCharts = data.timeSpentCharts ? data.timeSpentCharts : [];
  }
}
