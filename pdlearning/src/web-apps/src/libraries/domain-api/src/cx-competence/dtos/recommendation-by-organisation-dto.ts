import { ICreateActionItemResultRequest } from './create-action-item-result-request';

export interface IRecommendationByOrganisationItemResult {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  hasMoreData: boolean;
  items: ICreateActionItemResultRequest[];
}
