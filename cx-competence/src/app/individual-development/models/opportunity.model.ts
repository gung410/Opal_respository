import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';

export class PDCatalogCourseModel {
  course: PDOpportunityDetailModel;
  isSelected: boolean;
  isBookmarked?: boolean;
  constructor(data?: Partial<PDCatalogCourseModel>) {
    if (!data) {
      return;
    }
    this.course = data.course ? data.course : undefined;
    this.isSelected = !!data.isSelected;
    this.isBookmarked = !!data.isBookmarked;
  }
}
