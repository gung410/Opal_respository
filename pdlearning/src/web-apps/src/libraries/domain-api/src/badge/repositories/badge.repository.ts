import { Badge, BadgeId, BadgeWithCriteria, BaseBadgeCriteria } from '../models/badge.model';
import { Observable, from } from 'rxjs';

import { ActiveContributorsBadgeCriteria } from '../models/active-contributor-badge-criteria.model';
import { BadgeApiService } from '../services/badge-api.service';
import { BadgeRepositoryContext } from '../badge-repository-context';
import { BaseRepository } from '@opal20/infrastructure';
import { ISaveActiveContributorBadgeCriteriaRequest } from '../dtos/save-active-contributor-badge-criteria-request';
import { Injectable } from '@angular/core';

@Injectable()
export class BadgeRepository extends BaseRepository<BadgeRepositoryContext> {
  constructor(context: BadgeRepositoryContext, private apiSvc: BadgeApiService) {
    super(context);
  }

  public getAllBadges(showSpinner: boolean = true): Observable<Badge[]> {
    return this.processUpsertData(
      this.context.badgeSubject,
      implicitLoad => from(this.apiSvc.getAllBadges(!implicitLoad && showSpinner)),
      'getAllBadges',
      [],
      'implicitReload',
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.id]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.id,
      true
    );
  }

  public getBadgeById<T extends BaseBadgeCriteria>(id: BadgeId): Observable<BadgeWithCriteria<T>> {
    return this.processUpsertData(
      this.context.badgeSubject,
      implicitLoad => from(this.apiSvc.getBadgeById<T>(id, !implicitLoad)),
      'getBadgeById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id] as BadgeWithCriteria<T>;
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true
    );
  }

  public saveActiveContributorCriteria(request: ISaveActiveContributorBadgeCriteriaRequest): Observable<void> {
    return from(
      this.apiSvc.saveActiveContributorCriteria(request).then(_ => {
        this.upsertData(
          this.context.badgeSubject,
          [<Partial<BadgeWithCriteria<ActiveContributorsBadgeCriteria>>>{ id: BadgeId.ActiveContributor, criteria: request.criteria }],
          item => item.id
        );
        return _;
      })
    );
  }
}
