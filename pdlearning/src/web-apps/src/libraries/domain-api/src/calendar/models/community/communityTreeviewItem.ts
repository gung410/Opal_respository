import { Community, ICommunity } from './community';

export interface ICommunityTreeviewItem extends ICommunity {
  checked: boolean;
}

export class CommunityTreeviewItem extends Community implements ICommunityTreeviewItem {
  public checked: boolean = true;
  public id: string;
  public title: string;
  public subCommunities: ICommunityTreeviewItem[];

  constructor(data: ICommunityTreeviewItem) {
    super(data);
    this.checked = data.checked || true;

    this.subCommunities = this.subCommunities.map(s => {
      s.checked = s.checked || true;
      return s;
    });
  }
}
