import { DocumentInterruptSource, Idle } from '@ng-idle/core';

import { Injectable } from '@angular/core';
import { ModalService } from '../services/modal.service';

@Injectable()
export class BrowserIdleHandler {
  private timedOut: boolean;

  constructor(private idle: Idle, private modalService: ModalService) {}

  public start(): void {
    // Set idle timeout
    this.idle.setIdle(AppGlobal.environment.idleConfig.idleTimeoutInSecond);

    // Set inactive timeout
    this.idle.setTimeout(AppGlobal.environment.idleConfig.inActiveTimeoutInSecond);

    // Sets the interrupts, in this case, things like clicks, scrolls, touches to the document
    this.idle.setInterrupts([new DocumentInterruptSource(AppGlobal.environment.idleConfig.userEvents)]);

    // After (idleTimeout + inActiveTimeout) of idle and inactive, the system will force logout
    this.idle.onIdleStart.subscribe(() => {
      this.modalService.showWarningMessage('Your session is about to end in 5 minutes. Please click OK to continue your session');
    });
    this.idle.onTimeout.subscribe(() => {
      this.timedOut = true;

      if (AppGlobal.logoutFn) {
        AppGlobal.logoutFn();
      }
    });

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
