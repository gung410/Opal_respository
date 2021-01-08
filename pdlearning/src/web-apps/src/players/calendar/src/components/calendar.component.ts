import { BasePageComponent, FragmentRegistry, ModuleFacadeService } from '@opal20/infrastructure';
import { CalendarSwitcherService, CalendarViewModeEnum } from '@opal20/domain-api';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'calendar',
  templateUrl: './calendar.component.html'
})
export class CalendarComponent extends BasePageComponent implements OnInit {
  @Input() public calendarHeight: string = '100%';

  @Input()
  public set viewMode(viewMode: CalendarViewModeEnum) {
    this.calendarSwitcherService.changeViewMode(viewMode);
  }

  public viewModeEnum: typeof CalendarViewModeEnum = CalendarViewModeEnum;
  public selectedViewMode: CalendarViewModeEnum;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public fragmentRegistry: FragmentRegistry,
    private calendarSwitcherService: CalendarSwitcherService
  ) {
    super(moduleFacadeService);

    this.registerOnCalendarChange();
    this.calendarSwitcherService.detectViewMode();
  }

  private registerOnCalendarChange(): void {
    this.subscribe(this.calendarSwitcherService.calendarViewMode, viewMode => {
      this.onViewChangeHandler(viewMode);
    });
  }

  private onViewChangeHandler(viewMode: CalendarViewModeEnum): void {
    if (viewMode) {
      this.selectedViewMode = viewMode;
    }
  }
}
