import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { QuestionType } from '@opal20/domain-api';
import { QuestionTypeSelectionService } from '../../services/question-type-selection.service';

@Component({
  selector: 'question-date-option-selection-dialog',
  templateUrl: './question-date-option-selection-dialog.component.html'
})
export class QuestionDateOptionSelectionDialogComponent extends BaseFormComponent {
  public readonly QuestionType: typeof QuestionType = QuestionType;

  public selectedQuestionType: QuestionType;

  public priority: number;

  public minorPriority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    private questionTypeSelectionService: QuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  public onDateQuestionSelected(): void {
    if (this.selectedQuestionType) {
      this.questionTypeSelectionService.setNewQuestionType(this.selectedQuestionType, this.priority, this.minorPriority);
      this.dialogRef.close();
    }
  }

  public onCloseDialog(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
  }

  public onDateQuestionChanged(dateQuestion: QuestionType): void {
    this.selectedQuestionType = dateQuestion;
  }
}
