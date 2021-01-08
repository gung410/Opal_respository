import { BaseComponent, FileUploaderSetting, MAX_INT, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { MetadataId } from '@opal20/domain-api';

@Component({
  selector: 'basic-info-tab',
  templateUrl: './basic-info-tab.component.html'
})
export class BasicInfoTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  public get course(): CourseDetailViewModel {
    return this._course;
  }
  @Input()
  public set course(v: CourseDetailViewModel) {
    this._course = v;
    this.fileUploaderSetting.extensions = this._course.allowedThumbnailExtensions;
  }
  @Input() public mode: CourseDetailMode | undefined;
  public maxInt: number = MAX_INT;
  public MetadataId: typeof MetadataId = MetadataId;
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public fileUploaderSetting: FileUploaderSetting;
  private _course: CourseDetailViewModel;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
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
