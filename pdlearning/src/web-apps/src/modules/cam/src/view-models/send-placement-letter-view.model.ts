import { SendAnnouncemmentEmailTemplateModel } from '@opal20/domain-api';
import { Utils } from '@opal20/infrastructure';

export class SendPlacementLetterViewModel {
  private _mailContent: string = '';

  constructor(public defaultTemplate: SendAnnouncemmentEmailTemplateModel = new SendAnnouncemmentEmailTemplateModel()) {
    this._mailContent = defaultTemplate.template;
  }

  public get mailContent(): string {
    return this._mailContent;
  }
  public set mailContent(mailContent: string) {
    this._mailContent = mailContent;
  }

  public reset(): SendPlacementLetterViewModel {
    return Utils.clone(this, p => {
      p.mailContent = p.defaultTemplate.template;
    });
  }
}
