import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { IUserSharing, UserSharing } from '../models/user-sharing-model';

import { IGetUserSharingRequest } from '../dtos/user-sharing-request.dto';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { ISaveUserSharing } from '../dtos/save-user-sharing-request-dto';
import { Injectable } from '@angular/core';
import { LearnerLearningPath } from '../models/my-learning-path.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UserSharingAPIService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/usersharing';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public createUserSharing(request: ISaveUserSharing): Promise<UserSharing> {
    return this.post<ISaveUserSharing, IUserSharing>('', request)
      .pipe(map(result => new UserSharing(result)))
      .toPromise();
  }

  public updateUserSharing(request: ISaveUserSharing): Promise<UserSharing> {
    return this.put<ISaveUserSharing, IUserSharing>('', request)
      .pipe(map(result => new UserSharing(result)))
      .toPromise();
  }

  public getUserSharing(request: IGetUserSharingRequest): Promise<IPagedResultDto<UserSharing>> {
    const queryParams: IGetParams = {
      searchText: request.searchText,
      itemType: request.itemType,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };

    return this.get<IPagedResultDto<UserSharing>>('/search', queryParams).toPromise();
  }

  public getUserSharingByItemId(itemId: string): Observable<UserSharing> {
    return this.get<IUserSharing>(`/details/byitemid/${itemId}`).pipe(map(_ => new UserSharing(_)));
  }

  public getSharedLearningPathToMe(request: IGetUserSharingRequest): Promise<IPagedResultDto<LearnerLearningPath>> {
    const queryParams: IGetParams = {
      searchText: request.searchText,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };

    return this.get<IPagedResultDto<LearnerLearningPath>>('/shared/me/learningpath', queryParams).toPromise();
  }

  public getUserSharingByItemIds(itemIds: string[]): Promise<UserSharing[]> {
    return this.post<string[], UserSharing[]>(`/search/byItemIds`, itemIds).toPromise();
  }

  public deleteUserSharingById(id: string): Promise<IUserSharing> {
    return this.delete<IUserSharing>(`/${id}`).toPromise();
  }
}
