import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

import { ApiDetailDataService } from './api-detail-data.service';

@Component({
  selector: 'api-detail',
  templateUrl: './api-detail.component.html',
  styleUrls: ['./api-detail.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [ApiDetailDataService]
})
export class ApiDetailComponent implements OnInit {
  @Input() originResource: string;
  monitorStatus: object;
  constructor(private apiDetailDataService: ApiDetailDataService) {}

  ngOnInit(): void {
    this.apiDetailDataService
      .getMonitorStatus(this.originResource)
      .subscribe((monitorStatus) => {
        this.monitorStatus = monitorStatus;
      });
  }
}
