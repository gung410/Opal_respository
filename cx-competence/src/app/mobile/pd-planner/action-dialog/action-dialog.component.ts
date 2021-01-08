import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Constant } from 'app/shared/app.constant';

@Component({
  selector: 'action-dialog',
  templateUrl: './action-dialog.component.html',
  styleUrls: ['./action-dialog.component.scss'],
})
export class ActionDialogComponent implements OnInit {
  @Input() title: string = Constant.STRING_EMPTY;
  @Input() actionMessage: string = Constant.STRING_EMPTY;
  @Output() action = new EventEmitter<any>();
  constructor() {}

  ngOnInit() {}

  doAction() {
    this.action.emit();
  }
}
