import { IRegistration, Registration } from '../models/registrations.model';

export interface ISearchRegistrationResult {
  items: IRegistration[];
  totalCount: number;
}

export class SearchRegistrationResult implements ISearchRegistrationResult {
  public items: Registration[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchRegistrationResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Registration(item));
    this.totalCount = data.totalCount;
  }
}
