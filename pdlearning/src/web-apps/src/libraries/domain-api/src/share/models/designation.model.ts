export interface IDesignation {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class Designation implements IDesignation {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: IDesignation) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
