import { TaxonomyRequestActionEnum } from '../constant/taxonomy-request-action-type.enum';
import { TaxonomyRequestStatus } from '../constant/taxonomy-request-status.enum';
import { TaxonomyRequest } from './taxonomy-request.model';

export interface ITaxonomyRequestViewModel {
  taxonomyRequestId: string;
  createdBy: string;
  createdDate: Date;
  status: TaxonomyRequestStatus;
  level1ApprovalOfficerId?: string;
  level2ApprovalOfficerId?: string;
}

export class TaxonomyRequestViewModel implements ITaxonomyRequestViewModel {
  taxonomyRequestId: string;
  createdBy: string;
  createdDate: Date;
  status: TaxonomyRequestStatus;
  level1ApprovalOfficerId?: string;
  level2ApprovalOfficerId?: string;
  taxonomyRequestItemId: string;
  nodeId: string;
  metadataType: string;
  metadataName: string;
  oldMetadataName: string;
  pathName: string;
  pathIds: string[];
  reason: string;
  abbreviation: string;
  type: TaxonomyRequestActionEnum;

  constructor(data?: TaxonomyRequest) {
    if (data == null) {
      return;
    }

    this.taxonomyRequestId = data.id;
    this.createdBy = data.createdBy;
    this.createdDate = data.createdDate;
    this.status = data.status;
    this.level1ApprovalOfficerId = data.level1ApprovalOfficerId;
    this.level2ApprovalOfficerId = data.level2ApprovalOfficerId;
  }
}
