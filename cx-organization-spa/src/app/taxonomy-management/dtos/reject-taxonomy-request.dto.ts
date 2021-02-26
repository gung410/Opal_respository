import { UpdateTaxonomyRequestItemRequest } from './update-taxonomy-request-Item-request.dto';
import { IUpdateTaxonomyRequestRequest } from './update-taxonomy-request-request.dto';

export interface IRejectTaxonomyRequest extends IUpdateTaxonomyRequestRequest {}

export class RejectTaxonomyRequest implements IRejectTaxonomyRequest {
  taxonomyRequestId: string;
  item: UpdateTaxonomyRequestItemRequest;
  comment: string;

  constructor(data?: IRejectTaxonomyRequest) {
    if (data == null) {
      return;
    }

    this.taxonomyRequestId = data.taxonomyRequestId;
    this.item = data.item;
    this.comment = data.comment;
  }
}
