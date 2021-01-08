export interface IRoleSpecificProficiency {
  id: string;
  displayText: string;
  fullStatement: string;
}

export class RoleSpecificProficiency implements IRoleSpecificProficiency {
  public id: string;
  public displayText: string;
  public fullStatement: string;

  constructor(data?: IRoleSpecificProficiency) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
      this.fullStatement = data.fullStatement;
    }
  }
}
