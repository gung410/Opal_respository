import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CalendarAccessSharingsResult, ICalendarAccessSharingsResult } from '../models/calendar-access-sharings-result';

import { GetCalendarAccessSharingRequest } from '../dtos/get-calendar-access-sharings-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SaveTeamCalendarAccessSharingsRequest } from '../dtos/save-team-calendar-access-sharings-request';
import { map } from 'rxjs/operators';

@Injectable()
export class TeamCalendarSharingService extends BaseBackendService {
  private readonly teamCalendarSharingRoute: string = '/calendars/team/shares';

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public getCalendarAccessSharings(
    request: GetCalendarAccessSharingRequest,
    showSpinner: boolean = true
  ): Observable<ICalendarAccessSharingsResult> {
    return this.get<ICalendarAccessSharingsResult>(this.teamCalendarSharingRoute, request, showSpinner).pipe(
      map(e => new CalendarAccessSharingsResult(e))
    );
  }

  public saveCalendarAccessSharings(request: SaveTeamCalendarAccessSharingsRequest, showSpinner: boolean = true): Promise<unknown> {
    return this.post(this.teamCalendarSharingRoute, request, showSpinner).toPromise();
  }
}
