import { AnnouncementTemplate, IAnnouncementTemplate } from '../models/announcement-template.model';

export interface ISearchAnnouncementTemplateResult {
  items: IAnnouncementTemplate[];
  totalCount: number;
}

export class SearchAnnouncementTemplateResult implements ISearchAnnouncementTemplateResult {
  public items: AnnouncementTemplate[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAnnouncementTemplateResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new AnnouncementTemplate(item));
    this.totalCount = data.totalCount;
  }
}
