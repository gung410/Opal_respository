import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { ITrackingModel, TrackingModel } from '../models/tracking.model';
import { ITrackingRequest, IUserTrackingEventRequest } from '../dtos/user-tracking.dto';

import { IPagedResultDto } from './../../share/dtos/paged-result.dto';
import { IPagedResultRequestDto } from './../../share/dtos/paged-request.dto';
import { Injectable } from '@angular/core';
import { TrackingSharedDetailByModel } from '../models/tracking-shared-detail-to.model';
import { map } from 'rxjs/operators';

@Injectable()
export class UserTrackingAPIService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/userTracking';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public sendEvent(event: IUserTrackingEventRequest): Promise<void> {
    return this.post<IUserTrackingEventRequest, void>('', event, false).toPromise();
  }

  public getTrackingInfoByItemId(request: ITrackingRequest): Promise<TrackingModel> {
    return this.post<ITrackingRequest, ITrackingModel>(`/trackingInfo/byItemId`, request)
      .pipe(map(_ => new TrackingModel(_)))
      .toPromise();
  }

  public likeEvent(request: ITrackingRequest): Promise<TrackingModel> {
    return this.post<ITrackingRequest, ITrackingModel>(`/like`, request)
      .pipe(map(_ => new TrackingModel(_)))
      .toPromise();
  }

  public shareEvent(request: ITrackingRequest): Promise<TrackingModel> {
    return this.post<ITrackingRequest, ITrackingModel>(`/share`, request)
      .pipe(map(_ => new TrackingModel(_)))
      .toPromise();
  }

  public getSharedTo(request: IPagedResultRequestDto, showSpinner: boolean = true): Promise<IPagedResultDto<TrackingSharedDetailByModel>> {
    const queryParams: IGetParams = {
      skipCount: request.skipCount,
      maxResultCount: request.maxResultCount
    };
    return this.get<IPagedResultDto<TrackingSharedDetailByModel>>(`/share/get`, queryParams, showSpinner)
      .pipe(
        map(p => {
          const result: IPagedResultDto<TrackingSharedDetailByModel> = {
            items: p.items.map(_ => new TrackingSharedDetailByModel(_)),
            totalCount: p.totalCount
          };
          return result;
        })
      )
      .toPromise();
  }
}
