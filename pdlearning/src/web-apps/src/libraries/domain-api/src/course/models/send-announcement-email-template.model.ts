import { EmailTemplateModel, IEmailTemplateModel } from './email-template.model';

export interface ISendAnnouncemmentEmailTemplateModel extends IEmailTemplateModel {
  userNameTagValue?: string;
  courseTitleTagValue?: string;
  courseCodeTagValue?: string;
  courseAdminNameTagValue?: string;
  courseAdminEmailTagValue?: string;
  listSessionTagValue?: string;
  detailUrlTagValue?: string;
}

export class SendAnnouncemmentEmailTemplateModel extends EmailTemplateModel implements ISendAnnouncemmentEmailTemplateModel {
  public userNameTagValue?: string;
  public courseTitleTagValue?: string;
  public courseCodeTagValue?: string;
  public courseAdminNameTagValue?: string;
  public courseAdminEmailTagValue?: string;
  public listSessionTagValue?: string;
  public detailUrlTagValue?: string;

  constructor(data?: ISendAnnouncemmentEmailTemplateModel) {
    super(data);

    if (data == null) {
      return;
    }
    this.userNameTagValue = data.userNameTagValue;
    this.courseTitleTagValue = data.courseTitleTagValue;
    this.courseCodeTagValue = data.courseCodeTagValue;
    this.courseAdminNameTagValue = data.courseAdminNameTagValue;
    this.courseAdminEmailTagValue = data.courseAdminEmailTagValue;
    this.listSessionTagValue = data.listSessionTagValue;
    this.detailUrlTagValue = data.detailUrlTagValue;
  }
}
