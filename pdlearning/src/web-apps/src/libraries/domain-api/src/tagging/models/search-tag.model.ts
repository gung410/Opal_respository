export interface ISearchTag {
  id?: string;
  name: string;
}

export class SearchTag implements ISearchTag {
  public id: string;
  public name: string;

  constructor(data: ISearchTag) {
    this.id = data.id;
    this.name = data.name;
  }
}
