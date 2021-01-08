export class LearningNeedAnalysisRemindingRequest {
  userExtId: string;
  userFullName: string;
  resultExtId: string;
  year?: number;
  cutOffDate: Date;
  constructor(data?: Partial<LearningNeedAnalysisRemindingRequest>) {
    if (!data) {
      return;
    }
    this.userExtId = data.userExtId;
    this.userFullName = data.userFullName;
    this.resultExtId = data.resultExtId;
    this.year = data.year || new Date().getFullYear();
    this.cutOffDate = data.cutOffDate;
  }
}
export class LearningNeedAnalysisRemindingList {
  learningNeedCompletionRemindings: LearningNeedAnalysisRemindingRequest[];
  dateToSend?: Date;
  constructor(data?: Partial<LearningNeedAnalysisRemindingList>) {
    this.learningNeedCompletionRemindings =
      data.learningNeedCompletionRemindings || [];
    this.dateToSend = data.dateToSend || undefined;
  }
}
