import { BasePageComponent, FragmentRegistry, ModuleFacadeService } from '@opal20/infrastructure';
import { CalendarSwitcherService, CalendarViewModeEnum } from '@opal20/domain-api';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'calendar-player',
  templateUrl: './calendar-player.component.html'
})
export class CalendarPlayerComponent extends BasePageComponent implements OnInit {
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
      if (viewMode) {
        this.selectedViewMode = viewMode;
      }
    });
  }
}