import {
  AssessmentStatusInfo,
  ObjectiveInfo,
  ResultIdentity,
} from 'app-models/assessment.model';

export class StatusHistoryDto {
  resultIdentity: ResultIdentity;
  timestamp: string;
  sourceStatusType: AssessmentStatusInfo;
  targetStatusType: AssessmentStatusInfo;
  changedBy: ObjectiveInfo;
  constructor(data?: Partial<StatusHistoryDto>) {
    if (!data) {
      return;
    }

    this.resultIdentity = data.resultIdentity;
    this.timestamp = data.timestamp;
    this.sourceStatusType = data.sourceStatusType;
    this.targetStatusType = data.targetStatusType;
    this.changedBy = data.changedBy;
  }
}
