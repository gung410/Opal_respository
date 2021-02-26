import { TaxonomyRequestStatus } from '../constant/taxonomy-request-status.enum';

export interface IGetMetadataSuggestionsRequest {
  searchByRequestTitle: string;
  statuses: TaxonomyRequestStatus[];
  skipCount: number;
  maxResultCount: number;
}

export class GetMetadataSuggestionsRequest
  implements IGetMetadataSuggestionsRequest {
  searchByRequestTitle: string;
  statuses: TaxonomyRequestStatus[];
  skipCount: number;
  maxResultCount: number;

  constructor(data?: IGetMetadataSuggestionsRequest) {
    if (data == null) {
      return;
    }

    this.searchByRequestTitle = data.searchByRequestTitle;
    this.statuses = data.statuses;
    this.skipCount = data.skipCount;
    this.maxResultCount = data.maxResultCount;
  }
}
