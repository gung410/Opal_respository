export interface IAreaOfProfessionalInterest {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class AreaOfProfessionalInterest implements IAreaOfProfessionalInterest {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: IAreaOfProfessionalInterest) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
