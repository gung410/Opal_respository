import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { StandaloneSurveyQuestionType } from '@opal20/domain-api';
import { StandaloneSurveyQuestionTypeSelectionService } from '@opal20/domain-components';

@Component({
  selector: 'standalone-survey-date-question-selection-dialog',
  templateUrl: './standalone-survey-date-question-selection-dialog.component.html'
})
export class StandaloneSurveyDateQuestionSelectionDialogComponent extends BaseFormComponent {
  public readonly StandaloneSurveyQuestionType: typeof StandaloneSurveyQuestionType = StandaloneSurveyQuestionType;

  public selectedQuestionType: StandaloneSurveyQuestionType;

  public priority: number;

  public minorPriority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    private questionTypeSelectionService: StandaloneSurveyQuestionTypeSelectionService
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

  public onDateQuestionChanged(dateQuestion: StandaloneSurveyQuestionType): void {
    this.selectedQuestionType = dateQuestion;
  }
}
