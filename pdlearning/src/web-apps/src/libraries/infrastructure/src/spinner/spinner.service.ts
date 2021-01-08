import { BehaviorSubject, Observable, SchedulerLike, Subject } from 'rxjs';

import { Injectable } from '@angular/core';
import { SignalUtils } from './signal.utils';
import { throttle } from 'rxjs/operators';

@Injectable()
export class SpinnerService {
  public onSignal$: Observable<string>;
  public offSignal$: Observable<string>;
  private minOnTime: number = 400;
  private maxOnTime: number = 15000;
  private scheduler?: SchedulerLike;
  /**
   * Turn on requests.
   */
  private on$: Subject<string> = new Subject();

  /**
   * Turn off requests.
   */
  private off$: Subject<string> = new Subject();

  /**
   * Force turn off when this has been on longer than maxOnTime since the last call to turnOn().
   */
  private forceOff$: Subject<string> = new Subject();

  constructor() {
    const onStack$: Observable<number> = SignalUtils.getOnStack(this.on$, this.off$, this.forceOff$);
    const offToOn$: Observable<number> = SignalUtils.getOffToOn(onStack$);

    // Use BehaviorSubject to get previous value.
    const minTimeBlock: BehaviorSubject<number> = new BehaviorSubject(0);
    SignalUtils.getMinTimeBlock(offToOn$, this.minOnTime, this.scheduler).subscribe(minTimeBlock);

    this.offSignal$ = SignalUtils.getOffSignal(onStack$, minTimeBlock);
    this.onSignal$ = this.on$.pipe(throttle(() => this.offSignal$));

    SignalUtils.getForceOff(this.on$, this.offSignal$, this.maxOnTime, this.scheduler).subscribe(this.forceOff$);
  }

  public show(): void {
    this.on$.next('on');
  }

  public hide(force: boolean = false): void {
    if (force) {
      this.forceOff$.next('forceOff');
    } else {
      this.off$.next('off');
    }
  }
}
