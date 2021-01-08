import { AssessmentCriteria, AssessmentCriteriaAnswer, AssessmentCriteriaScale, AssessmentScale } from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { AssessmentAnswerDetailViewModel } from './../../view-models/assessment-answer-detail-view.model';
import { ValidationErrors } from '@angular/forms';
import { of } from 'rxjs';

@Component({
  selector: 'assessment-criteria',
  templateUrl: './assessment-criteria.component.html'
})
export class AssessmentCriteriaComponent extends BaseFormComponent {
  @Input() public criteria: AssessmentCriteria = new AssessmentCriteria();
  @Input() public scales: AssessmentScale[] = [];
  @Input() public viewMode: boolean = false;
  @Input() public assessmentAnswerVm: AssessmentAnswerDetailViewModel = new AssessmentAnswerDetailViewModel();
  public criteriaScaleDic: Dictionary<AssessmentCriteriaScale> = {};

  public get logicValidationErrorKeys(): string[] | undefined {
    return this.logicValidationErrors ? Object.keys(this.logicValidationErrors) : undefined;
  }
  public logicValidationErrors: ValidationErrors | undefined;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public updateScaleAnswer(e: boolean, scaleId: string): void {
    if (e === false) {
      this.assessmentAnswerVm.updateScaleAnswer(this.criteria.id, '');
    } else {
      this.assessmentAnswerVm.updateScaleAnswer(this.criteria.id, scaleId);
    }

    this.validate();
  }

  protected onInit(): void {
    this.criteriaScaleDic = Utils.toDictionarySelect(this.criteria.scales, x => x.scaleId, x => x);
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return of(!this.anyLogicError()).toPromise();
  }

  private anyLogicError(): ValidationErrors | undefined {
    this.logicValidationErrors = AssessmentCriteriaAnswer.validate(this.assessmentAnswerVm.getCriteriaAnswer(this.criteria.id));
    return this.logicValidationErrors;
  }
}
