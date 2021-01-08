import { LangUtils } from './lang.utils';

export class IntervalScheduler {
  private _timer;
  private _interval: number;

  constructor(interval: number, callback: Function) {
    this._interval = interval;
    this._callback = callback;
  }

  public init(condition: Function = () => true, triggerCallbackImmediately: boolean = false): void {
    if (triggerCallbackImmediately) {
      this._callback();
    }

    this._timer = (setInterval(() => {
      if (LangUtils.isPromise(condition)) {
        condition.then(result => result === true && this._callback());
      } else if (condition()) {
        this._callback();
      }
    }, this._interval) as unknown) as number;
  }

  public destroy(): void {
    clearInterval(this._timer);
    this._timer = undefined;
  }

  private _callback: Function = () => {
    // tslint:disable-next-line:semicolon
  };
}
