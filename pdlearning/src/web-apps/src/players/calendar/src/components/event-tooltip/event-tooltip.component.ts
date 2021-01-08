import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
import { BookingSource, EventSource, WebinarApiService } from '@opal20/domain-api';

import { DateUtils } from '@opal20/infrastructure';
@Component({
  selector: 'event-tooltip',
  templateUrl: './event-tooltip.component.html'
})
export class EventTooltipComponent implements AfterViewInit {
  @Input() public eventElement: ElementRef;

  public eventId: string = '';
  public title: string = '';
  public description: string = '';
  public startTime: Date = new Date();
  public endTime: Date = new Date();
  public isAllDay: boolean = false;
  public eventSourceEnum: typeof EventSource = EventSource;
  public eventSource: EventSource = EventSource.Community;
  public communityTitle: string = '';
  public isSameDate: boolean = false;

  constructor(private webinarApiService: WebinarApiService) {}

  public ngAfterViewInit(): void {
    this.title = this.eventElement.nativeElement.getAttribute('event-title');
    this.description = this.eventElement.nativeElement.getAttribute('event-description');
    this.startTime = new Date(this.eventElement.nativeElement.getAttribute('event-start'));
    this.endTime = new Date(this.eventElement.nativeElement.getAttribute('event-end'));
    this.isAllDay = JSON.parse(this.eventElement.nativeElement.getAttribute('event-isAllDay'));
    this.eventSource = this.eventElement.nativeElement.getAttribute('event-source') as EventSource;
    this.communityTitle = this.eventElement.nativeElement.getAttribute('community-title');
    this.eventId = this.eventElement.nativeElement.getAttribute('event-id');

    this.isSameDate = this.checkSameDate(this.startTime, this.endTime);
  }

  public checkEventWebinarIsLive(): boolean {
    const now = new Date();
    return DateUtils.isDateInRange(this.startTime, this.endTime, now) && this.eventSource === this.eventSourceEnum.Webinar;
  }

  public onJoinWebinarClick(): void {
    this.webinarApiService.getJoinURL(this.eventId, BookingSource.Community).then(result => {
      if (result.isSuccess) {
        window.open(result.joinUrl);
      }
    });
  }

  public get isAbleToJoinMeeting(): boolean {
    const now = new Date();
    let diffTimes = (now.getTime() - this.startTime.getTime()) / 1000;
    diffTimes /= 60;
    const diffMinutes = Math.abs(Math.round(diffTimes));
    if ((diffMinutes <= 30 || this.startTime <= now) && now <= this.endTime) {
      return true;
    }
    return false;
  }

  private checkSameDate(startTime: Date, endTime: Date): boolean {
    return DateUtils.compareOnlyDay(new Date(startTime), new Date(endTime)) === 0;
  }
}
