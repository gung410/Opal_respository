import { DateUtils } from '@opal20/infrastructure';
import { ITeamCalendarRangeDate } from '../models/team-calendar-range-date';
import { Injectable } from '@angular/core';
import { TeamCalendarViewType } from '../enums/team-calendar-view-type-enum';

@Injectable()
export class TeamCalendarSwitchViewService {
  public caculateRangeDateGantt(type: TeamCalendarViewType, addYear: number = 0): ITeamCalendarRangeDate {
    switch (type) {
      case TeamCalendarViewType.CurrentMonth:
        return {
          start: DateUtils.getMondayOfWeek(DateUtils.startOfMonth(DateUtils.addYear(new Date(), addYear))),
          end: DateUtils.getSundayOfWeek(DateUtils.endOfMonth(DateUtils.addYear(new Date(), addYear)))
        };
      case TeamCalendarViewType.ThreeMonths:
        /// **View 3 months: Previous month, current month and 1 future month.*/
        return {
          start: DateUtils.getMondayOfWeek(DateUtils.startOfMonth(DateUtils.addMonths(DateUtils.addYear(new Date(), addYear), -1))),
          end: DateUtils.getSundayOfWeek(DateUtils.endOfMonth(DateUtils.addMonths(DateUtils.addYear(new Date(), addYear), 1)))
        };
      case TeamCalendarViewType.Year:
        return {
          start: DateUtils.addYear(DateUtils.startOfYear(), addYear),
          end: DateUtils.addYear(DateUtils.endOfYear(), addYear)
        };
    }
  }
}
