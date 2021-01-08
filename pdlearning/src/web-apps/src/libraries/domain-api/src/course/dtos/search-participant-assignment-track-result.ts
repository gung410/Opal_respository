import { IParticipantAssignmentTrack, ParticipantAssignmentTrack } from '../models/participant-assignment-track.model';

export interface ISearchParticipantAssignmentTrackResult {
  items: IParticipantAssignmentTrack[];
  totalCount?: number;
}

export class ParticipantAssignmentTrackResult {
  public items: ParticipantAssignmentTrack[] = [];
  public totalCount?: number = 0;

  constructor(data?: ISearchParticipantAssignmentTrackResult) {
    if (data == null) {
      return;
    }

    this.items = data.items ? data.items.map(item => new ParticipantAssignmentTrack(item)) : [];
    this.totalCount = data.totalCount;
  }
}
