import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { CoursePlanningCycleDetailMode } from './../../models/course-planning-cycle-detail-mode.model';
import { CoursePlanningCycleDetailViewModel } from './../../view-models/course-planning-cycle-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'course-planning-cycle-overview-info-tab',
  templateUrl: './course-planning-cycle-overview-info-tab.component.html'
})
export class CoursePlanningCycleOverviewInfoTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public coursePlanningCycle: CoursePlanningCycleDetailViewModel;
  @Input() public mode: CoursePlanningCycleDetailMode | undefined;

  public CoursePlanningCycleDetailMode: typeof CoursePlanningCycleDetailMode = CoursePlanningCycleDetailMode;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public isViewMode(): boolean {
    return (
      this.mode === CoursePlanningCycleDetailMode.View ||
      (!this.coursePlanningCycle.canEditStartDate() && this.mode === CoursePlanningCycleDetailMode.Edit)
    );
  }
}
