import { TaxonomyRequestItem } from '../models/taxonomy-request-item.model';

export interface ITaxonomyCreationRequest {
  level1ApprovalOfficerId?: string;
  taxonomyRequestItems: TaxonomyRequestItem[];
}

export class TaxonomyCreationRequest implements ITaxonomyCreationRequest {
  level1ApprovalOfficerId?: string;
  taxonomyRequestItems: TaxonomyRequestItem[];

  constructor(data?: ITaxonomyCreationRequest) {
    if (data == null) {
      return;
    }

    this.level1ApprovalOfficerId = data.level1ApprovalOfficerId;
    this.taxonomyRequestItems = data.taxonomyRequestItems;
  }
}
