import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { StandaloneSurveyDateQuestionSelectionDialogComponent } from '../standalone-survey-date-selection-dialog/standalone-survey-date-question-selection-dialog.component';
import { StandaloneSurveyQuestionType } from '@opal20/domain-api';
import { StandaloneSurveyQuestionTypeSelectionService } from '../../services/standalone-survey-question-type-selection.service';

@Component({
  selector: 'standalone-survey-question-template',
  templateUrl: './standalone-survey-question-template.component.html'
})
export class StandaloneSurveyQuestionTemplateComponent extends BaseComponent {
  public readonly questionTypeEnum: typeof StandaloneSurveyQuestionType = StandaloneSurveyQuestionType;

  @Input() public priority: number;
  @Input() public minorPriority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private questionTypeSelectionService: StandaloneSurveyQuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  public onDateQuestionTemplateClicked(): void {
    const dateQuestionDialogRef = this.moduleFacadeService.dialogService.open({
      content: StandaloneSurveyDateQuestionSelectionDialogComponent
    });
    dateQuestionDialogRef.content.instance.priority = this.priority;
    dateQuestionDialogRef.content.instance.minorPriority = this.minorPriority;
  }

  public onQuestionTemplateClick(newQuestionType: StandaloneSurveyQuestionType): void {
    if (newQuestionType) {
      this.questionTypeSelectionService.setNewQuestionType(newQuestionType, this.priority, this.minorPriority);
    }
  }
}
