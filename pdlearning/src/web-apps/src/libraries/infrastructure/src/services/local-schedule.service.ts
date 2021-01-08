import { BrowserIdleHandler } from '../specials/browser-idle.handler';
import { Injectable } from '@angular/core';
import { ScheduleService } from './schedule.service';

@Injectable()
export class LocalScheduleService extends ScheduleService {
  constructor(protected browserIdleHandle: BrowserIdleHandler) {
    super(browserIdleHandle);
  }
}
