import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'lna-survey-link',
  templateUrl: './lna-survey-link.component.html',
  styleUrls: ['./lna-survey-link.component.scss'],
})
export class LnaSurveyLinkComponent implements OnInit {
  @Input() link: string;
  @Output() clickStart: EventEmitter<void> = new EventEmitter();
  constructor() {}

  ngOnInit(): void {}
}
