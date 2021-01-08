export interface ITotalParticipantClassRunResult {
  classRunId?: string;
  participantTotal?: number;
}

export class TotalParticipantClassRunResult implements ITotalParticipantClassRunResult {
  public classRunId?: string;
  public participantTotal?: number;

  constructor(data?: ITotalParticipantClassRunResult) {
    if (data == null) {
      return;
    }
    this.classRunId = data.classRunId;
    this.participantTotal = data.participantTotal;
  }
}
