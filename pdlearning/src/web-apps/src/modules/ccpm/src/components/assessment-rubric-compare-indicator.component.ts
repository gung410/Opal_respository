import {
  CompareSide,
  IComparisonFormViewModel,
  IComparisonQuestionViewModel,
  ItemChangeType
} from './dialogs/compare-version-form-dialog.component';
import { Component, Input } from '@angular/core';

import { FormDetailMode } from '@opal20/domain-components';
import { QuestionType } from '@opal20/domain-api';

@Component({
  selector: 'assessment-rubric-compare-indicator',
  templateUrl: './assessment-rubric-compare-indicator.component.html'
})
export class AssessmentRubricCompareIndicatorComponent {
  @Input() public type: 'Scale' | 'Criteria' | 'Rubric' = 'Scale';
  @Input() public mode: FormDetailMode = FormDetailMode.View;
  @Input() public comparisonData: IComparisonFormViewModel[] = [];
  @Input() public side: CompareSide = CompareSide.Left;

  @Input() public canShowDescripton: boolean = true;
  @Input() public canShowScaleValue: boolean = true;

  public ITEMCHANGETYPE = ItemChangeType;

  public get editableData(): IComparisonFormViewModel[] {
    switch (this.type) {
      case 'Scale':
        return this.comparisonData.filter(q => q.formQuestions[0].questionType === QuestionType.Scale);
      case 'Criteria':
        return this.comparisonData.filter(q => q.formQuestions[0].questionType === QuestionType.Criteria);
      default:
        return this.comparisonData;
    }
  }

  public getRubricValue(scaleId: string, criteriaId: string): string {
    const a = this.editableData.find(data => data.formQuestions[0].id === criteriaId);

    return a.formQuestions[0].questionOptions.find(x => x.scaleId === scaleId).value.toString();
  }

  public get scaleData(): IComparisonQuestionViewModel[] {
    return this.editableData.map(d => d.formQuestions[0]).filter(q => q.questionType === QuestionType.Scale);
  }

  public get criteriaData(): IComparisonQuestionViewModel[] {
    return this.editableData.map(d => d.formQuestions[0]).filter(q => q.questionType === QuestionType.Criteria);
  }

  public checkAssessmentDataBlankArea(changeType: ItemChangeType): boolean {
    return (
      (this.side === CompareSide.Right && changeType === ItemChangeType.Removed) ||
      (this.side === CompareSide.Left && changeType === ItemChangeType.AddNew)
    );
  }
}
