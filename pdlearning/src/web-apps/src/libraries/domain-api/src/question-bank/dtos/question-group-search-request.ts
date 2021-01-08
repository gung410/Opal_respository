import { IPagedInfo } from '../../content/models/paged-info';

export interface IQuestionGroupSearchRequest {
  name: string;
  isFilterByUsing: boolean;
  pagedInfo: IPagedInfo;
}
