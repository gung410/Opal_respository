export interface IEmailTemplateModel {
  id: string;
  template: string;
  tags?: Dictionary<string>;
}

export class EmailTemplateModel implements IEmailTemplateModel {
  public id: string = '';
  public template: string = '';
  public tags: Dictionary<string> = {};

  constructor(data?: IEmailTemplateModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.template = data.template;
    this.tags = data.tags ? data.tags : {};
  }
}
