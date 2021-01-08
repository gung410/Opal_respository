import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { clone } from 'lodash';

@Component({
  selector: 'cx-gantt-scrubber',
  templateUrl: './cx-gantt-scrubber.component.html',
  styleUrls: ['./cx-gantt-scrubber.component.scss'],
})
export class CxGanttScrubberComponent
  implements OnInit, AfterViewInit, OnChanges {
  @Input() selectedYear: number;
  @Input() yearList: number[];
  @Output() changeYear: EventEmitter<number> = new EventEmitter();
  @ViewChild('scrubberSelector', { static: true }) scrubberSelector: ElementRef<HTMLElement>;
  scrubberYearList: number[] = [];
  yearStep: number = 40; //px
  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    this.calculateNewSelectorPosition(this.selectedYear);
  }

  ngOnInit(): void {
    if (!this.yearList) {
      return;
    }
    const yearList = clone(this.yearList);

    this.scrubberYearList = yearList.reverse();

    this.selectedYear = yearList[yearList.length - 1];
  }

  ngAfterViewInit(): void {
    const firstSelectorPosition = (this.yearList.length - 1) * this.yearStep;
    this.scrubberSelector.nativeElement.style.top = `${firstSelectorPosition}px`;
  }

  calculateNewSelectorPosition(year: number): void {
    const index = this.scrubberYearList.indexOf(year);
    const selectorPosition = index * this.yearStep;

    this.scrubberSelector.nativeElement.style.top = `${selectorPosition}px`;
  }

  onYearClicked(year: number): void {
    this.calculateNewSelectorPosition(year);
    this.selectedYear = year;
    this.changeYear.emit(year);
  }
}
