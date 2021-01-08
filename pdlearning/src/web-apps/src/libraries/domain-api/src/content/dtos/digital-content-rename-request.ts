export interface IDigitalContentRenameRequest {
  id: string;
  title: string;
}

export class DigitalContentRenameRequest implements IDigitalContentRenameRequest {
  public id: string;
  public title: string;

  constructor(data?: IDigitalContentRenameRequest) {
    if (data != null) {
      this.id = data.id;
      this.title = data.title;
    }
  }
}
