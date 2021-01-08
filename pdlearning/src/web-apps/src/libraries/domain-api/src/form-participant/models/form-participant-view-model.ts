import { FormParticipant, IFormParticipant } from './form-participant';

import { IGridDataItem } from '@opal20/infrastructure';
import { IPublicUserInfo } from '../../share/models/user-info.model';

export interface IFormParticipantViewModel extends IFormParticipant {
  participant: IPublicUserInfo;
}

export class FormParticipantViewModel extends FormParticipant implements IGridDataItem {
  public type: string;
  public id: string;
  public selected: boolean;
  public participant: IPublicUserInfo;
  public get attendance(): string {
    return `${this.isStarted ? 1 : 0}/1`;
  }
  public static createFromParticipantModel(formParticipant: IFormParticipant, participant: IPublicUserInfo): FormParticipantViewModel {
    return new FormParticipantViewModel({
      ...formParticipant,
      participant: participant
    });
  }

  constructor(data?: IFormParticipantViewModel) {
    super(data);
    if (data != null) {
      this.participant = data.participant;
    }
  }
}
