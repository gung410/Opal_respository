import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CommunityModel, ICommunityModel } from '../models/community';
import { CommunityTreeviewItem, ICommunityTreeviewItem } from '../models/community/communityTreeviewItem';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CommunityApiService extends BaseBackendService {
  private readonly communityRoute: string = '/communities';

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public getCommunityHierarchyOfCurrentUser(showSpinner?: boolean): Observable<ICommunityTreeviewItem[]> {
    return this.get<ICommunityTreeviewItem[]>(`${this.communityRoute}/hierarchy`, null, showSpinner).pipe(
      map(_ => _.map(h => new CommunityTreeviewItem(h)))
    );
  }

  public getOwnCommunities(showSpinner: boolean = true): Observable<ICommunityModel[]> {
    return this.get<ICommunityModel[]>(`${this.communityRoute}/own-communities`, null, showSpinner).pipe(
      map(_ => _.map(e => new CommunityModel(e)))
    );
  }
}
