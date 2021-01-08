import { Injectable } from '@angular/core';
import {
  CleanUpRegistrationPayload,
  RegisterNotification
} from 'app-models/notification.model';
import { HttpHelpers } from 'app-utilities/http-helpers';

import { NotificationCommand } from 'app-models/communication-command.model';
import { Observable } from 'rxjs';
import { AppConstant } from '../app.constant';

@Injectable()
export class NotificationDataService {
  constructor(private httpHelpers: HttpHelpers) {}
  register(registerModel: RegisterNotification): Observable<unknown> {
    // TODO: need to uncomment this when api ready
    return this.httpHelpers.post(
      `${AppConstant.api.communication}/communication/notification/register`,
      registerModel,
      undefined,
      { avoidIntercepterCatchError: true }
    );
  }

  trackNotificationData(userId: string): Observable<unknown> {
    return this.httpHelpers.post(
      `${AppConstant.api.communication}/communication/notification/track_notification_history`,
      null,
      {
        userId
      },
      { avoidIntercepterCatchError: true }
    );
  }

  readNotification(
    notificationCommand: NotificationCommand
  ): Observable<unknown> {
    return this.httpHelpers.post(
      `${AppConstant.api.communication}/notification/send_notification_event`,
      notificationCommand,
      undefined,
      { avoidIntercepterCatchError: true }
    );
  }

  cleanUpRegistrationData(
    cleanUpRegistrationPayload: CleanUpRegistrationPayload
  ): Observable<unknown> {
    return this.httpHelpers.post(
      `${AppConstant.api.communication}/notification/send_notification_event`,
      cleanUpRegistrationPayload,
      undefined,
      { avoidIntercepterCatchError: true }
    );
  }
}
