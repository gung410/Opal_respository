export interface ICommunity {
  id: string;
  title: string;
  subCommunities: ICommunity[];
}

export class Community implements ICommunity {
  public id: string;
  public title: string;
  public subCommunities: ICommunity[];

  constructor(data: ICommunity) {
    this.id = data.id;
    this.title = data.title;
    this.subCommunities = data.subCommunities || [];
  }
}
