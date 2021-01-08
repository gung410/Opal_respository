import { BaseComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { FormGroup } from '@angular/forms';

@Component({
  selector: 'archival-tab',
  templateUrl: './archival-tab.component.html'
})
export class ArchivalTabComponent extends BaseComponent {
  // input region
  @Input('form') public form: FormGroup;
  @Input() public readOnly: boolean = false;
  @Input() public date: Date;
  @Input() public disableDate?: (date: Date) => boolean;

  // output region
  @Output() public onDateChange: EventEmitter<Date> = new EventEmitter<Date>();

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public disabledDatesInThePast = (date: Date): boolean => {
    const today = DateUtils.removeTime(new Date());
    const dateToCompate = DateUtils.removeTime(date);

    return dateToCompate <= today;
  };

  public onDateInputChange(date: Date): void {
    this.onDateChange.emit(date);
  }
}
