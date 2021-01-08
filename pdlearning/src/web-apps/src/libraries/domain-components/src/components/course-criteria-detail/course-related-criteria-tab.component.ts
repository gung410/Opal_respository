import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Course, CourseRepository, SearchCourseType, SystemRoleEnum } from '@opal20/domain-api';

import { CourseCriteriaDetailComponent } from './course-criteria-detail.component';
import { CourseCriteriaDetailMode } from '../../models/course-criteria-detail-mode.model';
import { CourseCriteriaDetailViewModel } from '../../view-models/course-criteria-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'course-related-criteria-tab',
  templateUrl: './course-related-criteria-tab.component.html'
})
export class CourseRelatedCriteriaTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public courseCriteria: CourseCriteriaDetailViewModel;
  @Input() public mode: CourseCriteriaDetailMode | undefined;
  public fetchPrerequisiteCourseFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]>;
  public CourseCriteriaDetailMode: typeof CourseCriteriaDetailMode = CourseCriteriaDetailMode;
  constructor(public moduleFacadeService: ModuleFacadeService, private courseRepository: CourseRepository) {
    super(moduleFacadeService);
    this.fetchPrerequisiteCourseFn = this._createFetchPrerequisiteCourseFn();
  }

  public asViewMode(): boolean {
    return CourseCriteriaDetailComponent.asViewMode(this.mode);
  }

  private _createFetchPrerequisiteCourseFn(
    inRoles?: SystemRoleEnum[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.courseRepository
        .loadSearchCourses(_searchText, null, SearchCourseType.Prerequisite, null, _skipCount, _maxResultCount, null, null, false)
        .pipe(map(_ => _.items));
    };
  }
}
