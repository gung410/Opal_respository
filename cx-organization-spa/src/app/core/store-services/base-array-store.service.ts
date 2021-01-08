import { BehaviorSubject } from 'rxjs';

import { BaseStoreService } from './base-store.service';

export abstract class BaseArrayStoreService<
  A extends any[]
> extends BaseStoreService<A> {
  protected storeSubject: BehaviorSubject<A> = new BehaviorSubject(null);
  abstract get(): BehaviorSubject<A>;
  abstract add(element: any): BehaviorSubject<A>;
  abstract edit(element: any): BehaviorSubject<A>;
  abstract delete(element: any): BehaviorSubject<A>;

  protected checkForCurrentData(): A {
    const currentValue = this.storeSubject.getValue();
    if (currentValue) {
      this.storeSubject.next(currentValue);
    }

    return currentValue;
  }

  protected addElementToTheBeginning(element: any): void {
    const currentValue = this.storeSubject.getValue();
    currentValue.unshift(element);
    this.storeSubject.next(currentValue);
  }
}
