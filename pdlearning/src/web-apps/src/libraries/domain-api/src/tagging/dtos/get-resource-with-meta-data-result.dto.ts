import { IMetadataTagModel, MetadataTagModel } from '../models/metadata-tag.model';
import { IResourceModel, ResourceModel } from '../models/resource.model';

export interface IGetResourceWithMetadataResult {
  resource: IResourceModel;
  metadataTags: IMetadataTagModel[];
}

export class GetResourceWithMetadataResult implements IGetResourceWithMetadataResult {
  public resource: ResourceModel = new ResourceModel();
  public metadataTags: MetadataTagModel[] = [];

  constructor(data?: IGetResourceWithMetadataResult) {
    if (data != null) {
      this.resource = new ResourceModel(data.resource);
      this.metadataTags = data.metadataTags.map(p => new MetadataTagModel(p));
    }
  }
}
