import { BaseUserInfo, SendAnnouncemmentEmailTemplateModel } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class SendOrderRefreshmentViewModel {
  private _sendToEmails: string[] = [];
  private _emailContent: string = '';
  private _emailSubject: string = '';
  private _emailCC: string[] = [];

  constructor(
    public defaultTemplate: SendAnnouncemmentEmailTemplateModel = new SendAnnouncemmentEmailTemplateModel(),
    public courseTitle?: string,
    public users?: BaseUserInfo[]
  ) {
    this._emailContent = defaultTemplate.template;
    if (users != null) {
      this._emailCC = users.map(x => x.emailAddress);
    }
    this._emailSubject = `Refreshment for ${this.courseTitle}`;
  }

  public get sendToEmails(): string[] {
    return this._sendToEmails;
  }
  public set sendToEmails(sendToEmails: string[]) {
    this._sendToEmails = sendToEmails;
  }

  public get emailSubject(): string {
    return this._emailSubject;
  }
  public set emailSubject(emailSubject: string) {
    this._emailSubject = emailSubject;
  }

  public get emailCC(): string[] {
    return this._emailCC;
  }
  public set emailCC(emailCC: string[]) {
    this._emailCC = emailCC;
  }

  public get emailContent(): string {
    return this._emailContent;
  }
  public set emailContent(mailContent: string) {
    this._emailContent = mailContent;
  }

  public reset(): SendOrderRefreshmentViewModel {
    return Utils.clone(this, p => {
      p._emailContent = p.defaultTemplate.template;
    });
  }

  public checkReceiveValidation(): boolean {
    if (this.emailCC.length === 0 || this.sendToEmails.length === 0 || this.emailCC == null || this.sendToEmails == null) {
      return false;
    }
    return true;
  }
}
