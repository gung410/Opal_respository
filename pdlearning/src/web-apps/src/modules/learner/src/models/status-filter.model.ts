export interface IStatusFilterModel {
  status: string;
  displayText: string;
}

export class StatusFilterModel implements IStatusFilterModel {
  public status: string;
  public displayText: string;

  constructor(data?: Partial<IStatusFilterModel>) {
    if (data == null) {
      return;
    }
    this.status = data.status;
    this.displayText = data.displayText;
  }
}
