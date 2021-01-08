export interface ITeachingLevel {
  id: string;
  displayText: string;
  fullStatement: string;
  abbreviatedStatement: string;
}

export class TeachingLevel implements ITeachingLevel {
  public id: string;
  public displayText: string;
  public fullStatement: string;
  public abbreviatedStatement: string;

  constructor(data?: ITeachingLevel) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
      this.abbreviatedStatement = data.abbreviatedStatement;
    }
  }
}
