import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Output,
  ViewEncapsulation
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'user-show-hide-columns',
  templateUrl: './user-show-hide-column.component.html',
  styleUrls: ['./user-show-hide-column.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserShowHideComponent extends BaseSmartComponent {
  userListColumnDef: any;
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
