import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { HttpHelpers } from 'app-utilities/http-helpers';
import * as firebase from 'firebase';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';

import { FcmPushService } from './fcm-push.service';
import { NotificationDataService } from './notification-data.service';

describe('FcmPushService', () => {
  let fcmPushService: FcmPushService;
  beforeAll(() => {
    firebase.initializeApp({ messagingSenderId: 'dummy' });
  });
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        HttpHelpers,
        NotificationDataService,
        {
          provide: ToastrService,
          useValue: {
            error: (param?, error?) => {
              return;
            },
            warning: (param?) => {
              return;
            }
          }
        },
        FcmPushService
      ]
    });
  });

  it('should be created', () => {
    fcmPushService = TestBed.get(FcmPushService);
    expect(fcmPushService).toBeTruthy();
  });
  describe('initGettingNotificationFlow', () => {
    it('should set up message listener', () => {
      const requestNotificationSpy = spyOn(
        Notification,
        'requestPermission'
      ).and.returnValue(Promise.resolve());
      const getTokenSpy = spyOn(
        (fcmPushService as any).messaging,
        'getToken'
      ).and.returnValue(Promise.resolve('dummyToken'));
      const registerTokenSpy = spyOn(
        (fcmPushService as any).notificationDataService,
        'register'
      ).and.returnValue(of('success'));
      const setUpMessagingEventListenerSpy = spyOn(
        navigator.serviceWorker,
        'addEventListener'
      );
      fcmPushService.initGettingNotificationFlow('dummy');
      requestNotificationSpy.calls.mostRecent().returnValue.then(() => {
        expect(getTokenSpy).toHaveBeenCalled();
        getTokenSpy.calls.mostRecent().returnValue.then(() => {
          expect(registerTokenSpy).toHaveBeenCalled();
          expect(setUpMessagingEventListenerSpy).toHaveBeenCalled();
        });
      });
    });

    it('should show warning message if request permission failed', () => {
      const requestNotificationSpy = spyOn(
        Notification,
        'requestPermission'
      ).and.returnValue(Promise.reject());
      const toastrServiceWarningSpy = spyOn(
        (fcmPushService as any).toastrService,
        'warning'
      );

      fcmPushService.initGettingNotificationFlow('dummy');
      requestNotificationSpy.calls.mostRecent().returnValue.then(() => {
        expect(toastrServiceWarningSpy).toHaveBeenCalled();
      });
    });

    it('should show error message if register token failed', () => {
      const requestNotificationSpy = spyOn(
        Notification,
        'requestPermission'
      ).and.returnValue(Promise.resolve());

      const getTokenSpy = spyOn(
        (fcmPushService as any).messaging,
        'getToken'
      ).and.returnValue(Promise.resolve('dummyToken'));
      spyOn(
        (fcmPushService as any).notificationDataService,
        'register'
      ).and.returnValue(throwError({ error: 'Error' }));
      const toastrServiceErrorSpy = spyOn(
        (fcmPushService as any).toastrService,
        'error'
      );
      fcmPushService.initGettingNotificationFlow('dummy');
      requestNotificationSpy.calls.mostRecent().returnValue.then(() => {
        getTokenSpy.calls.mostRecent().returnValue.then(() => {
          expect(toastrServiceErrorSpy).toHaveBeenCalled();
        });
      });
    });
  });
});
