import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export abstract class NotificationService {
  currentMessage: BehaviorSubject<any> = new BehaviorSubject(null);
  constructor(protected toastrService: ToastrService) {}
  abstract initGettingNotificationFlow(userId: string): Promise<any>;
  abstract unsubscribeUserFromNotificationSource(
    userExternalId?: string
  ): Promise<any>;
}
