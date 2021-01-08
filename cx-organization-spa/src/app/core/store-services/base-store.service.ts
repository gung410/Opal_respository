import { BehaviorSubject, Subject } from 'rxjs';

export abstract class BaseStoreService<T> {
  protected storeSubject: BehaviorSubject<T> = new BehaviorSubject(null);
  // Emit anything, use to clarify the structure: store is just interact with data service and return data for subscriber
  protected subject: Subject<any> = new Subject();
  abstract get(): BehaviorSubject<T>;

  abstract edit(obj: T): void;

  protected checkForCurrentData(): T {
    const currentValue = this.storeSubject.getValue();
    if (currentValue) {
      this.storeSubject.next(currentValue);
    }

    return currentValue;
  }
}
