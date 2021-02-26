import { TaxonomyRequestStatus } from '../constant/taxonomy-request-status.enum';

export class TaxonomyRequestStatusType {
  pendingLevel1: TaxonomyRequestStatus;
  pendingLevel2: TaxonomyRequestStatus;
  rejectLevel1: TaxonomyRequestStatus;
  rejectLevel2: TaxonomyRequestStatus;
  approved: TaxonomyRequestStatus;
  completed: TaxonomyRequestStatus;
}
