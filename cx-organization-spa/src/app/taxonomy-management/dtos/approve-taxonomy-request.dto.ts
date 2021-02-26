import { UpdateTaxonomyRequestItemRequest } from './update-taxonomy-request-Item-request.dto';
import { IUpdateTaxonomyRequestRequest } from './update-taxonomy-request-request.dto';

export interface IApproveTaxonomyRequest
  extends IUpdateTaxonomyRequestRequest {}

export class ApproveTaxonomyRequest implements IApproveTaxonomyRequest {
  taxonomyRequestId: string;
  item: UpdateTaxonomyRequestItemRequest;
  comment: string;

  constructor(data?: IApproveTaxonomyRequest) {
    if (data == null) {
      return;
    }

    this.taxonomyRequestId = data.taxonomyRequestId;
    this.item = data.item;
    this.comment = data.comment;
  }
}
