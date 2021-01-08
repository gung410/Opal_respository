export interface ITypeOfOrganization {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class TypeOfOrganization implements ITypeOfOrganization {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: ITypeOfOrganization) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
