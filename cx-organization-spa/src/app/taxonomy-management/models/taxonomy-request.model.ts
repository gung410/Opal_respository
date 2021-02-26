import { TaxonomyRequestStatus } from '../constant/taxonomy-request-status.enum';
import { TaxonomyRequestItem } from './taxonomy-request-item.model';

export interface ITaxonomyRequest {
  id: string;
  createdBy: string;
  createdDate: Date;
  status: TaxonomyRequestStatus;
  level1ApprovalOfficerId?: string;
  level2ApprovalOfficerId?: string;
  level1ApprovalOfficerComment?: string;
  level2ApprovalOfficerComment?: string;
  taxonomyRequestItems: TaxonomyRequestItem[];
}

export class TaxonomyRequest implements ITaxonomyRequest {
  id: string;
  createdBy: string;
  createdDate: Date;
  status: TaxonomyRequestStatus;
  level1ApprovalOfficerId?: string;
  level2ApprovalOfficerId?: string;
  level1ApprovalOfficerComment?: string;
  level2ApprovalOfficerComment?: string;
  taxonomyRequestItems: TaxonomyRequestItem[];

  constructor(data?: ITaxonomyRequest) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.createdBy = data.createdBy;
    this.createdDate = data.createdDate;
    this.status = data.status;
    this.level1ApprovalOfficerComment = data.level1ApprovalOfficerComment;
    this.level2ApprovalOfficerComment = data.level2ApprovalOfficerComment;
    this.level1ApprovalOfficerId = data.level1ApprovalOfficerId;
    this.level2ApprovalOfficerId = data.level2ApprovalOfficerId;
    this.taxonomyRequestItems = data.taxonomyRequestItems;
  }
}
