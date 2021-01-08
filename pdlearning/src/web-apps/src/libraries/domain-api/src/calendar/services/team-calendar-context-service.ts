import { BehaviorSubject } from 'rxjs';
import { ITeamCalendarContextModel } from '../models/team-calendar-context.model';
import { Injectable } from '@angular/core';
@Injectable()
export class TeamCalendarContextService {
  public teamCalendarContext: BehaviorSubject<ITeamCalendarContextModel> = new BehaviorSubject(null);

  public changeContext(context: ITeamCalendarContextModel): void {
    this.teamCalendarContext.next(context);
  }
}
