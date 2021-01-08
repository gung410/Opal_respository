import { TestBed } from '@angular/core/testing';
import { AppService } from './app.service';
import { NotificationItem } from '@conexus/cx-angular-common';

describe('AppService', () => {
  let appService: AppService;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppService],
    });
  });
  it('should be created', () => {
    appService = TestBed.get(AppService);
    expect(appService).toBeTruthy();
  });
  // describe('setPositionForNewNotification', () => {
  //     let mockDescSortedByDateCurrentNotificationItems: NotificationItem[];
  //     beforeEach(() => {
  //         mockDescSortedByDateCurrentNotificationItems = [
  //             {
  //                 id: 1,
  //                 message: 'Dummy message 1',
  //                 senderName: 'T',
  //                 sentDate: new Date(2011, 10, 29)
  //             },
  //             {
  //                 id: 2,
  //                 message: 'Dummy message 2',
  //                 senderName: 'T',
  //                 sentDate: new Date(2011, 10, 20)
  //             },
  //             {
  //                 id: 3,
  //                 message: 'Dummy message 3',
  //                 senderName: 'T',
  //                 sentDate: new Date(2011, 10, 10)
  //             }
  //         ];
  //     });

  //     it('should append to the head of array', () => {
  //         const newNotificationItemId = 4;
  //         appService.setPositionForNewNotification({
  //             id: newNotificationItemId,
  //             message: 'New',
  //             senderName: 'T',
  //             sentDate: new Date(2011, 10, 30)
  //         } as NotificationItem,
  //             mockDescSortedByDateCurrentNotificationItems);
  //         expect(newNotificationItemId).toEqual(mockDescSortedByDateCurrentNotificationItems[0].id);
  //     });

  //     it('should append to the second position in the array (index = 1)', () => {
  //         const newNotificationItemId = 4;
  //         appService.setPositionForNewNotification({
  //             id: newNotificationItemId,
  //             message: 'New',
  //             senderName: 'T',
  //             sentDate: new Date(2011, 10, 25)
  //         } as NotificationItem,
  //             mockDescSortedByDateCurrentNotificationItems);
  //         expect(newNotificationItemId).toEqual(mockDescSortedByDateCurrentNotificationItems[1].id);
  //     });

  //     it('should append to the end of the array', () => {
  //         const newNotificationItemId = 4;
  //         appService.setPositionForNewNotification({
  //             id: newNotificationItemId,
  //             message: 'New',
  //             senderName: 'T',
  //             sentDate: new Date(2011, 10, 9)
  //         } as NotificationItem,
  //             mockDescSortedByDateCurrentNotificationItems);
  //         expect(newNotificationItemId).toEqual(mockDescSortedByDateCurrentNotificationItems[
  //             mockDescSortedByDateCurrentNotificationItems.length - 1].id);
  //     });

  //     it('should not add new notification to the list (because of duplication)', () => {
  //         const newNotificationItemId = 3;
  //         const mockDescSortedByDateCurrentNotificationItemsInitalLength
  //             = mockDescSortedByDateCurrentNotificationItems.length;
  //         appService.setPositionForNewNotification({
  //             id: newNotificationItemId,
  //             message: 'New',
  //             senderName: 'T',
  //             sentDate: new Date(2011, 10, 9)
  //         } as NotificationItem,
  //             mockDescSortedByDateCurrentNotificationItems);
  //         expect(mockDescSortedByDateCurrentNotificationItems.length)
  //             .toEqual(mockDescSortedByDateCurrentNotificationItemsInitalLength);
  //     });
  // });
});
