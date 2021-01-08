export interface ISelectDataModel {
  id: string;
  displayText: string;
}

export class SelectDataModel implements ISelectDataModel {
  public id: string;
  public displayText: string;

  constructor(data?: ISelectDataModel) {
    if (data) {
      this.id = data.id;
      this.displayText = data.displayText;
    }
  }
}
