import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import { CourseCriteriaDetailMode } from '../../models/course-criteria-detail-mode.model';
import { CourseCriteriaDetailViewModel } from './../../view-models/course-criteria-detail-view.model';
import { CourseCriteriaTabContent } from '../../models/course-criteria-tab.model';
import { FormGroup } from '@angular/forms';
import { ScrollableMenu } from '@opal20/common-components';

@Component({
  selector: 'course-criteria-detail',
  templateUrl: './course-criteria-detail.component.html'
})
export class CourseCriteriaDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: CourseCriteriaDetailMode;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;

  public get courseCriteria(): CourseCriteriaDetailViewModel {
    return this._courseCriteria;
  }

  @Input()
  public set courseCriteria(v: CourseCriteriaDetailViewModel) {
    this._courseCriteria = v;
  }
  @ViewChild('learnerRelatedCriteriaTab', { static: false })
  public learnerRelatedCriteriaTab: ElementRef;

  @ViewChild('courseRelatedCriteriaTabTab', { static: false })
  public courseRelatedCriteriaTabTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: CourseCriteriaTabContent.LearnerRelatedCriteria,
      title: 'Learner Related Criteria',
      elementFn: () => {
        return this.learnerRelatedCriteriaTab;
      }
    },
    {
      id: CourseCriteriaTabContent.CourseRelatedCriteria,
      title: 'Course Related Criteria',
      elementFn: () => {
        return this.courseRelatedCriteriaTabTab;
      }
    }
  ];
  public activeTab: string = CourseCriteriaTabContent.LearnerRelatedCriteria;
  public loadingData: boolean = false;
  private _courseCriteria: CourseCriteriaDetailViewModel;
  public static asViewMode(mode: CourseCriteriaDetailMode): boolean {
    return mode === CourseCriteriaDetailMode.View;
  }

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
