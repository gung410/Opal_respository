import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { DateChangeEvent, EventClickEvent, SlotClassArgs, SlotClickEvent } from '@progress/kendo-angular-scheduler';
import { IPersonalEventModel, eventResources, personalCalendarModel } from '@opal20/domain-api';

import { BaseFormComponent } from '@opal20/infrastructure';

@Component({
  selector: 'calendar-view-template',
  templateUrl: './calendar-view-template.component.html'
})
export class CalendarViewTemplateComponent extends BaseFormComponent implements OnInit, OnDestroy {
  @Input() public modelFields: unknown = personalCalendarModel;
  @Input() public resources = eventResources;
  @Input() public selectedDate: Date = new Date();
  @Input() public workDayStart: string = '08:00';
  @Input() public workDayEnd: string = '18:00';
  @Input() public scrollTime: string | Date = '08:00';
  @Input() public events: Array<IPersonalEventModel> = [];
  @Input() public selectedViewIndex: number = 1;
  @Input() public tooltipClosable: boolean = false;
  @Input() public calendarHeight: string = '100%';
  @Input() public showWorkHours: boolean = false;

  @Output() public eventDblClick: EventEmitter<EventClickEvent> = new EventEmitter();
  @Output() public slotDblClick: EventEmitter<SlotClickEvent> = new EventEmitter();
  @Output() public dateChange: EventEmitter<DateChangeEvent> = new EventEmitter();

  private bodyClass: string = 'calendar-view-template-body';

  public ngOnInit(): void {
    const body = document.getElementsByTagName('body')[0];
    body.classList.add(this.bodyClass);
  }

  public ngOnDestroy(): void {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove(this.bodyClass);
  }

  public getSlotClass(args: SlotClassArgs): string {
    const today: Date = new Date();
    today.setHours(0, 0, 0, 0);
    return args.start.toDateString() === today.toDateString() ? 'today' : '';
  }

  public onEventDblClick(eventClickEvent: EventClickEvent): void {
    this.eventDblClick.emit(eventClickEvent);
  }

  public onSlotDblClick(slotClickEvent: SlotClickEvent): void {
    this.slotDblClick.emit(slotClickEvent);
  }

  public onDateChange(dateChangeEvent: DateChangeEvent): void {
    this.dateChange.emit(dateChangeEvent);
  }
}
