import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'cx-confirmation-dialog',
  templateUrl: './cx-confirmation-dialog.component.html',
  styleUrls: ['./cx-confirmation-dialog.component.scss']
})
export class CxConfirmationDialogComponent implements OnInit {
  @Input() header: string;
  @Input() content: string;
  @Input() cancelButtonText = 'Cancel';
  @Input() confirmButtonText = 'OK';
  @Input() isDanger: boolean;
  @Input() showCloseButton = true;
  @Input() showCancelButton = true;
  @Input() showConfirmButton = true;
  @Output() confirm: EventEmitter<any> = new EventEmitter();
  @Output() cancel: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  public onClickSubmit() {
    this.confirm.emit();
  }

  public onClickCancel() {
    this.cancel.emit();
  }
}
