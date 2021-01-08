import { AssessmentCriteriaAnswer } from '../models/assessment-criteria-answer.model';

export interface ISaveAssessmentAnswerRequest {
  id: string;
  criteriaAnswers: AssessmentCriteriaAnswer[];
  isSubmit: boolean;
}
