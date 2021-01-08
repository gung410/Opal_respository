import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { BehaviorSubject, Observable } from 'rxjs';

import { GetTeamMemberEventOverviewsRequest } from '../dtos/get-team-member-event-overviews-request';
import { GetTeamMemberEventsRequest } from '../dtos/get-team-member-events-request';
import { ITeamMemberEventModel } from '../models/team-member-event.model';
import { ITeamMemberEventOverviewModel } from '../models/team-member-event-overview.model';
import { ITeamMemberModel } from '../models/team-member-model';
import { Injectable } from '@angular/core';
import { TeamCalendarAOApiService } from './team-calendar-AO-api.service';
import { TeamCalendarLearnerApiService } from './team-calendar-learner-api.service';
import { TeamCalendarViewMode } from '../enums/team-calendar-view-mode-enum';

export interface ITeamCalendarApiResolverService {
  initParams(...parameters: unknown[]): void;
  getTeamMembers(showSpinner: boolean): Observable<ITeamMemberModel[]>;
  getTeamMemberEvents(request: GetTeamMemberEventsRequest, showSpinner: boolean): Observable<ITeamMemberEventModel[]>;
  getTeamMemberEventOverviews(
    request: GetTeamMemberEventOverviewsRequest,
    showSpinner: boolean
  ): Observable<ITeamMemberEventOverviewModel[]>;
  getViewMode(): TeamCalendarViewMode;
}

@Injectable()
export class TeamCalendarApiResolverService extends BaseBackendService {
  public teamCalendarApiService: BehaviorSubject<ITeamCalendarApiResolverService> = new BehaviorSubject(null);
  private teamCalendarApi: Dictionary<ITeamCalendarApiResolverService> = {};

  constructor(
    protected commonFacadeService: CommonFacadeService,
    teamCalendarAOApiService: TeamCalendarAOApiService,
    teamCalendarLearnerApiService: TeamCalendarLearnerApiService
  ) {
    super(commonFacadeService);

    this.teamCalendarApi[TeamCalendarViewMode.AOView] = teamCalendarAOApiService;
    this.teamCalendarApi[TeamCalendarViewMode.LearnerView] = teamCalendarLearnerApiService;
  }

  public initViewMode(teamCalendarViewMode: TeamCalendarViewMode, ...parameters: unknown[]): void {
    this.teamCalendarApi[teamCalendarViewMode].initParams(parameters);
    this.teamCalendarApiService.next(this.teamCalendarApi[teamCalendarViewMode]);
  }
}
