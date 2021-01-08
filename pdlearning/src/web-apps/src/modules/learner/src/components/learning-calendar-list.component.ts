import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, OnInit } from '@angular/core';

import { LearnerNavigationService } from '../services/learner-navigation.service';
import { LearnerRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'learning-calendar-list',
  templateUrl: './learning-calendar-list.component.html'
})
export class LearningCalendarListComponent extends BaseComponent implements OnInit {
  public get isEmpty(): boolean {
    return true;
  }
  @Input()
  public numberOfHeightTimes: number;

  public showWorkHours: boolean = true;

  constructor(protected moduleFacadeService: ModuleFacadeService, private learnerNavigationService: LearnerNavigationService) {
    super(moduleFacadeService);
  }

  public onShowMoreClicked(): void {
    this.learnerNavigationService.navigateTo(LearnerRoutePaths.Calendar);
  }
}
