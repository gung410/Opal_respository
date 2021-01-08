import { BaseComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { FormGroup } from '@angular/forms';
import { IDueDate } from '@opal20/domain-api';

@Component({
  selector: 'due-date-tab',
  templateUrl: './due-date-tab.component.html'
})
export class DueDateTabComponent extends BaseComponent {
  // input region
  @Input('form') public form: FormGroup;
  @Input() public isSendNotification: boolean = true;
  @Input() public formRemindDueDate: Date;
  @Input() public remindBeforeDays: number = null;
  @Input() public readonly: boolean = true;

  // output region
  @Output('dueDateDataChange') public dueDateDataChangeEvent: EventEmitter<IDueDate> = new EventEmitter<IDueDate>();

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public disabledDatesInThePast = (date: Date): boolean => {
    const today = DateUtils.removeTime(new Date());
    const dateToCompate = DateUtils.removeTime(date);

    return dateToCompate <= today;
  };

  public get maximumDatesRemind(): number {
    const maximumDates = DateUtils.calculateAmountOfDayFromPresentToDate(this.formRemindDueDate);
    return maximumDates;
  }

  public onDueDateDataChange(): void {
    const dueDateData: IDueDate = {
      formRemindDueDate: this.formRemindDueDate,
      remindBeforeDays: this.remindBeforeDays,
      isSendNotification: this.isSendNotification
    };
    this.dueDateDataChangeEvent.emit(dueDateData);
  }
}
