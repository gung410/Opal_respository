import { IUserTrackingEventRequest, UserTrackingAPIService } from '@opal20/domain-api';

import { EventTrackParam } from './user-tracking.models';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

@Injectable()
export class TrackingSourceService {
  public eventTrack: ReplaySubject<EventTrackParam> = new ReplaySubject<EventTrackParam>(0);

  constructor(private userTrackingAPIService: UserTrackingAPIService) {
    this.setup();
  }

  private setup(): void {
    this.eventTrack.subscribe(evt => {
      const windowContext: IWindow = <IWindow>(<unknown>window);

      const event: IUserTrackingEventRequest = {
        sessionId: windowContext.AppGlobal.sessionUUID,
        time: new Date(),
        userId: windowContext.AppGlobal.user.extId,
        ...evt
      };

      this.userTrackingAPIService.sendEvent(event);
    });
  }
}
