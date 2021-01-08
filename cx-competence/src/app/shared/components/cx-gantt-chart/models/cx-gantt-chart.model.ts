import * as moment from 'moment';

export class CxGanttTaskModel {
  color: string;
  title: string;
  status?: string;
  statusIcon?: string;
  indicatorColor: string;
  startDate: Date;
  endDate: Date;
}

export class CxGanttChartModel {
  tasks: CxGanttTaskModel[];
  minYear: number;
  maxYear: number;
}

export class CxGanttChartConstant {
  static readonly NUMBER_OF_MONTHS_IN_A_YEAR: number = 12;
}

export class CxGanttTaskRenderModel {
  data: CxGanttTaskModel;
  offset: number = 0; // px
  width: number = 0; // px

  constructor(data: CxGanttTaskModel, minYear: number, yearStep: number) {
    this.data = data;
    this.processData(data, minYear, yearStep);
  }

  private processData(
    data: CxGanttTaskModel,
    minYear: number,
    yearStep: number
  ): void {
    // Calculate task item width
    this.width = this.calculateWidthBetweenTwoDate(
      data.startDate,
      data.endDate,
      yearStep
    );

    // Calculate task item offset width
    const minStartDate = new Date(minYear, 0, 1);
    this.offset = this.calculateWidthBetweenTwoDate(
      minStartDate,
      data.startDate,
      yearStep
    );
  }

  private calculateWidthBetweenTwoDate(
    d1: Date,
    d2: Date,
    widthStep: number
  ): number {
    const numberOfMonths = this.monthCounter(d1, d2);
    const numberOfYears =
      numberOfMonths / CxGanttChartConstant.NUMBER_OF_MONTHS_IN_A_YEAR;

    return numberOfYears * widthStep;
  }

  private monthCounter(d1: Date, d2: Date): number {
    const momentDate1 = moment(d1);
    const momentDate2 = moment(d2);

    return Math.abs(momentDate2.diff(momentDate1, 'months', true));
  }
}
