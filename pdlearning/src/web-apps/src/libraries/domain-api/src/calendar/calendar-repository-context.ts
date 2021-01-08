import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { ISharedTeamModel } from './models/shared-team-model';
import { ITeamMemberModel } from './models/team-member-model';
import { Injectable } from '@angular/core';

@Injectable()
export class CalendarRepositoryContext extends BaseRepositoryContext {
  public teamMembersSubject: BehaviorSubject<Dictionary<ITeamMemberModel>> = new BehaviorSubject({});
  public mySharedTeams: BehaviorSubject<Dictionary<ISharedTeamModel>> = new BehaviorSubject({});
}
