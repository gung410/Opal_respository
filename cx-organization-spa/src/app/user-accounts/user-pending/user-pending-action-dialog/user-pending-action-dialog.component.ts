import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'user-pending-action-dialog',
  templateUrl: './user-pending-action-dialog.component.html',
  styleUrls: ['./user-pending-action-dialog.component.scss']
})
export class UserPendingActionDialogComponent<TItem> {
  @Input() items: TItem[];
  @Input() dialogHeaderText: string;
  @Output() doneAction: EventEmitter<unknown> = new EventEmitter();

  cancel(): void {
    this.doneAction.emit();
  }

  done(): void {
    this.doneAction.emit(this.items);
  }
}
