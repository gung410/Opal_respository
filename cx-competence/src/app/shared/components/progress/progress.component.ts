import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'cx-progress',
  templateUrl: './progress.component.html',
  styleUrls: ['./progress.component.scss'],
})
export class ProgressComponent implements OnInit {
  @Input() progressClass: string;
  @Input() progressValue: any;
  @Input() progressText: any;

  constructor() {}

  ngOnInit() {}
}
