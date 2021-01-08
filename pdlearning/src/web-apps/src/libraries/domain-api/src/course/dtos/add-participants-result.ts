export interface IAddParticipantsResult {
  numberOfAddedParticipants: number;
  totalNumberOfUsers: number;
}

export class AddParticipantsResult implements IAddParticipantsResult {
  public numberOfAddedParticipants: number;
  public totalNumberOfUsers: number;

  constructor(data?: IAddParticipantsResult) {
    if (data == null) {
      return;
    }

    this.numberOfAddedParticipants = data.numberOfAddedParticipants;
    this.totalNumberOfUsers = data.numberOfAddedParticipants;
  }
}
