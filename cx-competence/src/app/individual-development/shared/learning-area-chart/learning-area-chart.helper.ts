import { Chart } from 'angular-highcharts';
import { ChartInfo, ReportData, Serie } from 'app-models/pdplan.model';
import {
  PositionObject,
  SeriesPieOptions,
  TooltipFormatterContextObject,
} from 'highcharts';
import { cloneDeep } from 'lodash';
import {
  CHARTS_COLORS,
  DEFAULT_JSON_PIE,
  MAX_CHART_LEVEL,
  MAX_HOURS,
} from './learning-area-chart.constants';
declare var $: any;

export default class LearningAreaChartHelper {
  public static reportDataToChartInfo(reportData: ReportData): ChartInfo {
    const chartOptions = this.renderPolarChart(
      reportData.name,
      reportData.series.map((serie, index) => [
        0,
        serie.value === MAX_CHART_LEVEL
          ? 1
          : serie.value === 1
          ? MAX_CHART_LEVEL
          : serie.value,
        serie.name,
      ])
    );
    this.customisePolarChartOptions(chartOptions);
    reportData.series.forEach((value, index) => {
      const colorIdx = index % CHARTS_COLORS.length;
      value.color = CHARTS_COLORS[colorIdx];
    });

    const chart = new Chart(chartOptions);

    return new ChartInfo({
      header: reportData.name,
      series: reportData.series,
      chart,
    });
  }

  public static seriesToPieCharts(series: Serie[]): Chart[] {
    const clonedSeries = cloneDeep(series);

    return clonedSeries.map((serie) => this.serieToPieChart(serie));
  }

  public static serieToPieChart(serie: Serie): Chart {
    const jsonPie: Highcharts.Options = cloneDeep(DEFAULT_JSON_PIE);
    jsonPie.colors = [serie.color, (serie.color += '50')];
    const chartSpent = Math.floor(
      (this.convertChartValue(serie.value) * MAX_HOURS) / MAX_CHART_LEVEL
    );
    const chartNotSpent = MAX_HOURS - chartSpent;
    if (jsonPie.series && jsonPie.series[0]) {
      const firstSeriesPieOptions = jsonPie.series[0] as SeriesPieOptions;
      if (firstSeriesPieOptions) {
        firstSeriesPieOptions.data = [
          ['Spent', chartSpent],
          ['Notspent', chartNotSpent],
        ];
      }
    }

    return new Chart(jsonPie);
  }

  private static levelMapping: string[] = ['Low', 'Moderate', 'High'];

  private static convertChartValue(value: number): number {
    if (value === MAX_CHART_LEVEL) {
      return 1;
    }
    if (value === 1) {
      return MAX_CHART_LEVEL;
    }

    return value;
  }

  private static customisePolarChartOptions(
    chartOptions: Highcharts.Options
  ): void {
    // tslint:disable:no-string-literal
    chartOptions.yAxis['plotBands'] = { from: -1, to: 0, color: '#ffffff' };
    chartOptions.plotOptions.series['colorByPoint'] = true;
    chartOptions.plotOptions.series['groupPadding'] = 0;
    chartOptions.plotOptions.series['pointPadding'] = 0;
    chartOptions.plotOptions.series['borderWidth'] = 0;
    // tslint:enable:no-string-literal
  }

