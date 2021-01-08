import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class OpalFooterService {
  public isShow: BehaviorSubject<boolean> = new BehaviorSubject(true);

  public show(): void {
    this.isShow.next(true);
  }

  public hide(): void {
    this.isShow.next(false);
  }
}
