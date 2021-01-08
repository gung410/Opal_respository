import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

import { DateUtils } from '@opal20/infrastructure';

@Component({
  selector: 'app-countdown-timer',
  templateUrl: './countdown-timer.component.html'
})
export class CountdownTimerComponent implements OnInit, OnDestroy {
  @Input() public start: Date | undefined;
  @Input() public end: Date | undefined;
  @Input() public label: string | undefined;
  @Input() public duration: number | undefined;

  @Output() public zeroTrigger: EventEmitter<void> = new EventEmitter();

  public displayTime: string = '00:00';
  public lastMilisecondDiff: number | undefined;

  private timer: number;

  constructor(private el: ElementRef) {
    this.zeroTrigger = new EventEmitter(true);
  }

  public ngOnInit(): void {
    this.updateCountdownTimeInfo(this.lastMilisecondDiff);
    this.timer = (setInterval(() => {
      const milisecDiff = this.updateCountdownTimeInfo(this.lastMilisecondDiff - 1000);
      if (milisecDiff <= 0) {
        this.zeroTrigger.emit();
        clearInterval(this.timer);
      }
    }, 1000) as unknown) as number;
  }

  public updateCountdownTimeInfo(milisecDiff?: number): number {
    if (milisecDiff == null) {
      if (this.duration != null) {
        milisecDiff = this.duration;
        this.displayTime = this.getDisplayTime(milisecDiff);
      } else if (this.start !== undefined) {
        milisecDiff = this.getMilisecDiff(this.start, true);
        this.displayTime = this.getDisplayTime(milisecDiff);
      } else if (this.end !== undefined) {
        milisecDiff = this.getMilisecDiff(this.end);
        this.displayTime = this.getDisplayTime(milisecDiff);
      } else {
        throw new Error('Either start or end must be defined');
      }
    } else {
      this.displayTime = this.getDisplayTime(milisecDiff);
    }
    this.lastMilisecondDiff = milisecDiff;

    return milisecDiff;
  }

  public ngOnDestroy(): void {
    this.stopTimer();
  }

  private getMilisecDiff(targetDate: Date, useAsTimer: boolean = false): number {
    const targetDateInMiliseconds = new Date(targetDate).getTime();
    const nowInMiliseconds = new Date().getTime();
    let milisecDiff = targetDateInMiliseconds - nowInMiliseconds;
    if (useAsTimer) {
      milisecDiff = nowInMiliseconds - targetDateInMiliseconds;
    }

    return milisecDiff;
  }

  private getDisplayTime(milisecDiff: number): string {
    const durationInfo = DateUtils.calcDurationInfo(milisecDiff);

    const dayString = durationInfo.days > 0 ? this.twoDigit(durationInfo.days) + ':' : '';
    const hourString = durationInfo.hours > 0 ? this.twoDigit(durationInfo.hours) + ':' : '';

    return dayString + hourString + this.twoDigit(durationInfo.minutes) + ':' + this.twoDigit(durationInfo.seconds);
  }

  private twoDigit(value: number): string {
    return value > 9 ? '' + value : '0' + value;
  }

  private stopTimer(): void {
    clearInterval(this.timer);
    this.timer = undefined;
  }
}
