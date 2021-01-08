import { Injectable } from '@angular/core';
import { BrowserIdleHandler } from 'app-utilities/browser-idle.handler';
import { ScheduleService } from './schedule.service';

@Injectable({
  // we declare that this service should be created
  // by the root application injector.
  providedIn: 'root',
})
export class LocalScheduleService extends ScheduleService {
  constructor(protected browserIdleHandle: BrowserIdleHandler) {
    super(browserIdleHandle);
  }
}
