import { Injectable } from '@angular/core';
import { DocumentInterruptSource, Idle } from '@ng-idle/core';
import { IdleConfig } from '../app.constant';

@Injectable()
export class BrowserIdleHandler {
  private timedOut: boolean;

  constructor(private idle: Idle) {}

  public start(): void {
    const idleTimeOut: number = 2000;
    // Set idle timeout
    this.idle.setIdle(idleTimeOut);

    // Set inactive timeout
    this.idle.setTimeout(idleTimeOut);

    // Sets the interrupts, in this case, things like clicks, scrolls, touches to the document
    this.idle.setInterrupts([
      new DocumentInterruptSource(IdleConfig.userEvents),
    ]);

    // After (idleTimeout + inActiveTimeout) of idle and inactive, the system will force logout
    this.idle.onIdleStart.subscribe(() => {});
    // Watching user events
    this.idle.watch();
  }

  public stop(): void {
    this.timedOut = false;
    this.idle.stop();
  }

  public restart(): void {
    this.idle.watch();
    this.timedOut = false;
  }

  public isTimedOut(): boolean {
    return this.timedOut;
  }
}
