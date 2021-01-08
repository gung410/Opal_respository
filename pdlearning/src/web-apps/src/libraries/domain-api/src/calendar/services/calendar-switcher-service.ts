import { BehaviorSubject } from 'rxjs';
import { CalendarIntergrationService } from './calendar-intergration.service';
import { CalendarViewModeEnum } from '../enums/calendar-view-mode-enum';
import { Injectable } from '@angular/core';

@Injectable()
export class CalendarSwitcherService {
  public calendarViewMode: BehaviorSubject<CalendarViewModeEnum> = new BehaviorSubject(null);
  constructor(protected calendarIntegrationService: CalendarIntergrationService) {}

  public changeViewMode(mode: CalendarViewModeEnum): void {
    this.calendarViewMode.next(mode);
  }

  public detectViewMode(): void {
    const params = this.calendarIntegrationService.GetParams();
    let selectedCalendarViewMode: CalendarViewModeEnum;

    switch (params.viewMode as CalendarViewModeEnum) {
      case CalendarViewModeEnum.Personal:
        selectedCalendarViewMode = CalendarViewModeEnum.Personal;
        break;
      case CalendarViewModeEnum.Communities:
        selectedCalendarViewMode = CalendarViewModeEnum.Communities;
        break;
      case CalendarViewModeEnum.Community:
        selectedCalendarViewMode = CalendarViewModeEnum.Community;
        break;
      case CalendarViewModeEnum.Team:
        selectedCalendarViewMode = CalendarViewModeEnum.Team;
        break;

      default:
        selectedCalendarViewMode = CalendarViewModeEnum.Personal;
    }

    this.changeViewMode(selectedCalendarViewMode);
  }
}
