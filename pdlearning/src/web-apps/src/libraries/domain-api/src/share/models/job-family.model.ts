export interface IJobFamily {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class JobFamily implements IJobFamily {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: IJobFamily) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
