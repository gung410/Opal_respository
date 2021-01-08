import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component, Input } from '@angular/core';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { isEmpty } from 'lodash';
import { StatusMapperService } from '../services/status-mapper.service';

@Component({
  selector: 'status-indicator',
  template: `
    <div class="status-indicator">
      <span
        class="status-indicator__dot"
        [style.background-color]="color"
      ></span>
      <span class="status-indicator__text">
        <span class="status-indicator__status-text">
          {{ text | translate }}
        </span>
      </span>
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class StatusCellRendererComponent implements ICellRendererAngularComp {
  @Input() color: string;
  @Input() text: string;

  constructor(private statusMapperService: StatusMapperService) {}

  // called on init
  agInit(param: any): boolean {
    this.processParam(param);

    return true;
  }

  // called when the cell is refreshed
  refresh(param: any): boolean {
    this.processParam(param);

    return true;
  }

  getTimeFromNow(date: string): string {
    return DateTimeUtil.getTimeFromNow(date);
  }

  private processParam(param: any): void {
    if (!param || isEmpty(param.value)) {
      return;
    }

    const statusMapper = this.statusMapperService.getStatusMapper(param.value);
    if (statusMapper) {
      this.color = statusMapper.color;
      this.text = statusMapper.text;
    }
  }
}
