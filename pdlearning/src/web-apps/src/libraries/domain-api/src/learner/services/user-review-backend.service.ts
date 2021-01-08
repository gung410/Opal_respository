import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import {
  ICreateUserReviewRequest,
  IPageUserReviewRequest,
  IUpdateUserReviewRequest,
  IUserReviewRequest,
  PagedUserReviewModelResult
} from '../dtos/user-review-backend-service.dto';
import { IUserReviewModel, UserReviewModel } from '../models/user-review.model';

import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class UserReviewApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/reviews';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getMyReviews(request: IUserReviewRequest): Promise<UserReviewModel> {
    const queryParams: IGetParams = {
      itemId: request.itemId,
      itemType: request.itemType
    };
    return this.get<UserReviewModel>('/me', queryParams)
      .pipe(map(response => new UserReviewModel(response)))
      .toPromise();
  }

  public getReviews(request: IPageUserReviewRequest): Promise<PagedUserReviewModelResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
      itemId: request.itemId,
      itemTypeFilter: request.itemTypeFilter,
      orderBy: request.orderBy,
      classRunId: request.classrunId
    };
    return this.get<PagedUserReviewModelResult>('', queryParams)
      .pipe(map(response => new PagedUserReviewModelResult(response.rating, response.totalCount, response.items)))
      .toPromise();
  }

  public createMyReview(request: ICreateUserReviewRequest): Promise<UserReviewModel> {
    return this.post<ICreateUserReviewRequest, IUserReviewModel>('/create', request)
      .pipe(map(result => new UserReviewModel(result)))
      .toPromise();
  }

  public updateMyReview(itemId: string, request: IUpdateUserReviewRequest): Promise<UserReviewModel> {
    return this.put<IUpdateUserReviewRequest, IUserReviewModel>(`/${itemId}`, request)
      .pipe(map(result => new UserReviewModel(result)))
      .toPromise();
  }
}
