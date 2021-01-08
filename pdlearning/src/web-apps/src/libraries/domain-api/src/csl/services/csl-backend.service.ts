import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { CommunityResultModel, ICommunityRequest } from '../models/csl-community.model';

import { CSLCommunityResults } from '../dtos/csl-community-list-result.model';
import { CommunityMemberShip } from '../models/csl-community-membership.model';
import { ICslPagedResultDto } from '../../share/dtos/paged-result.dto';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

function calculatePageNum(skipCount: number, maxResultCount: number): number {
  return Math.max(1, Math.ceil((skipCount + 1) / maxResultCount));
}

@Injectable()
export class CollaborativeSocialLearningApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.cslApiUrl + '/v1';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAllItemsCommunity(userId: string, request: ICommunityRequest, showSpinner: boolean = true): Promise<CSLCommunityResults> {
    let urlExtention: string = '';
    const queryParam: IGetParams = {
      page: calculatePageNum(request.skipCount, request.maxResultCount),
      limit: request.maxResultCount
    };
    const filterType = request.filterType;
    switch (filterType) {
      case 'Communities':
        urlExtention = '/user/space/joined/';
        break;
      case 'OwnCommunities':
        urlExtention = '/user/space/created/';
        break;
      default:
        urlExtention = '/user/space/';
    }
    return this.get<ICslPagedResultDto<CommunityResultModel>>(`${urlExtention}${userId}`, queryParam, showSpinner)
      .pipe(
        map(result => {
          const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
          return resCslCommunity;
        })
      )
      .toPromise();
  }

  public getCommunityByIds(communityIds: string[], showSpinner: boolean = true): Promise<CSLCommunityResults> {
    const communityBody: object = {
      guids: communityIds
    };
    return this.post<object, ICslPagedResultDto<CommunityResultModel>>('/space/list', communityBody, showSpinner)
      .pipe(
        map(result => {
          const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
          return resCslCommunity;
        })
      )
      .toPromise();
  }

  public getAllCommunities(page: number, limit: number, searchText: string = '', showSpinner?: boolean): Observable<CSLCommunityResults> {
    return this.get<ICslPagedResultDto<CommunityResultModel>>(
      `/space?searchText=${searchText}&page=${page}&limit=${limit}`,
      null,
      showSpinner
    ).pipe(
      map(result => {
        const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
        return resCslCommunity;
      })
    );
  }

  public getCommunitiesByUserId(
    userId: string,
    page: number,
    limit: number,
    searchText: string = '',
    showSpinner?: boolean
  ): Observable<CSLCommunityResults> {
    return this.get<ICslPagedResultDto<CommunityResultModel>>(
      `/user/space/${userId}?searchText=${searchText}&page=${page}&limit=${limit}`,
      null,
      showSpinner
    ).pipe(
      map(result => {
        const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
        return resCslCommunity;
      })
    );
  }

  public getCreatedCommunitiesByUserId(
    userId: string,
    page: number,
    limit: number,
    showSpinner?: boolean
  ): Observable<CSLCommunityResults> {
    return this.get<ICslPagedResultDto<CommunityResultModel>>(
      `/user/space/created/${userId}?page=${page}&limit=${limit}`,
      null,
      showSpinner
    ).pipe(
      map(result => {
        const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
        return resCslCommunity;
      })
    );
  }

  public getJoinedCommunitiesByUserId(userId: string, page: number, limit: number, showSpinner?: boolean): Observable<CSLCommunityResults> {
    return this.get<ICslPagedResultDto<CommunityResultModel>>(`/user/space/${userId}?page=${page}&limit=${limit}`, null, showSpinner).pipe(
      map(result => {
        const resCslCommunity = CSLCommunityResults.createCSLCommunityResults(result);
        return resCslCommunity;
      })
    );
  }

  public getCommunityMembers(
    communityId: string,
    page: number,
    limit: number,
    showSpinner?: boolean
  ): Observable<ICslPagedResultDto<CommunityMemberShip>> {
    return this.get<ICslPagedResultDto<CommunityMemberShip>>(
      `/space/${communityId}/membership?page=${page}&limit=${limit}`,
      null,
      showSpinner
    );
  }
}
