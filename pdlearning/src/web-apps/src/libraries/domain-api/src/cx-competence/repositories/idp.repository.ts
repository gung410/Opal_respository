import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { IIdpPlanPdosRequest } from '../dtos/idp-plans-pdos-request.dto';
import { IRecommendationByOrganisationItemResult } from '../dtos/recommendation-by-organisation-dto';
import { IdpPlanPdos } from '../dtos/idp-plans-pdos-response.dto';
import { IdpRepositoryContext } from '../cx-competence-repository-context';
import { IndividualDevelopmentPlanApiService } from '../services/idp-backend.service';
import { Injectable } from '@angular/core';

@Injectable()
export class IdpRepository extends BaseRepository<IdpRepositoryContext> {
  constructor(context: IdpRepositoryContext, private idpService: IndividualDevelopmentPlanApiService) {
    super(context);
  }

  public loadPlanPdos(request: IIdpPlanPdosRequest): Observable<IdpPlanPdos> {
    return this.processUpsertData(
      this.context.actionItemResponseSubject,
      implicitLoad => from(this.idpService.getPlanPdos(request, !implicitLoad)),
      'loadPlanPdos',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        if (apiResult.items) {
          apiResult.items = apiResult.items.map(a => repoData[a.objectiveInfo.identity.extId]);
        }
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.objectiveInfo.identity.extId
    );
  }

  public loadListRecommendationsByOU(pageIndex: number, pageSize: number): Observable<IRecommendationByOrganisationItemResult> {
    return this.processUpsertData(
      this.context.actionItemResultSubject,
      implicitLoad => from(this.idpService.getListRecommendationsByOU(pageIndex, pageSize, !implicitLoad)),
      'loadListRecommendationsByOU',
      [pageIndex, pageSize],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.resultIdentity.extId]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.resultIdentity.extId
    );
  }
}
