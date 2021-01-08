import { IMetadataTagModel, MetadataTagModel } from './metadata-tag.model';

import { MetadataTagGroupCode } from './metadata-tag-group-code.enum';

export interface IResourceMetadataModel {
  resourceId: string;
  metadatas: IMetadataTagModel[];
}

export class ResourceMetadataModel implements IResourceMetadataModel {
  public resourceId: string = '';
  public metadatas: MetadataTagModel[] = [];

  constructor(data?: IResourceMetadataModel) {
    if (data != null) {
      this.resourceId = data.resourceId;
      this.metadatas = data.metadatas.map(_ => new MetadataTagModel(_));
    }
  }

  public getMetaData<T>(groupCode: MetadataTagGroupCode, valueFn: (item: MetadataTagModel) => T): T | null {
    const foundFirstItem = this.metadatas.find(_ => _.groupCode === groupCode);
    return foundFirstItem != null ? valueFn(foundFirstItem) : null;
  }
}
