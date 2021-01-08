import { BehaviorSubject, Observable, SchedulerLike, empty, timer } from 'rxjs';
import { filter, map, merge, mergeMap, scan, share, switchMap, take, takeUntil } from 'rxjs/operators';

// @dynamic
export class SignalUtils {
  /**
   * The remaining turn on call that hasn't been turned off.
   */
  public static getOnStack(on$: Observable<string>, off$: Observable<string>, forceOff$: Observable<string> = empty()): Observable<number> {
    return on$.pipe(
      merge(forceOff$),
      merge(off$),
      scan<string, number>((onStack, cur) => {
        return onStack > 0 && cur === 'forceOff' ? 0 : Math.max(onStack, 0) + (cur === 'on' ? 1 : cur === 'off' ? -1 : 0);
      }, 0),
      share()
    );
  }

  /**
   * Emit 1 when turn off to on.
   */
  public static getOffToOn(onStack$: Observable<number>): Observable<number> {
    return onStack$.pipe(
      scan<number, number[]>((acc, cur) => [acc[1], cur], [undefined, undefined]),
      filter(([pre, cur]) => cur === 1 && (!pre || pre <= 0)),
      map(([pre, cur]) => cur),
      share()
    );
  }

  /**
   * The block to prevent turning off right after turning on.
   *
   * Emit 1 when block is activated, 0 when block is lifted.
   */
  public static getMinTimeBlock(offToOnObservable: Observable<number>, minOnTime: number, scheduler?: SchedulerLike): Observable<number> {
    return offToOnObservable.pipe(
      switchMap(() => timer(minOnTime, scheduler)),
      merge(offToOnObservable),
      share()
    );
  }

  public static getOffSignal(onStack$: Observable<number>, minTimeBlock$: BehaviorSubject<number>): Observable<string> {
    return onStack$.pipe(
      filter(v => v === 0),
      map(() => 'off'),
      mergeMap(signal =>
        // Since minTimeBlockSubject is BehaviorSubject.
        // Turn off right away when block is currently inactive.
        // Otherwise wait until block is lifted.
        minTimeBlock$.pipe(
          filter(b => b === 0),
          // Cancel waiting when turn on again
          takeUntil(onStack$.pipe(filter(v => v > 0))),
          take(1),
          map(() => 'off')
        )
      ),
      share()
    );
  }

  public static getForceOff(
    on$: Observable<string>,
    offSignal$: Observable<string>,
    maxOnTime: number,
    scheduler?: SchedulerLike
  ): Observable<string> {
    return on$.pipe(
      switchMap(() =>
        timer(maxOnTime, scheduler).pipe(
          // Cancel when turn off.
          takeUntil(offSignal$),
          map(() => 'forceOff')
        )
      )
    );
  }
}
