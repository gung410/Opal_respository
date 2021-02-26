export interface IUpdateTaxonomyRequestItemRequest {
  taxonomyRequestItemId: string;
  nodeId: string;
  metadataName: string;
  reason: string;
  abbreviation: string;
}

export class UpdateTaxonomyRequestItemRequest
  implements IUpdateTaxonomyRequestItemRequest {
  taxonomyRequestItemId: string;
  nodeId: string;
  metadataName: string;
  reason: string;
  abbreviation: string;

  constructor(data?: IUpdateTaxonomyRequestItemRequest) {
    if (data == null) {
      return;
    }

    this.taxonomyRequestItemId = data.taxonomyRequestItemId;
    this.nodeId = data.nodeId;
    this.metadataName = data.metadataName;
    this.reason = data.reason;
    this.abbreviation = data.abbreviation;
  }
}
