import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';
import { tagGroupData, MockData } from './mock-data/tag-group-data';

@Component({
  selector: 'cx-tag-group-doc',
  templateUrl: './cx-tag-group-doc.component.html',
  styleUrls: ['./cx-tag-group-doc.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTagGroupDocComponent implements OnInit {
  public tagGroupObject: any;
  public tagGroupObjectSimple: any;
  constructor() { }

  ngOnInit() {
    this.tagGroupObject = tagGroupData.groups;
    this.tagGroupObjectSimple = MockData.applied_filter;
  }

  public onCloseTag(tag: any, tagGroupConst: string) {
    console.log(tag);
    console.log(tagGroupConst);
  }

  public onCloseTagGroup(tagGroup: any) {
    console.log(tagGroup);
  }

  public onClearAll() {
    console.log('clear all');
  }
}
