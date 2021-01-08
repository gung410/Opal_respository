export const CHART_DESCRIPTION: any = {
  colors: [
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
    '#78D0EC',
  ],
  credits: {
    enabled: false,
  },
  pane: {
    background: {
      backgroundColor: '#EBEBEB',
    },
  },
  chart: {
    polar: true,
    height: '100%',
    width: 330,
    events: {
      load() {
        const chart = this;
        chart.renderer
          .text('LOW', 170, 85)
          .attr({
            zIndex: 1000,
          })
          .css({
            fontSize: '12px',
            fontWeight: 'bold',
          })
          .add();

        chart.renderer
          .text('MODERATE', 200, 130)
          .attr({
            zIndex: 1000,
          })
          .css({
            fontSize: '12px',
            fontWeight: 'bold',
          })
          .add();

        chart.renderer
          .text('HIGH', 210, 180)
          .attr({
            zIndex: 1000,
          })
          .css({
            fontSize: '12px',
            fontWeight: 'bold',
          })
          .add();
      },
    },
  },
  title: {
    text: '',
    floating: true,
  },
  legend: {
    enabled: false,
  },
  tooltip: {
    enabled: false,
  },
  xAxis: {
    gridZIndex: 100,
    gridLineColor: '#fff',
    gridLineWidth: 2,
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
    plotBands: {
      from: -1,
      to: 0,
      color: '#fff',
    },
  },
  plotOptions: {
    series: {
      colorByPoint: true,
      groupPadding: 0,
      pointPadding: 0,
      borderWidth: 0,
    },
  },
  series: [
    {
      type: 'columnrange',
      data: [
        [0, 1],
        [0, 2],
        [0, 3],
        [0, 0],
        [0, 0],
        [0, 0],
        [0, 0],
        [0, 0],
      ],
    },
  ],
};

export const MAX_HOURS: number = 100;

export const MAX_CHART_LEVEL: number = 3;

export const CHARTS_COLORS: string[] = [
  '#ff6b81',
  '#63cce1',
  '#1e90ff',
  '#78ecae',
  '#ff7f50',
  '#9341d9',
  '#e2c335',
  '#e44821',
  '#4a2784',
  '#322759',
  '#b270cf',
  '#d344b7',
  '#62221e',
  '#8b9ddb',
  '#76e23f',
  '#883678',
  '#519782',
  '#ace585',
  '#467937',
  '#b948df',
  '#49a638',
  '#99bd7f',
  '#90872f',
  '#8f8a67',
  '#5ee8a7',
  '#4f1d3a',
  '#613ac9',
  '#30412c',
  '#44551e',
  '#d8397b',
  '#56aad2',
  '#87667f',
  '#daae58',
  '#ad753a',
  '#de434c',
  '#d8ac9c',
  '#dcdd7a',
  '#e28536',
  '#db74a8',
  '#2b2931',
  '#4b66a8',
  '#4b6f7f',
  '#d3ddb6',
  '#a03b52',
  '#d8a1d2',
  '#88ab36',
  '#a64224',
  '#b7c3d4',
  '#4ab26e',
  '#77e2cb',
];

export const DEFAULT_JSON_PIE: Highcharts.Options = {
  credits: {
    enabled: false,
  },
  colors: ['#ED324E', '#FFBEC4'],
  chart: {
    type: 'pie',
    plotBackgroundColor: null,
    plotBorderWidth: 0,
    plotShadow: false,
    height: '100%',
  },
  tooltip: {
    enabled: false,
  },
  plotOptions: {
    pie: {
      allowPointSelect: true,
      cursor: 'pointer',
      dataLabels: {
        enabled: false,
      },
    },
  },
  title: {
    text: '',
    align: 'center',
    verticalAlign: 'middle',
  },
  series: [
    {
      type: 'pie',
      name: 'Browser share',
      innerSize: '50%',
      data: [
        ['Spent', 20],
        ['Notspent', 80],
      ],
    },
  ],
};
