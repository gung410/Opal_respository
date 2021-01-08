import { Component, HostListener } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
@Component({
  selector: 'broadcast-message-notification',
  templateUrl: './broadcast-message-notification.component.html'
})
export class BroadcastMessageNotificationComponent {
  public notificationAlertUrl: SafeResourceUrl;
  public cssClass: string;
  public iframeHeight: string = 'auto';
  public enableBroadcastMessage: boolean = AppGlobal.environment.enableBroadcastMessage;
  public isShowBroadcastMessageBanner: boolean = true;

  constructor(private domSanitizer: DomSanitizer) {}

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    const message = event.data.paramsAlert;

    // It is not our messages
    if (!message) {
      return;
    }

    if (message && !message.height) {
      this.isShowBroadcastMessageBanner = false;
      this.cssClass = 'empty';
      return;
    }

    this.iframeHeight = message.height;
  }

  protected ngAfterViewInit(): void {
    this.notificationAlertUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(AppGlobal.environment.broadcastMessageNotificationUrl);
  }
}
