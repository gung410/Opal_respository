import { FormParticipant, IFormParticipant } from '../models/form-participant';

export interface IFormParticipantSearchResult {
  totalCount: number;
  items: IFormParticipant[];
}

export class FormParticipantSearchResult implements IFormParticipantSearchResult {
  public totalCount: number;
  public items: FormParticipant[];

  constructor(data: IFormParticipantSearchResult) {
    if (!data) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items.map(p => new FormParticipant(p));
  }
}
