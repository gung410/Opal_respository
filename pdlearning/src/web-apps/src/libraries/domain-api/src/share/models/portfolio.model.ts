export interface IPortfolio {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class Portfolio implements IPortfolio {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: IPortfolio) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
