export class EmailModel {
  body?: string;
  subject?: string;
  emails?: string[];
  isHtmlEmail?: boolean;
  templateData?: TemplateModel;

  constructor(data?: any) {
    this.body = data ? data.body : '';
    this.subject = data ? data.subject : '';
    this.emails = data ? data.emails : [];
    this.isHtmlEmail = data ? data.isHtmlEmail : true;
    this.templateData = data ? data.templateData : {};
  }
}

export class TemplateModel {
  project?: string;
  module?: string;
  templateName?: string;
  data?: any;
  constructor(item?: any) {
    this.project = item ? item.project : '';
    this.module = item ? item.module : '';
    this.templateName = item ? item.templateName : '';
    this.data = item ? item.data : '';
  }
}
