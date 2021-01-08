import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { Course, LearningPathCourseModel } from '@opal20/domain-api';
import { DialogAction, OpalDialogService, OpalSelectComponent } from '@opal20/common-components';

import { Align } from '@progress/kendo-angular-popup';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LearningPathCourseViewModel } from '../../view-models/learning-path-course-view.model';
import { LearningPathDetailViewModel } from '../../view-models/learning-path-detail-view.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'learning-path-pd-opportunities-dialog',
  templateUrl: './learning-path-pd-opportunities-dialog.component.html'
})
export class LearningPathPdOpportunitiesDialogComponent extends BaseFormComponent {
  @ViewChild(OpalSelectComponent, { static: false }) public opalSelectComponent: OpalSelectComponent;
  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }

  @Input()
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    if (Utils.isDifferent(v, this._learningPathDetailVM)) {
      this._learningPathDetailVM = v;
    }
  }
  @Input() public isViewMode: boolean;

  @Input()
  public fetchPublishedCoursesFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> = null;
  @Input() public openCoursePreviewDialogFn: (courseId: string) => DialogRef;

  public showingPreviewCourseDialog: boolean = false;
  public focusRow: number = -1;
  public popupAlign: Align = { horizontal: 'center', vertical: 'top' };
  private _selectedCourse: Course;
  private _learningPathDetailVM: LearningPathDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, private opalDialogService: OpalDialogService) {
    super(moduleFacadeService);
  }

  public selectSearchFn: (term: string, item: object | string | number) => boolean = () => true;

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onSubmit(): void {
    this.dialogRef.close({ learningPathDetailVM: this.learningPathDetailVM });
  }

  public onSelectedItemChanged(): void {
    this.opalSelectComponent.ngSelect.handleClearClick();
  }

  public onClickAddPDOpportunities(selectedCourse: Course): void {
    this._selectedCourse = selectedCourse;
    const newLearningPathCourse = new LearningPathCourseModel();
    newLearningPathCourse.id = null;
    newLearningPathCourse.order = this.learningPathDetailVM.listCourses.length + 1;
    newLearningPathCourse.courseId = this._selectedCourse.id;

    const newLearningPathCourseVM = new LearningPathCourseViewModel(newLearningPathCourse, this._selectedCourse);
    this.learningPathDetailVM.addToListCourses(newLearningPathCourseVM);
  }

  public openCoursePreview(selectedCourse: Course): void {
    if (this.openCoursePreviewDialogFn != null) {
      const dialogRef: DialogRef = this.openCoursePreviewDialogFn(selectedCourse.id);
      this.subscribe(dialogRef.result, (action: DialogAction) => {
        this.showingPreviewCourseDialog = false;
      });
    }
    this.showingPreviewCourseDialog = true;
  }

  public onRemoveSelectedItem(dataItem: LearningPathCourseViewModel): void {
    this.learningPathDetailVM.removeFromListCourses(dataItem);
  }

  public isCourseExit(dataItem: Course): boolean {
    if (dataItem) {
      return this.learningPathDetailVM.learningPathCourseVMs.some(course => course.course.id === dataItem.id);
    }
  }

  public canPreviewCourse(dataItem: Course): boolean {
    return this.openCoursePreviewDialogFn != null;
  }

  public removeCourseSelected(dataItem: Course): boolean {
    return this.learningPathDetailVM.learningPathCourseVMs.some(course => {
      if (course.course.id === dataItem.id) {
        this.onRemoveSelectedItem(course);
      }
    });
  }
}
