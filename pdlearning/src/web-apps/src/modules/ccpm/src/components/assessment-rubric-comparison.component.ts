import { CompareSide, IComparisonFormViewModel, ItemChangeType } from './dialogs/compare-version-form-dialog.component';
import { Component, Input } from '@angular/core';

import { FormDetailMode } from '@opal20/domain-components';
import { FormType } from '@opal20/domain-api';
import { Utils } from '@opal20/infrastructure';

@Component({
  selector: 'assessment-rubric-comparison',
  templateUrl: './assessment-rubric-comparison.component.html'
})
export class AssessmentRubricComparisonComponent {
  @Input() public type: FormType = FormType.Holistic;
  @Input() public mode: FormDetailMode = FormDetailMode.View;
  @Input() public comparisonData: IComparisonFormViewModel[] = [];
  @Input() public side: CompareSide = CompareSide.Left;

  public ITEMCHANGETYPE = ItemChangeType;

  public get compareAbleAssessmentData(): IComparisonFormViewModel[] {
    return Utils.orderBy(this.comparisonData, x => x.sectionPrioprity);
  }

  public get assessmentTitle(): string {
    return `${this.type} Rubric`;
  }
}