  private static renderPolarChart(
    name: string,
    series: any
  ): Highcharts.Options {
    return {
      colors: CHARTS_COLORS,
      credits: {
        enabled: false,
      },
      pane: {
        background: [
          {
            backgroundColor: '#EBEBEB',
          },
        ],
      },
      chart: {
        polar: true,
        height: '100%',
        width: 330,
      },
      title: {
        text: '',
        floating: true,
      },
      legend: {
        enabled: false,
      },
      tooltip: {
        className: 'learning-area-chart-tooltip-container',
        hideDelay: 100,
        // tslint:disable-next-line:object-literal-shorthand
        formatter: function (): string {
          return LearningAreaChartHelper.formatterPieChartToolTip(this, series);
        },
        useHTML: true,
        outside: true,
        positioner: LearningAreaChartHelper.calculateChartPosition,
      },
      xAxis: {
        gridZIndex: 200,
        gridLineColor: '#fff',
        gridLineWidth: 1,
        lineColor: 'gray',
        labels: {
          style: {
            whiteSpace: 'nowrap',
          },
          enabled: false,
        },
        categories: [],
      },
      yAxis: {
        gridZIndex: 200,
        min: -1,
        max: 3,
        tickInterval: 1,
        title: {
          text: null,
        },

        labels: {
          enabled: false,
        },
        gridLineWidth: 1,
        tickLength: 0,
        gridLineColor: '#fff',
        lineColor: '#fff',
      },
      plotOptions: {
        series: {},
      },
      series: [
        {
          name,
          type: 'columnrange',
          data: series.map((item) => [item[0], item[1]]),
          states: {
            hover: {
              enabled: false,
            },
          },
          point: {
            events: {
              mouseOver: LearningAreaChartHelper.toggleHoverSeriesOnChart,
              mouseOut: LearningAreaChartHelper.toggleHoverSeriesOnChart,
            },
          },
        },
      ],
    };
  }

  private static toggleHoverSeriesOnChart(eventData: any): void {
    const pathElement = eventData?.target?.graphic?.element as HTMLElement;
    if (!pathElement) {
      return;
    }

    // Toggle path
    pathElement.classList.toggle('highcharts-point-hover');

    // Toggle legend
    const pathElement2ndClass = pathElement.classList[1];
    const targetClassIndex = 2;
    const seriesIndex = pathElement2ndClass?.split('-')[targetClassIndex];
    const chartContainerElement = pathElement.closest(
      '.learning-area-chart-container'
    );
    const legendContainer = chartContainerElement?.querySelector(
      '.legends-container'
    );
    legendContainer?.classList.toggle('legend-series-container-hover');
    const legendItem = legendContainer?.querySelector(
      `#legend-series-${seriesIndex}`
    );
    legendItem?.classList.toggle('legend-series-hover');
  }

  private static formatterPieChartToolTip(
    tooltipFormatterContextObject: TooltipFormatterContextObject,
    seriesData: []
  ): string {
    const seriesPoint = tooltipFormatterContextObject.point as any;
    const color = tooltipFormatterContextObject.color;
    const index = tooltipFormatterContextObject.colorIndex;
    const areaNameItemIndex = 2;
    const areaLevel = seriesPoint.high;
    const areaLevelName = LearningAreaChartHelper.levelMapping[areaLevel - 1];
    const areaName = seriesData[index][areaNameItemIndex];

    return `
      <div class="area-chart-tooltip">
        <div class="area-chart-tooltip-area">
          <div class="area-chart-tooltip__label">
            Learning Area
          </div>
          <div class="area-chart-tooltip__name">
            <div class="area-chart-tooltip-area-color" style="background: ${color}"></div>
            ${areaName}
          </div>
        </div>
        <div class="area-chart-tooltip-prioritisation ">
          <div class="area-chart-tooltip__label">
            Prioritisation
          </div>
          <div class="area-chart-tooltip__name">
            ${areaLevelName}
          </div>
        </div>
      </div>
    `;
  }

  private static calculateChartPosition(): PositionObject {
    const that: any = this;
    const chartObject = that.chart;
    const chartContainerElement = chartObject?.container;
    const chartContainerOffset = $(chartContainerElement)?.offset();
    const chartWidth = chartObject?.chartWidth;
    const paddingValue = 100;
    const tooltipOffsetX =
      chartContainerOffset.left + chartWidth - paddingValue;
    const tooltipOffsetY = chartContainerOffset.top;

    return {
      x: tooltipOffsetX,
      y: tooltipOffsetY,
    };
  }
}
