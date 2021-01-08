import { AsyncSubject, Subject, Subscription, empty, interval } from 'rxjs';
import { Injectable, OnDestroy } from '@angular/core';

import { BrowserIdleHandler } from '../specials/browser-idle.handler';
import { mergeMap } from 'rxjs/operators';

// tslint:disable:no-any

export class ScheduledTask {
  constructor(public id: string, public intervalTimer: number, public action: Function, public isAsync: boolean) {}
}

@Injectable()
export class ScheduleService implements OnDestroy {
  private _scheduledTasks: Map<string, AsyncSubject<any>> = new Map();

  private tasks: Map<string, ScheduledTask> = new Map();
  private taskSubscriptions: Map<string, Subscription> = new Map();

  private onTaskRegistered: Subject<ScheduledTask> = new Subject();

  constructor(protected browserIdleHandle: BrowserIdleHandler) {
    this.onTaskRegistered.subscribe(task => this.trigger(task));
  }

  get scheduledTasks(): Map<string, AsyncSubject<any>> {
    return this._scheduledTasks;
  }

  public ngOnDestroy(): void {
    this._scheduledTasks.forEach(task => task.unsubscribe());
    this.taskSubscriptions.forEach(task => task.unsubscribe());
    this.onTaskRegistered.unsubscribe();
  }

  public getTask(id: string): AsyncSubject<any> {
    return this._scheduledTasks.get(id);
  }

  public register(task: ScheduledTask): AsyncSubject<any> {
    this.tasks.set(task.id, task);
    const asyncSubject = new AsyncSubject<any>();
    this._scheduledTasks.set(task.id, asyncSubject);
    this.onTaskRegistered.next(task);

    return asyncSubject;
  }

  public unregister(id: string): void {
    this.tasks.delete(id);

    this.taskSubscriptions.get(id).unsubscribe();
    this.taskSubscriptions.delete(id);

    this._scheduledTasks.get(id).unsubscribe();
    this._scheduledTasks.delete(id);
  }

  private trigger(task: ScheduledTask): void {
    if (task.isAsync) {
      this.taskSubscriptions.set(
        task.id,
        interval(task.intervalTimer)
          .pipe(
            mergeMap(() => {
              if (!this.browserIdleHandle.isTimedOut()) {
                return task.action();
              } else {
                return empty();
              }
            })
          )
          .subscribe(result => {
            this.notifyResult(task.id, result);
          })
      );
    } else {
      this.taskSubscriptions.set(
        task.id,
        interval(task.intervalTimer).subscribe(() => {
          if (!this.browserIdleHandle.isTimedOut()) {
            const executionResult = task.action();
            this.notifyResult(task.id, executionResult);
          }
        })
      );
    }
  }

  private notifyResult(taskId: string, result: any): void {
    const scheduledTask = this._scheduledTasks.get(taskId);
    if (scheduledTask) {
      scheduledTask.next(result);
    }
  }
}
