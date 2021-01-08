import { AssignmentType, QuizAssignmentQuestionType } from '@opal20/domain-api';
import { BaseComponent, MAX_ZINDEX_OF_TOOLTIP, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { AssignmentQuestionDateOptionSelectionDialogComponent } from './dialogs/assignment-question-date-option-selection-dialog.component';
import { AssignmentQuestionTypeSelectionService } from '../services/assignment-question-type-selection.service';
import { OpalDialogService } from '@opal20/common-components';

@Component({
  selector: 'assignment-question-template',
  templateUrl: './assignment-question-template.component.html'
})
export class AssignmentQuestionTemplateComponent extends BaseComponent {
  public AssignmentType: typeof AssignmentType = AssignmentType;
  public readonly QuizAssignmentQuestionType: typeof QuizAssignmentQuestionType = QuizAssignmentQuestionType;

  @Input() public type: AssignmentType = AssignmentType.Quiz;
  @Input() public priority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private assignmentQuestionTypeSelectionService: AssignmentQuestionTypeSelectionService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onDateQuestionTemplateClicked(): void {
    const dateQuestionDialogRef = this.opalDialogService.openDialogRef(
      AssignmentQuestionDateOptionSelectionDialogComponent,
      {
        priority: this.priority
      },
      { zIndex: MAX_ZINDEX_OF_TOOLTIP }
    );
  }

  public onQuestionTemplateClick(newQuestionType: QuizAssignmentQuestionType): void {
    if (newQuestionType) {
      this.assignmentQuestionTypeSelectionService.setNewQuestionType(newQuestionType, this.priority);
    }
  }
}
