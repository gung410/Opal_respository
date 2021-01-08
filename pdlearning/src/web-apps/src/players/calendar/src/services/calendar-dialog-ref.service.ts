import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ICommunityEventDialogAction {
  action: string;
}

@Injectable()
export class CalendarDialogRefService {
  public communityEventDialogRef: Subject<ICommunityEventDialogAction> = new Subject();

  public notifyDialogAction(actionName: string): void {
    this.communityEventDialogRef.next({
      action: actionName
    });
  }
}
