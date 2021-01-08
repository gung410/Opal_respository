import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { Injectable } from '@angular/core';
import { JoinWebinarUrlResult } from '../models/join-webinar-url-result.model';
import { map } from 'rxjs/operators';

@Injectable()
export class WebinarApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.webinarApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getJoinURL(id: string, source: string, isShowSpinner: boolean = true): Promise<JoinWebinarUrlResult> {
    return this.get<JoinWebinarUrlResult>(`/webinar/sessions/${source}/${id}/joinUrl`, null, isShowSpinner)
      .pipe(map(data => data))
      .toPromise();
  }
}
