import { Component, OnInit, ViewEncapsulation, Output, Input, EventEmitter } from '@angular/core';
import { ActionsModel } from './models/actions.model';
import { ActionToolbarModel } from './models/actions-toolbar.model';

@Component({
  selector: 'cx-action-toolbar',
  templateUrl: './cx-action-toolbar.component.html',
  styleUrls: ['./cx-action-toolbar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CxActionToolbarComponent implements OnInit {
  @Input() actions: ActionToolbarModel;
  @Input() isVerticalToShowMenuAction: boolean;
  @Output() menuActionClick: EventEmitter<ActionsModel> = new EventEmitter<ActionsModel>();

  constructor() { }

  ngOnInit() {
  }

  onClick(action: ActionsModel): void {
    this.menuActionClick.emit(action);
  }

  get showMainActionButtons(): boolean {
    return this.actions.listEssentialActions && this.actions.listEssentialActions.length > 0;
  }

  get showMoreActionButtons(): boolean {
    return this.actions.listSpecifyActions && this.actions.listSpecifyActions.length > 0
      || this.actions.listNonEssentialActions && this.actions.listNonEssentialActions.length > 0;
  }

}
