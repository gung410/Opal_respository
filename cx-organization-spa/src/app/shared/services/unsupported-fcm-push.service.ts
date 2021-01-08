import 'rxjs/add/operator/take';

import { Injectable } from '@angular/core';
import { environment } from 'app-environments/environment';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { NotificationService } from './notification.service';

@Injectable()
export class UnsupportedFcmPushService extends NotificationService {
  currentMessage: BehaviorSubject<any> = new BehaviorSubject(null);
  constructor(toastrService: ToastrService) {
    super(toastrService);
  }

  initGettingNotificationFlow(userId: string): Promise<unknown> {
    // TODO: implement logic for unsupported later
    if (environment.notification.enableToggleToFireBase) {
      this.toastrService.warning(
        'Currently you cannot receive notification from this page because we have not support yet! Use modern browser to get notification',
        'Notification is not supported',
        {
          timeOut: 5000
        }
      );
    }

    return new Promise((resolve) => {
      resolve();
    });
  }

  unsubscribeUserFromNotificationSource(): Promise<any> {
    return new Promise((resolve) => {
      resolve();
    });
  }
}
