import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import { CoursePlanningCycleDetailMode } from './../../models/course-planning-cycle-detail-mode.model';
import { CoursePlanningCycleDetailViewModel } from './../../view-models/course-planning-cycle-detail-view.model';
import { CoursePlanningCycleTabInfo } from './../../models/course-planning-cycle-tab.model';
import { FormGroup } from '@angular/forms';
import { ScrollableMenu } from '@opal20/common-components';

@Component({
  selector: 'course-planning-cycle-detail',
  templateUrl: './course-planning-cycle-detail.component.html'
})
export class CoursePlanningCycleDetailComponent extends BaseComponent {
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;
  @Input() public form: FormGroup;

  @Input() public mode: CoursePlanningCycleDetailMode;

  public get coursePlanningCycle(): CoursePlanningCycleDetailViewModel {
    return this._coursePlanningCycle;
  }

  @Input()
  public set coursePlanningCycle(v: CoursePlanningCycleDetailViewModel) {
    this._coursePlanningCycle = v;
  }

  @ViewChild('planningCycleOverviewInfoTab', { static: false })
  public planningCycleOverviewInfoTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: CoursePlanningCycleTabInfo.OverviewInfo,
      title: 'Overview',
      elementFn: () => {
        return this.planningCycleOverviewInfoTab;
      }
    }
  ];
  public activeTab: string = CoursePlanningCycleTabInfo.OverviewInfo;
  public loadingData: boolean = false;
  private _coursePlanningCycle: CoursePlanningCycleDetailViewModel;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
