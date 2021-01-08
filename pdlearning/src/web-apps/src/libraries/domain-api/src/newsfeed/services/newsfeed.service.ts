import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { INewsfeedResult, NewsfeedResult } from '../dtos/newsfeed-result.dto';

import { IGetNewsfeedRequest } from '../dtos/get-newsfeed-request.dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class NewsfeedApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.newsfeedApiUrl + '/newsfeed';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getNewsfeeds(request: IGetNewsfeedRequest, showSpinner: boolean = true): Promise<NewsfeedResult> {
    return this.post<IGetNewsfeedRequest, INewsfeedResult>(``, request, showSpinner)
      .pipe(map(_ => new NewsfeedResult(_)))
      .toPromise();
  }
}
