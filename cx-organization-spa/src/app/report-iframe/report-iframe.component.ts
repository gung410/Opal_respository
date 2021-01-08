import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'report-iframe',
  templateUrl: './report-iframe.component.html'
})
export class ReportIframeComponent implements OnInit {
  reportBaseUrl: string = AppConstant.moduleLink.Report;
  @ViewChild('reportIframe') reportIframeRef: ElementRef;

  ngOnInit(): void {
    if (this.reportIframeRef) {
      this.reportIframeRef.nativeElement.src = this.reportBaseUrl;
    }
  }
}
