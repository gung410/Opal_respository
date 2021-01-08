import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  ViewChild,
} from '@angular/core';
import { Chart } from 'angular-highcharts';
import { Serie } from 'app-models/pdplan.model';
import { CHARTS_COLORS } from './learning-area-chart.constants';
import { LearningAreaChartModel } from './learning-area-chart.model';
declare var $: any;

@Component({
  selector: 'learning-area-chart',
  templateUrl: './learning-area-chart.component.html',
  styleUrls: ['./learning-area-chart.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LearningAreaChartComponent {
  @Input() learningAreaChart: LearningAreaChartModel;
  @Input() hasTitle: boolean = true;
  @ViewChild('chartComponent')
  chartComponent: Highcharts.Chart;
  @ViewChild('learningAreaChartContainer')
  learningAreaChartContainer: ElementRef;

  public getTimeSpentForPieChart(chart: Chart): string {
    // TODO: Fix this code later since it's not using in the UI
    // and the new version of angular-highchart doesn't allow to access Chart.options
    const timeSpent = 10;

    return `${timeSpent} / 100`;
  }

  onMouseEnter(eventData: MouseEvent, series: Serie): void {
    if (!this.chartRef) {
      return;
    }

    const seriesIndex = CHARTS_COLORS.indexOf(series.color);
    this.toggleSeriesOnChart(seriesIndex);
    this.toggleHoverClassLegendSeries(eventData);
    this.showChartTooltip(seriesIndex);
  }

  onMouseLeave(eventData: MouseEvent, series: Serie): void {
    if (!this.chartRef) {
      return;
    }

    const seriesIndex = CHARTS_COLORS.indexOf(series.color);
    this.toggleSeriesOnChart(seriesIndex);
    this.toggleHoverClassLegendSeries(eventData);
    this.hideChartTooltip();
  }

  private get chartRef(): Highcharts.Chart {
    return this.learningAreaChart.prioritisationChart.chart.ref;
  }

  private toggleHoverClassLegendSeries(eventData: MouseEvent): void {
    const targetElement = eventData.target as HTMLElement;
    if (!targetElement) {
      return;
    }
    targetElement.classList.toggle('legend-series-hover');
    targetElement.parentElement.classList.toggle(
      'legend-series-container-hover'
    );
  }

  private toggleSeriesOnChart(seriesIndex: number): void {
    if (!this.learningAreaChartContainer) {
      return;
    }

    const chartContainerElement = this.learningAreaChartContainer
      .nativeElement as HTMLElement;
    const highChartSeriesContainerElement = chartContainerElement.querySelector(
      '.highcharts-series'
    );
    const seriesElement = chartContainerElement.querySelector(
      `.highcharts-color-${seriesIndex}`
    );

    highChartSeriesContainerElement?.classList?.toggle(
      'highcharts-series-hover'
    );
    seriesElement?.classList?.toggle('highcharts-point-hover');
  }

  private showChartTooltip(seriesIndex: number): void {
    if (!this.chartRef || !this.chartRef.series) {
      return;
    }

    const seriesData = this.chartRef.series[0].data;
    const currentSeries = seriesData[seriesIndex];
    this.chartRef.tooltip.refresh(currentSeries);
  }

  private hideChartTooltip(): void {
    if (!this.chartRef) {
      return;
    }

    this.chartRef.tooltip.hide();
  }
}
