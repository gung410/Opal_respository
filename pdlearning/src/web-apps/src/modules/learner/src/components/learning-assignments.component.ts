import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { MY_ASSIGNMENT_STATUS_COLOR_MAP } from '@opal20/domain-components';
import { MyAssignmentDetail } from '../models/my-assignment-detail-model';
import { MyAssignmentStatus } from '@opal20/domain-api';

export type AssignmentClickedEvent = { assignment: MyAssignmentDetail; scrollToComment?: boolean };

@Component({
  selector: 'learning-assignments',
  templateUrl: './learning-assignments.component.html'
})
export class LearningAssignmentsComponent extends BaseComponent {
  @Input() public items: MyAssignmentDetail[];
  @Output() public assignmentClicked: EventEmitter<AssignmentClickedEvent> = new EventEmitter();
  public MyAssignmentStatus: typeof MyAssignmentStatus = MyAssignmentStatus;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get statusColorMap(): unknown {
    return MY_ASSIGNMENT_STATUS_COLOR_MAP;
  }

  public getButtonText(): string {
    return 'Comments';
  }

  public onAssignmentClicked(event: MyAssignmentDetail, enableScrollCommentSection?: boolean): void {
    this.assignmentClicked.emit({ assignment: event, scrollToComment: enableScrollCommentSection });
  }
}
