import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ISharedTeamModel, SharedTeamModel } from '../models/shared-team-model';
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
export class TeamCalendarLearnerApiService extends BaseBackendService implements ITeamCalendarApiResolverService {
  private readonly teamShareRoute: string = '/calendars/team/shares';
  private accessSharingId: string;

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public initParams(accessSharingId: string): void {
    this.accessSharingId = accessSharingId;
  }

  public getViewMode(): TeamCalendarViewMode {
    return TeamCalendarViewMode.LearnerView;
  }

  public getMySharedTeams(showSpinner: boolean = true): Observable<ISharedTeamModel[]> {
    return this.get<ISharedTeamModel[]>(`${this.teamShareRoute}/mySharedTeams`, null, showSpinner).pipe(
      map(_ => _.map(e => new SharedTeamModel(e)))
    );
  }

  public getTeamMembers(showSpinner: boolean): Observable<ITeamMemberModel[]> {
    return this.get<ITeamMemberModel[]>(`${this.teamShareRoute}/mySharedTeams/${this.accessSharingId}/members`, null, showSpinner).pipe(
      map(_ => _.map(e => new TeamMemberModel(e)))
    );
  }

  public getTeamMemberEvents(request: GetTeamMemberEventsRequest, showSpinner: boolean): Observable<ITeamMemberEventModel[]> {
    return this.get<ITeamMemberEventModel[]>(
      `${this.teamShareRoute}/mySharedTeams/${this.accessSharingId}/members/${request.memberId}/events`,
      request,
      showSpinner
    ).pipe(map(_ => _.map(e => new TeamMemberEventModel(e))));
  }

  public getTeamMemberEventOverviews(
    request: GetTeamMemberEventOverviewsRequest,
    showSpinner: boolean
  ): Observable<ITeamMemberEventOverviewModel[]> {
    return this.get<ITeamMemberEventOverviewModel[]>(
      `${this.teamShareRoute}/mySharedTeams/${this.accessSharingId}/events`,
      request,
      showSpinner
    ).pipe(map(_ => _.map(e => new TeamMemberEventOverviewModel(e))));
  }
}
