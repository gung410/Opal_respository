import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  HostListener,
} from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'report-page',
  styleUrls: ['./report-page.component.scss'],
  templateUrl: './report-page.component.html',
})
export class ReportPageComponent implements OnInit {
  height: number = 0;
  reportBaseUrl: string = AppConstant.moduleLink.report;
  @ViewChild('reportIframe', { static: true }) reportIframeRef: ElementRef;

  constructor() {}

  ngOnInit(): void {
    if (this.reportIframeRef) {
      this.reportIframeRef.nativeElement.src = this.reportUrl;
    }
  }

  get reportUrl(): string {
    // tslint:disable-next-line:max-line-length
    const encryptedPath =
      'oq3z04kz/nqIKIui6cEcxGd/4aCpGt48B5acxXmjJG5MO2oaMIoVJuVsJny9rxiz6KcKDyOk/nqw2iqz7EwahyCylerQMdX4tCw50VF6gov35UT+ValQvV44zfJkALJzJWJajXaxb4VPd0hWOU3s0vgxz5KLexpbaFzBa34uisrRKqrey+u9U7QiwcX627D4JHw2yH/mls7gs5m4Nnk9jQAfOkwenF98ZkHwtw1ZcGYwKQqhYCGLwYu96dL48rZPlvGjPA0c1oVgWKIrycZMjc9b2bquiG8JY50mAfWbZrx1U/bW2SVqtmbdwd0QltZoF3C6vSf48uwT6TEM3UAJ7VTWPRocrrfTMFyYL4Wgvtb3kRsOcUf9RtgQmwxvK4HbpndEA8vraGpvfdYwdD6pYHSeyvf9o02jYctmiyu6luX5ZGCeLAR/h4m2r2yQ55N2PtEM52qHvA5egq5ABOOfSp5aEL2h+WvqIvHMbkELql4=';

    return `${this.reportBaseUrl}?q=${encryptedPath}`;
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.params;
    if (!data || !data.height) {
      return;
    }

    this.height = data.height;
  }
}
