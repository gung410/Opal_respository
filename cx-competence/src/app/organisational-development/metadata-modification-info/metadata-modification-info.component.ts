import { Component, OnInit, Input } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'metadata-modification-info',
  templateUrl: './metadata-modification-info.component.html',
  styleUrls: ['./metadata-modification-info.component.scss'],
})
export class MetadataModificationInfoComponent implements OnInit {
  @Input() odpDto: any;

  constructor() {}

  ngOnInit() {}

  public getDateTimeFromNow(date: string) {
    return moment(date).fromNow();
  }

  public getDateTimeFormat(date) {
    return moment(date).format('lll');
  }
}
