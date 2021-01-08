import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { AppConstant } from 'app/shared/app.constant';
import { Subscription } from 'rxjs';

@Component({
  selector: 'team-calendar',
  styleUrls: ['./team-calendar.component.scss'],
  templateUrl: './team-calendar.component.html',
})
export class TeamCalendarPageComponent
  implements OnInit, AfterViewInit, OnDestroy {
  height: number = 0;
  calendarBaseUrl: string = AppConstant.moduleLink.calendar;
  @ViewChild('teamCalendarIframe', { static: true })
  teamCalendarIframeRef: ElementRef;
  updateTokenSubscribe: Subscription;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    if (this.teamCalendarIframeRef) {
      this.teamCalendarIframeRef.nativeElement.src = this.teamCalendarUrl;
    }
  }

  ngAfterViewInit(): void {
    this.updateTokenSubscribe = this.authService.accessTokenSubject.subscribe(
      this.updateAccessTokenForIframe
    );
  }

  ngOnDestroy(): void {
    if (this.updateTokenSubscribe) {
      this.updateTokenSubscribe.unsubscribe();
    }
  }

  get teamCalendarUrl(): string {
    const accessToken = this.authService.getAccessToken();

    return `${this.calendarBaseUrl}/team?accessToken=${accessToken}`;
  }

  private updateAccessTokenForIframe(accessToken: string): void {
    if (!this.teamCalendarIframeRef) {
      return;
    }

    const iframeElement = this.teamCalendarIframeRef
      .nativeElement as HTMLIFrameElement;
    const iframeContentWindow = iframeElement.contentWindow as any;

    try {
      iframeContentWindow.AppGlobal.calendarIntergrations.refreshAccessToken(
        accessToken
      );
    } catch (e) {
      console.error('Cannot refresh token for team calendar.');
    }
  }
}
