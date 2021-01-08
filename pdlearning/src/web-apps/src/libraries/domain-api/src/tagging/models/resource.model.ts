export interface IResourceModel {
  resourceId: string;
  resourceType: ResourceType;
  mainSubjectAreaTagId?: string;
  preRequisties?: string;
  objectivesOutCome?: string;
  tags: string[];
  searchTags: string[];
  dynamicMetaData?: Dictionary<unknown>;
}

export class ResourceModel implements IResourceModel {
  public resourceId: string = '';
  public resourceType: ResourceType = ResourceType.Content;
  public mainSubjectAreaTagId?: string;
  public preRequisties?: string;
  public objectivesOutCome?: string;
  public tags: string[] = [];
  public searchTags: string[] = [];
  public dynamicMetaData?: Dictionary<unknown> = {};

  constructor(data?: IResourceModel) {
    if (data) {
      this.resourceId = data.resourceId;
      this.resourceType = data.resourceType;
      this.mainSubjectAreaTagId = data.mainSubjectAreaTagId;
      this.preRequisties = data.preRequisties;
      this.objectivesOutCome = data.objectivesOutCome;
      this.tags = data.tags;
      this.searchTags = data.searchTags;
      this.dynamicMetaData = data.dynamicMetaData;
    }
  }
}

export enum ResourceType {
  Content = 'Content',
  Course = 'Course',
  Community = 'Community'
}
