import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { BaseUserInfo, Course, CourseRepository, SearchCourseType } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'select-course-dialog',
  templateUrl: './select-course-dialog.component.html'
})
export class SelectCourseDialogComponent extends BaseComponent {
  @Input() public title: string;

  @Input() public searchCourseType: SearchCourseType;

  @Input() public excludeCourseId: string = '';

  public selectedValue: string;
  public fetchCourseSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]>;
  public ignoreCourseItemFn: (item: BaseUserInfo) => boolean;

  public static selectRecurringCoursesConfig(): Partial<SelectCourseDialogComponent> {
    return {
      title: 'Recurring Course',
      searchCourseType: SearchCourseType.Recurring
    };
  }

  public static selectToCloneCoursesConfig(): Partial<SelectCourseDialogComponent> {
    return {
      title: 'Duplicate Course',
      searchCourseType: SearchCourseType.Cloneable
    };
  }

  public static selectToCloneCourseContentsConfig(courseId: string): Partial<SelectCourseDialogComponent> {
    return {
      title: 'Duplicate Course Content',
      searchCourseType: SearchCourseType.Owner,
      excludeCourseId: courseId
    };
  }

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, public courseRepository: CourseRepository) {
    super(moduleFacadeService);
    this.fetchCourseSelectItemFn = this._createFetchCourseSelectFn();
    this.ignoreCourseItemFn = x => x.id === this.excludeCourseId;
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onProceed(): void {
    this.dialogRef.close({ id: this.selectedValue });
  }

  private _createFetchCourseSelectFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> {
    return (searchText, skipCount, maxResultCount) => {
      return this.courseRepository
        .loadSearchCourses(searchText, null, this.searchCourseType, null, skipCount, maxResultCount, null, null, false)
        .pipe(map(_ => _.items));
    };
  }
}
