import {
  CxGanttChartModel,
  CxGanttTaskModel,
} from 'app/shared/components/cx-gantt-chart/models/cx-gantt-chart.model';
import { isEmpty } from 'lodash';

export enum CareerAspirationStatusTypeEnum {
  Current = 'Current',
  Previous = 'Previous',
}

export class CareerAspirationGanttChartStyleScheme {
  color: string;
  text: string;
  icon: string;
}

export class CareerAspirationItemDto {
  title: string;
  type: string;
  startDate: string;
  endDate: string;
}

export class CareerAspirationChartDataDto {
  careerAspirations: CareerAspirationItemDto[];
  minStartDate: string;
  maxEndDate: string;
}

export class CareerGanttChartConstant {
  static readonly CAREER_ASPIRATION_STATUS_MAPPER: any = {
    [CareerAspirationStatusTypeEnum.Previous]: {
      color: '#8799BA',
      text: 'Previous goal',
      icon: 'icon-turn-around',
    },
    [CareerAspirationStatusTypeEnum.Current]: {
      color: '#7BDEAA',
      text: 'Current goal',
      icon: 'icon-circle',
    },
  };
}

export class CxGanttChartCarrerAspirationItem extends CxGanttTaskModel {
  constructor(dto: CareerAspirationItemDto) {
    super();
    if (!dto) {
      return;
    }

    this.title = dto.title;

    if (dto.startDate) {
      this.startDate = new Date(dto.startDate);
    }

    if (dto.endDate) {
      this.endDate = new Date(dto.endDate);
    }
    this.processStatus(dto.type);
  }

  private processStatus(statusType: string): void {
    if (statusType === null) {
      return;
    }

    const statusStyleScheme: CareerAspirationGanttChartStyleScheme =
      CareerGanttChartConstant.CAREER_ASPIRATION_STATUS_MAPPER[statusType];

    if (!statusStyleScheme) {
      return;
    }

    this.color = statusStyleScheme.color;
    this.status = statusStyleScheme.text;
    this.statusIcon = statusStyleScheme.icon;
  }
}

export class CareerAspirationChartDataModel extends CxGanttChartModel {
  constructor(dto: CareerAspirationChartDataDto) {
    super();

    if (!dto) {
      return;
    }

    this.processMinMaxDate(dto);

    if (!isEmpty(dto.careerAspirations)) {
      this.tasks = dto.careerAspirations.map(
        (i) => new CxGanttChartCarrerAspirationItem(i)
      );
    }
  }

  private processMinMaxDate(dto: CareerAspirationChartDataDto): void {
    if (!!dto.minStartDate) {
      this.minYear = new Date(dto.minStartDate).getFullYear();
    }

    if (!!dto.maxEndDate) {
      this.maxYear = new Date(dto.maxEndDate).getFullYear();
    }
  }
}
