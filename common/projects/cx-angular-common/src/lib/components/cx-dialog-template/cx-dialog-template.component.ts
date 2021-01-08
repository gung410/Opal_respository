import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CxButtonComponent } from '../cx-button/cx-button.component';
@Component({
  selector: 'cx-dialog-template',
  templateUrl: './cx-dialog-template.component.html',
  styleUrls: ['./cx-dialog-template.component.scss']
})
export class CxDialogTemplateComponent implements OnInit {
  @Input() cancelButtonText = 'Cancel';
  @Input() doneButtonText = 'Done';
  @Input() doneButtonDisabled = false;
  @Input() showFooterActionButtons = true;
  @Input() showCustomFooter = false;
  @Input() showModalHeader = true;
  @Input() showBorderHeader = true;
  @Input() showCancelButton = true;
  @Input() showConfirmButton = true;
  @Input() doneButtonType: string = CxButtonComponent.Types.primary;
  @Input() showCloseButton = true;
  @Input() fixedHeight: boolean;

  @Output() cancel: EventEmitter<any> = new EventEmitter();
  @Output() done: EventEmitter<any> = new EventEmitter();

  @Input() isShowingMandatoryText: boolean;
  public cancelButtonType = CxButtonComponent.Types.outlineSecondary;
  constructor() {
  }

  ngOnInit() {}

  onCancel() {
    this.cancel.emit();
  }

  onDone() {
    this.done.emit();
  }

}
