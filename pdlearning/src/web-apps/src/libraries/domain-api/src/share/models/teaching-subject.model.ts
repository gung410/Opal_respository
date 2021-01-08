export interface ITeachingSubject {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class TeachingSubject implements ITeachingSubject {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: ITeachingSubject) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
