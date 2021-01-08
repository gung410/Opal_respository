import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'cx-gantt-timeline',
  templateUrl: './cx-gantt-timeline.component.html',
  styleUrls: ['./cx-gantt-timeline.component.scss'],
})
export class CxGanttTimelineComponent implements OnInit {
  @Input() yearList: number[];
  @Input() currentYear: number;
  constructor() {}

  ngOnInit(): void {}
}
