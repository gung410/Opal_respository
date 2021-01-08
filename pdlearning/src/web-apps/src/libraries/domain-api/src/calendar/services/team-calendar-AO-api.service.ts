import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ITeamMemberEventModel, TeamMemberEventModel } from '../models/team-member-event.model';
import { ITeamMemberEventOverviewModel, TeamMemberEventOverviewModel } from '../models/team-member-event-overview.model';
import { ITeamMemberModel, TeamMemberModel } from '../models/team-member-model';

import { GetTeamMemberEventOverviewsRequest } from '../dtos/get-team-member-event-overviews-request';
import { GetTeamMemberEventsRequest } from '../dtos/get-team-member-events-request';
import { ITeamCalendarApiResolverService } from './team-calendar-api-resolver.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TeamCalendarViewMode } from '../enums/team-calendar-view-mode-enum';
import { map } from 'rxjs/operators';

@Injectable()
export class TeamCalendarAOApiService extends BaseBackendService implements ITeamCalendarApiResolverService {
  private readonly teamRoute: string = '/calendars/team';

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public initParams(): void {
    // No need any param
  }

  public getViewMode(): TeamCalendarViewMode {
    return TeamCalendarViewMode.AOView;
  }

  public getTeamMemberEventOverviews(
    request: GetTeamMemberEventOverviewsRequest,
    showSpinner: boolean
  ): Observable<ITeamMemberEventOverviewModel[]> {
    return this.get<ITeamMemberEventOverviewModel[]>(`${this.teamRoute}/events`, request, showSpinner).pipe(
      map(_ => _.map(e => new TeamMemberEventOverviewModel(e)))
    );
  }

  public getTeamMemberEvents(request: GetTeamMemberEventsRequest, showSpinner: boolean): Observable<ITeamMemberEventModel[]> {
    return this.get<ITeamMemberEventModel[]>(`${this.teamRoute}/members/${request.memberId}/events`, request, showSpinner).pipe(
      map(_ => _.map(e => new TeamMemberEventModel(e)))
    );
  }

  public getTeamMembers(showSpinner: boolean): Observable<ITeamMemberModel[]> {
    return this.get<ITeamMemberModel[]>(`${this.teamRoute}/members`, null, showSpinner).pipe(map(_ => _.map(e => new TeamMemberModel(e))));
  }
}
