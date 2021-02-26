import { TaxonomyRequestStatusLabel } from '../constant/taxonomy-request-status-label.enum';

export class TaxonomyRequestStatusLabelType {
  pendingLevel1: TaxonomyRequestStatusLabel;
  pendingLevel2: TaxonomyRequestStatusLabel;
  rejected: TaxonomyRequestStatusLabel;
  approved: TaxonomyRequestStatusLabel;
  completed: TaxonomyRequestStatusLabel;
}
