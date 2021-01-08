import { Component, Input, OnInit } from '@angular/core';
import {
  CourseContentItemModel,
  CourseContentItemType,
} from 'app-models/course-content-item.model';

@Component({
  selector: 'table-of-content',
  templateUrl: './table-of-content.component.html',
  styleUrls: ['./table-of-content.component.scss'],
})
export class TableOfContentComponent implements OnInit {
  public contentType: typeof CourseContentItemType = CourseContentItemType;
  @Input() contents: CourseContentItemModel[];

  constructor() {}

  ngOnInit(): void {}
}
