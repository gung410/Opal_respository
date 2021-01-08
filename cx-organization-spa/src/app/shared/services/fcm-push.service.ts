import 'rxjs/add/operator/take';

import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CleanUpRegistrationPayload } from 'app-models/notification.model';
import { ErrorModel } from 'app/user-accounts/models/http.model';
import * as firebase from 'firebase/app';
import { ToastrService } from 'ngx-toastr';
import { v4 as uuid } from 'uuid';

import { AppConstant } from '../app.constant';
import { NotificationDataService } from './notification-data.service';
import { NotificationService } from './notification.service';

@Injectable()
export class FcmPushService extends NotificationService {
  private messaging: firebase.messaging.Messaging;
  private currentToken: string;
  private supportPushNotification: boolean;
  constructor(
    toastrService: ToastrService,
    private notificationDataService: NotificationDataService
  ) {
    super(toastrService);
    this.supportPushNotification = firebase.messaging.isSupported();
    if (this.supportPushNotification) {
      this.messaging = firebase.messaging();
      this.messaging.usePublicVapidKey(AppConstant.firebase.publicVapidKey);
    }
  }

  initGettingNotificationFlow(userId: string): Promise<any> {
    if (!this.supportPushNotification) {
      // TODO: handle unsupport firebase messaging here
      return Promise.reject('Browser not support firebase messaging');
    }

    return Notification.requestPermission()
      .then((permission: string) => {
        if (permission === 'denied') {
          this.toastrService.warning(
            'Messageing: Notifications have been blocked. Can not get permission from this browser.',
            'Alert'
          );
        }

        return this.messaging.getToken();
      })
      .then((token: string) => {
        if (!token) {
          this.toastrService.error(
            'Returned token is null',
            'Get token from firebase failed'
          );

          return new Promise((resolve, reject) => {
            reject();
          });
        }
        this.currentToken = token;

        return new Promise((resolve, reject) => {
          this.notificationDataService
            .register({
              clientId: AppConstant.clientId,
              userId,
              instanceIdToken: token
            })
            .subscribe(
              (success) => {
                console.log('Registered token successfully', token);
                resolve();
                navigator.serviceWorker.addEventListener('message', (event) => {
                  this.currentMessage.next(event.data);
                });
              },
              (error: HttpErrorResponse) => {
                reject();
                const errorCode = 500;
                if (error.status >= errorCode) {
                  this.toastrService.error(
                    'Server has some prolem while getting notification',
                    'Registered receiving notification failed'
                  );

                  return;
                }
                this.toastrService.error(
                  (error.error as ErrorModel).Message,
                  'Registered receiving notification failed'
                );
              }
            );
        });
      });
  }

  unsubscribeUserFromNotificationSource(userExternalId: string): Promise<any> {
    if (!this.supportPushNotification) {
      // TODO: handle unsupport firebase messaging here
      return Promise.reject('Browser not support firebase messaging');
    }

    return this.messaging.deleteToken(this.currentToken).then(() => {
      const cleanUpRegistrationData = this.buildCleanUpRegistrationModel(
        userExternalId,
        this.currentToken
      );

      return new Promise((res, rej) => {
        this.notificationDataService
          .cleanUpRegistrationData(cleanUpRegistrationData)
          .take(1)
          .subscribe(
            (success) => {
              res();
            },
            (err) => {
              rej();
            }
          );
      });
    });
  }

  private buildCleanUpRegistrationModel(
    userExternalId: string,
    currentToken: string
  ): CleanUpRegistrationPayload {
    const cleanUpRegistrationData = new CleanUpRegistrationPayload();
    cleanUpRegistrationData.type = 'event';
    cleanUpRegistrationData.version = '1.0';
    cleanUpRegistrationData.payload = {
      identity: {
        clientId: AppConstant.clientId,
        customerId: AppConstant.customerId.toString(),
        userId: userExternalId
      },
      body: {
        registrationToken: currentToken,
        userId: userExternalId
      },
      references: {
        correlationId: uuid()
      }
    };
    cleanUpRegistrationData.routing = {
      action: `${AppConstant.clientId}.communication.logout.firebase.requested`,
      actionVersion: '1.0',
      entity: `${AppConstant.clientId}.employee.user`,
      entityId: userExternalId
    };

    return cleanUpRegistrationData;
  }
}
