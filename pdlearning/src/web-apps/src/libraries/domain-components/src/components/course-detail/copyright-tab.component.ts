import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'copyright-tab',
  templateUrl: './copyright-tab.component.html'
})
export class CopyRightTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;

  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
  }
}
