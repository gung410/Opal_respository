import { UpdateTaxonomyRequestItemRequest } from './update-taxonomy-request-Item-request.dto';

export interface IUpdateTaxonomyRequestRequest {
  taxonomyRequestId: string;
  item: UpdateTaxonomyRequestItemRequest;
  comment: string;
}

export class UpdateTaxonomyRequestRequest
  implements IUpdateTaxonomyRequestRequest {
  taxonomyRequestId: string;
  item: UpdateTaxonomyRequestItemRequest;
  comment: string;

  constructor(data?: IUpdateTaxonomyRequestRequest) {
    if (!data) {
      return;
    }

    this.taxonomyRequestId = data.taxonomyRequestId;
    this.item = data.item;
    this.comment = data.comment;
  }
}
