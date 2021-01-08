import { Component, Input, OnInit } from '@angular/core';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AppConstant } from 'app/shared/app.constant';
import { CxGanttTaskModel } from '../models/cx-gantt-chart.model';

@Component({
  selector: 'cx-gantt-task',
  templateUrl: './cx-gantt-task.component.html',
  styleUrls: ['./cx-gantt-task.component.scss'],
})
export class CxGanttTaskComponent implements OnInit {
  @Input() set data(value: CxGanttTaskModel) {
    this.taskData = value;
  }
  taskData: CxGanttTaskModel;

  constructor() {}

  ngOnInit(): void {}

  get getStartDate(): string {
    if (!this.taskData || !this.taskData.startDate) {
      return;
    }

    return this.getDateString(this.taskData.startDate);
  }

  get getEndDate(): string {
    if (!this.taskData || !this.taskData.endDate) {
      return;
    }

    return this.getDateString(this.taskData.endDate);
  }

  private getDateString(date: Date): string {
    return DateTimeUtil.toDateString(date, AppConstant.backendDateFormat);
  }
}
