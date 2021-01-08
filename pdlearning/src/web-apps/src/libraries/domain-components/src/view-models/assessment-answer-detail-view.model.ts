import {
  Assessment,
  AssessmentAnswer,
  AssessmentCriteria,
  AssessmentCriteriaAnswer,
  AssessmentScale,
  UserInfoModel
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class AssessmentAnswerDetailViewModel {
  public assessmentAnswerData: AssessmentAnswer = new AssessmentAnswer();
  public originAssessmentAnswerData: AssessmentAnswer = new AssessmentAnswer();

  constructor(
    assessmentAnswer?: AssessmentAnswer,
    public assessment: Assessment = new Assessment(),
    public modifyUser: UserInfoModel = null
  ) {
    if (assessmentAnswer != null) {
      this.updateAssessmentAnswerData(assessmentAnswer);
    }
  }

  public get id(): string {
    return this.assessmentAnswerData.id;
  }

  public get submittedDate(): Date {
    return this.assessmentAnswerData.submittedDate;
  }

  public get changedDate(): Date {
    return this.assessmentAnswerData.changedDate;
  }

  public get criteria(): AssessmentCriteria[] {
    return this.assessment.criteria;
  }

  public get scales(): AssessmentScale[] {
    return this.assessment.scales;
  }

  public get criteriaAnswers(): AssessmentCriteriaAnswer[] {
    return this.assessmentAnswerData.criteriaAnswers;
  }

  public getCriteriaAnswer(criteriaId: string): AssessmentCriteriaAnswer {
    let criteriaAnswer = this.assessmentAnswerData.criteriaAnswers.find(x => x.criteriaId === criteriaId);

    if (criteriaAnswer == null) {
      criteriaAnswer = new AssessmentCriteriaAnswer({
        criteriaId: criteriaId,
        scaleId: '',
        comment: ''
      });
      this.assessmentAnswerData.criteriaAnswers.push(criteriaAnswer);
    }

    return criteriaAnswer;
  }

  public updateScaleAnswer(criteriaId: string, scaleId: string): void {
    const criteriaAnswer = this.assessmentAnswerData.criteriaAnswers.find(x => x.criteriaId === criteriaId);
    criteriaAnswer.scaleId = scaleId;
  }

  public updateCommentAnswer(criteriaId: string, comment: string): void {
    const criteriaAnswer = this.assessmentAnswerData.criteriaAnswers.find(x => x.criteriaId === criteriaId);
    criteriaAnswer.comment = comment;
  }

  public updateAssessmentAnswerData(assessmentAnswer: AssessmentAnswer): void {
    this.originAssessmentAnswerData = Utils.cloneDeep(assessmentAnswer);
    this.assessmentAnswerData = Utils.cloneDeep(assessmentAnswer);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originAssessmentAnswerData, this.assessmentAnswerData);
  }
}
