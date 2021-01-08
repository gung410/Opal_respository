export interface ISaveResourceMetadataRequest {
  tagIds: string[];
  mainSubjectAreaTagId?: string;
  preRequisties?: string;
  objectivesOutCome?: string;
  searchTags?: string[];
  dynamicMetaData?: Dictionary<unknown>;
}
