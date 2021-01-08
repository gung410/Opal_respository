import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CxGanttChartComponent } from './cx-gantt-chart.component';
import { CxGanttScrubberComponent } from './cx-gantt-scrubber/cx-gantt-scrubber.component';
import { CxGanttTaskComponent } from './cx-gantt-task/cx-gantt-task.component';
import { CxGanttTimelineComponent } from './cx-gantt-timeline/cx-gantt-timeline.component';

@NgModule({
  imports: [CommonModule, NgbModule],
  declarations: [
    CxGanttScrubberComponent,
    CxGanttTaskComponent,
    CxGanttTimelineComponent,
    CxGanttChartComponent,
  ],
  exports: [
    CxGanttScrubberComponent,
    CxGanttTaskComponent,
    CxGanttTimelineComponent,
    CxGanttChartComponent,
  ],
})
export class CxGanttChartModule {}
