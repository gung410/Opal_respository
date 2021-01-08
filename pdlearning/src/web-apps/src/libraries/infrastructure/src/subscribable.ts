import { Observable, Subscription } from 'rxjs';

import { OnDestroy } from '@angular/core';
import { take } from 'rxjs/operators';

export class SubscriptionCollection {
  private subscriptionRefs: Subscription[] = [];

  public add<T>(observable: Observable<T>, handler: { (data: T): void }): void {
    const subscription: Subscription = observable.subscribe(data => {
      handler(data);
    });
    this.subscriptionRefs.push(subscription);
  }

  public addItems(subscriptions: Subscription[]): void {
    this.subscriptionRefs.push(...subscriptions);
  }

  public clear(): void {
    this.subscriptionRefs.forEach(s => s.unsubscribe());
  }
}

export abstract class Subscribable implements OnDestroy {
  protected subscriptionCollection: SubscriptionCollection = new SubscriptionCollection();

  public ngOnDestroy(): void {
    this.subscriptionCollection.clear();
    this.onUnsubscribe();
    this.onDestroy();
  }

  protected subscribe<T>(observable: Observable<T>, handler: { (data: T): void }): void {
    this.subscriptionCollection.add(observable, handler);
  }

  protected subscribeOne<T>(observable: Observable<T>, handler: { (data: T): void }): void {
    observable.pipe(take(1)).subscribe(data => handler(data));
  }

  protected onUnsubscribe(): void {
    // Virtual method
  }

  protected onDestroy(): void {
    // Virtual method
  }
}
