import { BaseComponent, NotificationType } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { ButtonAction } from './../../models/button-action.model';

@Component({
  selector: 'action-bar',
  templateUrl: './action-bar.component.html'
})
export class ActionBarComponent extends BaseComponent {
  @Input() public buttons: ButtonAction<unknown>[] = [];
  @Input() public selectedItems: unknown[] = [];
  @Input() public showMoreButton: boolean = false;
  public onClick(button: ButtonAction<unknown>): void {
    if (button.actionFn != null) {
      const executeItems = this.filterSatisfyingItems(button);
      if (executeItems.length > 0) {
        button.actionFn(executeItems).then(_ => {
          if (_) {
            this.showNotificationForUser(executeItems.length, this.selectedItems.length);
            this.selectedItems = [];
          }
        });
      } else {
        this.showNotificationForUser(executeItems.length, this.selectedItems.length);
      }
    }
  }

  public disableButton(button: ButtonAction<unknown>): boolean {
    return this.selectedItems.length === 0 || this.filterSatisfyingItems(button).length === 0;
  }

  public showButton(button: ButtonAction<unknown>): boolean {
    return button.actionFn != null && (button.hiddenFn == null || !button.hiddenFn());
  }

  private filterSatisfyingItems(button: ButtonAction<unknown>): unknown[] {
    return this.selectedItems.filter(item => button.conditionFn == null || button.conditionFn(item) === true);
  }

  private showNotificationForUser(numberOfExecuteItems: number, totalItemNumber: number): void {
    if (numberOfExecuteItems !== totalItemNumber) {
      this.showNotification(
        this.translateCommon(
          `${numberOfExecuteItems} item(s) updated successfully. ${totalItemNumber - numberOfExecuteItems} item(s) failed`
        ),
        NotificationType.Warning
      );
    } else {
      this.showNotification(this.translateCommon('All selected item(s) updated successfully'), NotificationType.Success);
    }
  }
}
