import { AssessmentCriteriaScale, AssessmentScale } from '@opal20/domain-api';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'assessment-scale',
  templateUrl: './assessment-scale.component.html'
})
export class AssessmentScaleComponent extends BaseFormComponent {
  @Input() public scale: AssessmentScale = new AssessmentScale();
  @Input() public criteriaScale: AssessmentCriteriaScale = new AssessmentCriteriaScale();
  @Input() public checked: boolean = false;
  @Input() public viewMode: boolean = false;

  @Output('checkedChange') public checkedChangeEvent: EventEmitter<boolean> = new EventEmitter();
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onCheckboxClicked(e: MouseEvent): void {
    if (this.viewMode) {
      e.preventDefault();
      return;
    }

    const newCheckedValue = true;
    if (newCheckedValue !== this.checked) {
      this.checked = newCheckedValue;
      this.checkedChangeEvent.emit(this.checked);
    } else {
      e.preventDefault();
    }
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {}
    };
  }
}
