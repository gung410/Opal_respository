import { Injectable } from '@angular/core';
import { NotificationItem } from '@conexus/cx-angular-common';

@Injectable()
export class AppService {
  public setPositionForNewNotification(
    newNotification: NotificationItem,
    currentNotifications: NotificationItem[]
  ) {
    const isNewNotificationExisted =
      currentNotifications.find((noti) => noti.id === newNotification.id) !==
      undefined;
    if (isNewNotificationExisted) {
      return;
    }
    const currentNotificationCachedLength = currentNotifications.length;
    for (let i = 0; i < currentNotificationCachedLength; i++) {
      if (currentNotifications[i].sentDate > newNotification.sentDate) {
        const isLastElement = i === currentNotificationCachedLength - 1;
        if (!isLastElement) {
          continue;
        }
        currentNotifications.push(newNotification);
        continue;
      }
      // Add to before the item have sent date older (or equal) new notification
      for (let j = i + 1; j < currentNotificationCachedLength; j++) {
        currentNotifications[j + 1] = currentNotifications[j];
      }
      currentNotifications[i] = newNotification;
      break;
    }
  }
}
