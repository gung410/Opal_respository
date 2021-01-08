import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { AssignmentQuestionTypeSelectionService } from '../../services/assignment-question-type-selection.service';
import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { QuizAssignmentQuestionType } from '@opal20/domain-api';

@Component({
  selector: 'assignment-question-date-option-selection-dialog',
  templateUrl: './assignment-question-date-option-selection-dialog.component.html'
})
export class AssignmentQuestionDateOptionSelectionDialogComponent extends BaseFormComponent {
  public readonly QuizAssignmentQuestionType: typeof QuizAssignmentQuestionType = QuizAssignmentQuestionType;

  public selectedQuestionType: QuizAssignmentQuestionType;

  public priority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    private assignmentQuestionTypeSelectionService: AssignmentQuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  public onDateQuestionSelected(): void {
    if (this.selectedQuestionType) {
      this.assignmentQuestionTypeSelectionService.setNewQuestionType(this.selectedQuestionType, this.priority);
      this.dialogRef.close();
    }
  }

  public onCloseDialog(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
  }

  public onDateQuestionChanged(dateQuestion: QuizAssignmentQuestionType): void {
    this.selectedQuestionType = dateQuestion;
  }
}
