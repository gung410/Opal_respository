import {
  interval,
  of,
  Subscription,
  Observable,
  pipe,
  UnaryFunction
} from 'rxjs';
import { delay, takeUntil } from 'rxjs/operators';

// @dynamic
export class CxProcessUtil {
  public static minDefaultDelay = 10;

  public static debounce(func: (...args: any[]) => any, wait: number) {
    if (wait <= 0) {
      return func;
    }

    let timeout: any;
    return (...args: any[]) => {
      const context = this;

      const executeFunction = () => {
        func.apply(context, args);
      };

      clearTimeout(timeout);
      timeout = setTimeout(executeFunction, wait);
    };
  }

  public static delay(
    callback: (...args: any[]) => void,
    immediateOrTime?: number | boolean,
    cancelOn$?: Observable<any>,
    ...args: any[]
  ): Subscription {
    if (
      immediateOrTime === true ||
      (typeof immediateOrTime === 'number' && immediateOrTime === 0)
    ) {
      callback(args);
      return new Subscription();
    } else {
      const delayObs = pipe(
        cancelOn$ !== undefined
          ? takeUntil(cancelOn$)
          : (obs: Observable<any>) => obs,
        delay(
          immediateOrTime === undefined || immediateOrTime === false
            ? CxProcessUtil.minDefaultDelay
            : immediateOrTime
        )
      );
      return delayObs(of(undefined)).subscribe(() => callback(args));
    }
  }

  public static interval(
    callback: (...args: any[]) => void,
    ms: number,
    ...args: any[]
  ): Subscription {
    return interval(ms).subscribe(() => {
      callback(args);
    });
  }
}
