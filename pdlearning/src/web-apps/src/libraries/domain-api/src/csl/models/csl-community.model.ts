import { LogicOperator } from './../../learning-catalog/models/catalog-search-results.model';

export interface ICommunityOnwerModel {
  guid?: string;
  display_name?: string;
  url: string;
}

export class CommunityOnwerModel implements ICommunityOnwerModel {
  public id: number;
  public guid?: string;
  // tslint:disable-next-line:variable-name
  public display_name?: string;
  public url: string;
  constructor(data?: CommunityOnwerModel) {
    if (data != null) {
      this.id = data.id;
      this.guid = data.guid;
      this.display_name = data.display_name;
      this.url = data.url;
    }
  }
}

export interface ICommunityResultModel {
  guid: string;
  ext_guid?: string;
  name: string;
  description: string;
  url: string;
  members: number;
  isBookmark: boolean;
  owner?: ICommunityOnwerModel;
}

export class CommunityResultModel implements ICommunityResultModel {
  public guid: string = '';
  // tslint:disable-next-line:variable-name
  public ext_guid?: string = '';
  public name: string = '';
  public description: string = '';
  public url: string = '';
  public members: number = 0;
  public isBookmark: boolean = false;
  public owner = undefined;

  constructor(data?: ICommunityResultModel) {
    if (!data) {
      return;
    }

    this.guid = data.guid;
    this.ext_guid = data.ext_guid;
    this.name = data.name;
    this.description = data.description;
    this.url = data.url;
    this.members = data.members;
    this.isBookmark = data.isBookmark;
    this.owner = data.owner;
  }

  public hasValue(term: string, filterFn: (item: CommunityResultModel) => string): boolean {
    const value = filterFn(this);
    return value && value.toLowerCase().includes(term.toLowerCase());
  }
}

export interface ICommunityRequest {
  currentUser?: string;
  searchingText?: string;
  maxResultCount: number;
  skipCount: number;
  filterType: string;
  tagIds?: string[];
  searchOrByType?: { [propOrTagName: string]: [LogicOperator, ...string[]] };
}

export enum FilterCommunityStatus {
  All = 'All',
  Joined = 'My communities',
  Owned = 'My own communities'
}

export enum CommunityStatus {
  disabled = 'disabled',
  enabled = 'enabled',
  archived = 'archived'
}
