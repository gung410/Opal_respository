import { ISession, Session } from '../models/session.model';

export interface ISearchSessionResult {
  items: ISession[];
  totalCount: number;
}

export class SearchSessionResult implements ISearchSessionResult {
  public items: Session[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchSessionResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Session(item));
    this.totalCount = data.totalCount;
  }
}
