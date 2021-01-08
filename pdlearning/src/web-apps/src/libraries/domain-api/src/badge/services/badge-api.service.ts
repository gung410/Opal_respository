import { Badge, BadgeId, BadgeWithCriteria, BaseBadgeCriteria, IBadge, IBadgeWithCriteria } from './../models/badge.model';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { ISaveActiveContributorBadgeCriteriaRequest } from './../dtos/save-active-contributor-badge-criteria-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
@Injectable()
export class BadgeApiService extends BaseBackendService {
  private badgeUrlDictionary: Dictionary<string> = {
    [BadgeId.ActiveContributor]: 'activeContributorBadge'
  };
  protected get apiUrl(): string {
    return AppGlobal.environment.badgeApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAllBadges(showSpinner?: boolean): Promise<Badge[]> {
    return this.get<IBadge[]>(`/badges/getInfo`, null, showSpinner)
      .pipe(map(data => data.map(x => new Badge(x))))
      .toPromise();
  }

  public getBadgeById<T extends BaseBadgeCriteria>(id: BadgeId, showSpinner?: boolean): Promise<BadgeWithCriteria<T> | null> {
    return this.get<IBadgeWithCriteria<T>>(`/badges/${this.badgeUrlDictionary[id]}`, null, showSpinner)
      .pipe(map(data => new BadgeWithCriteria<T>(data)))
      .toPromise();
  }

  public saveActiveContributorCriteria(request: ISaveActiveContributorBadgeCriteriaRequest): Promise<void> {
    return this.post<ISaveActiveContributorBadgeCriteriaRequest, void>(`/badges/saveActiveContributorCriteria`, request).toPromise();
  }
}
