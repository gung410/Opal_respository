import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'user-groups-remove-confirmation',
  templateUrl: './user-groups-remove-confirmation.component.html',
  styleUrls: ['./user-groups-remove-confirmation.component.scss']
})
export class UserGroupsRemoveConfirmationComponent implements OnInit {
  @Input() message: string;
  @Input() doneButtonText: string;
  @Input() number: number;
  @Input() removedGroups: any[] = [];
  @Input() status: string;
  @Output() done: EventEmitter<any> = new EventEmitter();
  @Output() cancel: EventEmitter<any> = new EventEmitter();
  response: any = {};
  doneButtonDisabled: boolean = false;

  ngOnInit(): void {}

  onComplete(): void {
    this.done.emit(this.response);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
