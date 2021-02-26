import { TaxonomyRequestActionEnum } from '../constant/taxonomy-request-action-type.enum';

export interface ITaxonomyRequestItem {
  id: string;
  taxonomyRequestId: string;
  nodeId: string;
  metadataType: string;
  metadataName: string;
  oldMetadataName: string;
  pathName: string;
  pathIds: string[];
  reason: string;
  abbreviation: string;
  type: TaxonomyRequestActionEnum;
}

export class TaxonomyRequestItem implements ITaxonomyRequestItem {
  id: string;
  taxonomyRequestId: string;
  nodeId: string;
  metadataType: string;
  metadataName: string;
  oldMetadataName: string;
  pathName: string;
  pathIds: string[];
  reason: string;
  abbreviation: string;
  type: TaxonomyRequestActionEnum;

  constructor(data?: ITaxonomyRequestItem) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.taxonomyRequestId = data.taxonomyRequestId;
    this.nodeId = data.nodeId;
    this.metadataType = data.metadataType;
    this.metadataName = data.metadataName;
    this.oldMetadataName = data.oldMetadataName;
    this.pathName = data.pathName;
    this.pathIds = data.pathIds;
    this.reason = data.reason;
    this.abbreviation = data.abbreviation;
    this.type = data.type;
  }
}
