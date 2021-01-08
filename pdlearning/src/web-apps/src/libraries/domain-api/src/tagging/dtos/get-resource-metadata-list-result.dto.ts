import { IResourceMetadataModel, ResourceMetadataModel } from '../models/resource-metadata';

export interface IGetResourceMetadataListResult {
  items: IResourceMetadataModel[];
}

export class GetResourceMetadataListResult implements IGetResourceMetadataListResult {
  public items: ResourceMetadataModel[];
  constructor(data?: IGetResourceMetadataListResult) {
    if (data != null) {
      this.items = data.items.map(_ => new ResourceMetadataModel(_));
    }
  }
}
