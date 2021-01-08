import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { LearningPathCourseViewModel } from '../../view-models/learning-path-course-view.model';
import { LearningPathDetailViewModel } from '../../view-models/learning-path-detail-view.model';

@Component({
  selector: 'learning-path-selected-courses',
  templateUrl: './learning-path-selected-courses.component.html'
})
export class LearningPathSelectedSoursesComponent extends BaseFormComponent {
  @Input() public isViewMode: boolean;
  @Input() public learningPathDetailVM: LearningPathDetailViewModel;
  @Input() public hasThumbnail: boolean = false;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onMoveUpSelectedItem(dataItem: LearningPathCourseViewModel): void {
    this.learningPathDetailVM.changeCourseItemOrder(dataItem.learningPathCourse.order, 'up');
  }

  public onMoveDownSelectedItem(dataItem: LearningPathCourseViewModel): void {
    this.learningPathDetailVM.changeCourseItemOrder(dataItem.learningPathCourse.order, 'down');
  }

  public onRemoveSelectedItem(dataItem: LearningPathCourseViewModel): void {
    this.learningPathDetailVM.removeFromListCourses(dataItem);
  }

  public canMoveUpSelectedItem(dataItem: LearningPathCourseViewModel): boolean {
    return dataItem.learningPathCourse.order > 1 && !this.isViewMode;
  }

  public canMoveDownSelectedItem(dataItem: LearningPathCourseViewModel): boolean {
    return dataItem.learningPathCourse.order < this.learningPathDetailVM.learningPathCourseVMs.length && !this.isViewMode;
  }

  public canRemoveSelectedItem(): boolean {
    return !this.isViewMode;
  }

  public onContextMenuItemSelect(eventData: { id: string }, course: LearningPathCourseViewModel): void {
    switch (eventData.id) {
      case 'Delete':
        this.onRemoveSelectedItem(course);
        break;
      case 'MoveUp':
        this.onMoveUpSelectedItem(course);
        break;
      case 'MoveDown':
        this.onMoveDownSelectedItem(course);
        break;
    }
  }
}
