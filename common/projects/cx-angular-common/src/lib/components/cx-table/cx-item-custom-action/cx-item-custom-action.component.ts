import { Component, OnInit, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { CxAbstractTableComponent } from '../cx-abstract-table.component';

@Component({
  selector: 'cx-item-custom-action',
  templateUrl: './cx-item-custom-action.component.html',
  styleUrls: ['./cx-item-custom-action.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CxItemCustomActionComponent<TItem> implements OnInit {
    @Input() item: TItem;
    @Input() icon = 'material-icons extension';
    @Output() customActionClick = new EventEmitter<TItem>();
  constructor() { }

  ngOnInit() {
  }
  onItemClicked() {
      this.customActionClick.emit(this.item);
  }
}
