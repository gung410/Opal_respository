import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { TaxonomyRequestStatus } from 'app/taxonomy-management/constant/taxonomy-request-status.enum';

@Component({
  selector: 'cell-requested-date',
  templateUrl: './cell-requested-date.component.html',
  styleUrls: ['./cell-requested-date.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellRequestedDateComponent implements ICellRendererAngularComp {
  requestedDateString: string;

  agInit(params: any): void {
    this.requestedDateString = this.createdDateComparator(params.value);
  }

  refresh(params?: any): boolean {
    return true;
  }

  private createdDateComparator(date: Date): string {
    return DateTimeUtil.toDateString(date);
  }
}
