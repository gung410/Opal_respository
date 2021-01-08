import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  CxGanttChartModel,
  CxGanttTaskRenderModel,
} from './models/cx-gantt-chart.model';

@Component({
  selector: 'cx-gantt-chart',
  templateUrl: './cx-gantt-chart.component.html',
  styleUrls: ['./cx-gantt-chart.component.scss'],
})
export class CxGanttChartComponent implements OnInit, AfterViewInit {
  @Input() data: CxGanttChartModel;
  @ViewChild('cxGanttChartViewer', { static: true })
  cxGanttChartViewer: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartHeader', { static: true })
  cxGanttChartHeader: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartController', { static: true })
  cxGanttChartController: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartTasksList', { static: true })
  cxGanttChartTasksList: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartScrollDiv', { static: true })
  cxGanttChartScrollDiv: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartTimeline', { read: ElementRef, static: true })
  cxGanttChartTimeline: ElementRef<HTMLElement>;
  @ViewChild('cxGanttChartScrubber', { read: ElementRef, static: true })
  cxGanttChartScrubber: ElementRef<HTMLElement>;

  taskRenderDataList: CxGanttTaskRenderModel[] = [];
  yearList: number[] = [];
  currentYear: number = 0;

  private yearStep: number = 0;
  private isDragging: boolean = false;
  private minYear: number = 0;
  private maxYear: number = 0;

  constructor(private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.initData();
  }

  ngAfterViewInit(): void {
    if (!this.verifyRenderedElement()) {
      console.error('CxGanttChartComponent has trouble when render elements');
    }

    this.calculateYearStep();
    this.processTaskDataRender();
    this.setHeightForContentViewer();
    this.setControllerContainerMargin();
  }

  onScroll(): void {
    if (this.isDragging) {
      this.calcCurrentYearByCurrentScroll();
    }
  }

  onMouseDown(): void {
    this.isDragging = true;
  }

  onMouseUp(): void {
    this.isDragging = false;
  }

  onBackward(): void {
    this.onChangeYear(this.currentYear - 1);
  }

  onForward(): void {
    this.onChangeYear(this.currentYear + 1);
  }

  onScrubberChangeYear(year: number): void {
    if (year >= this.minYear) {
      this.onChangeYear(year);
    }
  }

  get enableBackward(): boolean {
    return this.currentYear > this.minYear;
  }

  get enableForward(): boolean {
    return this.currentYear < this.maxYear;
  }

  private initData(): void {
    if (!this.data || !this.data.tasks || this.data.tasks.length <= 0) {
      console.error('Invalid data input for cx-gantt-chart');

      return;
    }

    if (
      this.data.minYear === null ||
      this.data.maxYear === null ||
      this.data.minYear <= 0 ||
      this.data.minYear > this.data.maxYear
    ) {
      console.error('Invalid input date range for cx-gantt-chart');

      return;
    }

    this.minYear = this.data.minYear;
    this.maxYear = this.processMaxYear();

    this.currentYear = this.minYear;
    this.processYearList();
  }

  private onChangeYear(year: number): void {
    this.currentYear = Math.min(Math.max(year, this.minYear), this.maxYear);
    this.updateScrollToCurrentYear();
  }

  private updateScrollToCurrentYear(): void {
    const currentStep = this.currentYear - this.minYear;
    const scrollLeft = currentStep * this.yearStep;
    this.cxGanttChartScrollDiv.nativeElement.scrollLeft = scrollLeft;
  }

  private setHeightForContentViewer(): void {
    const cxGanttChartViewerElement = this.cxGanttChartViewer.nativeElement;
    const cxGanttChartTasksListElement = this.cxGanttChartTasksList
      .nativeElement;
    const offsetHeight = 100; // px
    const contentHeight =
      cxGanttChartTasksListElement.offsetHeight + offsetHeight;

    if (contentHeight > 0) {
      cxGanttChartViewerElement.style.minHeight = contentHeight + 'px';
    }
  }

  private setControllerContainerMargin(): void {
    const cxGanttChartScrubberElement = this.cxGanttChartScrubber.nativeElement;
    const cxGanttChartHeaderElement = this.cxGanttChartHeader.nativeElement;
    const cxGanttChartControllerElement = this.cxGanttChartController
      .nativeElement;
    const scrubberWidth = cxGanttChartScrubberElement.clientWidth;

    cxGanttChartHeaderElement.style.marginLeft = `${scrubberWidth}px`;
    cxGanttChartControllerElement.style.marginLeft = `${scrubberWidth}px`;
    cxGanttChartControllerElement.style.width = `calc(100% - ${scrubberWidth}px)`;
  }

  private calculateYearStep(): void {
    const timeLineYearContainerClass = '.cx-gantt-timeline-year-container';
    const cxGanttChartTimelineElement = this.cxGanttChartTimeline.nativeElement;
    const firstYearElement = cxGanttChartTimelineElement.querySelector(
      timeLineYearContainerClass
    );

    if (firstYearElement) {
      this.yearStep = firstYearElement.clientWidth;
    }
  }

  private processTaskDataRender(): void {
    this.taskRenderDataList = this.data.tasks.map(
      (task) => new CxGanttTaskRenderModel(task, this.minYear, this.yearStep)
    );
    this.changeDetectorRef.detectChanges();
  }

  private processYearList(): void {
    for (let i = this.minYear; i <= this.maxYear; i++) {
      this.yearList.push(i);
    }
  }

  private calcCurrentYearByCurrentScroll(): void {
    const offsetValue = 10;
    const currentScrollLeft =
      this.cxGanttChartScrollDiv.nativeElement.scrollLeft + offsetValue;
    const currentStep = currentScrollLeft / this.yearStep;
    const currentStepFloored = Math.floor(currentStep);
    const currentYear = this.minYear + currentStepFloored;

    this.currentYear = currentYear;
  }

  private verifyRenderedElement(): boolean {
    if (
      !this.cxGanttChartScrollDiv ||
      !this.cxGanttChartTimeline ||
      !this.cxGanttChartScrubber ||
      !this.cxGanttChartController ||
      !this.cxGanttChartViewer ||
      !this.cxGanttChartTasksList ||
      !this.cxGanttChartHeader
    ) {
      return false;
    }

    return true;
  }

  private processMaxYear(): number {
    const additionalYear = 2;
    const maxYear = this.data && this.data.maxYear;

    return this.data.minYear === maxYear ? maxYear + additionalYear : maxYear;
  }
}
