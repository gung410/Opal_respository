import { ITeamCalendarApiResolverService, TeamCalendarApiResolverService } from '../services/team-calendar-api-resolver.service';
import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { CalendarRepositoryContext } from '../calendar-repository-context';
import { ISharedTeamModel } from '../models/shared-team-model';
import { ITeamMemberModel } from '../models/team-member-model';
import { Injectable } from '@angular/core';
import { TeamCalendarLearnerApiService } from '../services/team-calendar-learner-api.service';

@Injectable()
export class TeamCalendarRepository extends BaseRepository<CalendarRepositoryContext> {
  private teamCalendarApiService: ITeamCalendarApiResolverService;

  constructor(
    context: CalendarRepositoryContext,
    private teamCalendarApiResolverService: TeamCalendarApiResolverService,
    private teamCalendarLearnerApiService: TeamCalendarLearnerApiService
  ) {
    super(context);
    this.teamCalendarApiResolverService.teamCalendarApiService.subscribe(service => {
      this.teamCalendarApiService = service;
    });
  }

  public getTeamMembers(): Observable<ITeamMemberModel[]> {
    return this.processUpsertData(
      this.context.teamMembersSubject,
      () => from(this.teamCalendarApiService.getTeamMembers(true)),
      'getTeamMembers',
      null,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.learnerId]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.learnerId
    );
  }

  public getMySharedTeams(): Observable<ISharedTeamModel[]> {
    return this.processUpsertData(
      this.context.mySharedTeams,
      () => from(this.teamCalendarLearnerApiService.getMySharedTeams(true)),
      'getMySharedTeams',
      null,
      'loadOnce',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.accessShareId]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => {
        apiResult.map(x => (x.ownerFullName = `${x.ownerFullName}'s shared calendar`));
        return apiResult;
      },
      x => x.accessShareId
    );
  }
}
