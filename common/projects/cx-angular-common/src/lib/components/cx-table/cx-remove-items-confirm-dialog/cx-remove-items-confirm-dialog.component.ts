import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'cx-remove-items-confirm-dialog',
  templateUrl: './cx-remove-items-confirm-dialog.component.html',
  styleUrls: ['./cx-remove-items-confirm-dialog.component.scss']
})
export class CxRemoveItemsConfirmDialogComponent<TItem> implements OnInit {
    @Input() items: TItem[];
    @Input() dialogHeaderText = 'Confirm';
    @Output() removeItems = new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

  cancel() {
    this.removeItems.emit();
  }

  remove() {
    this.removeItems.emit(this.items);
  }
}
