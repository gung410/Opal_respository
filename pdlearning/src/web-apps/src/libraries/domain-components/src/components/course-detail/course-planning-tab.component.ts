import { BaseComponent, MAX_INT, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { NatureCourseType } from '@opal20/domain-api';

@Component({
  selector: 'course-planning-tab',
  templateUrl: './course-planning-tab.component.html'
})
export class CoursePlanningTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;

  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public maxInt: number = MAX_INT;
  public maxMaxReLearningTimes = MAX_INT - 1;
  public get createMode(): string {
    if (this.mode === CourseDetailMode.Recurring) {
      return this.translate('Recurring');
    }
    return this.translate('New');
  }

  public natureCourses = [
    {
      id: NatureCourseType.FullTime,
      label: 'Full-time'
    },
    {
      id: NatureCourseType.PartTime,
      label: 'Part-time'
    },
    {
      id: NatureCourseType.FullTimeCumPartTime,
      label: 'Full-time cum Part-time'
    },
    {
      id: NatureCourseType.DigitalLearning,
      label: 'Digital Learning'
    }
  ];
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
  }
}
