import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'staff-show-hide-columns',
  templateUrl: './staff-show-hide-column.component.html',
  styleUrls: ['./staff-show-hide-column.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class StaffShowHideComponent extends BaseSmartComponent {
  staffListColumnDef: any;
  $event: any;
  columnNeedToHide: any;
  @Output() changeSelected: EventEmitter<{}> = new EventEmitter<{}>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() changeShowHideColumn: EventEmitter<{}> = new EventEmitter<{}>();
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  changeSelectedColumn($event: any, columnNeedToHide: any): any {
    this.changeShowHideColumn.emit({ $event, columnNeedToHide });
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
