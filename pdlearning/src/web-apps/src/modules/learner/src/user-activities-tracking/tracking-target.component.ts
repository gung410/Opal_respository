import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { EventName } from './user-tracking.models';
import { TrackingSourceService } from './tracking-souce.service';

@Component({
  selector: 'tracking-target',
  template: ''
})
export class TrackingTargetComponent implements OnDestroy, OnInit {
  @Input() public pageStartEventName: EventName;
  @Input() public pageStartPayload: any;
  @Input() public pageEndEventName: EventName;
  @Input() public pageEndPayload: any;
  /**
   *
   */
  constructor(protected trackingSourceService: TrackingSourceService) {}

  public ngOnInit(): void {
    this.trackingSourceService.eventTrack.next({
      eventName: this.pageStartEventName,
      payload: this.pageStartPayload
    });
  }

  public ngOnDestroy(): void {
    this.trackingSourceService.eventTrack.next({
      eventName: this.pageEndEventName,
      payload: this.pageEndPayload
    });
  }
}
