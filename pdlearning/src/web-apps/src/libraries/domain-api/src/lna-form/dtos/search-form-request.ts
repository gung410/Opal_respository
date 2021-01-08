import { IStandaloneSurveyModel, StandaloneSurveyModel, SurveyStatus } from '../models/lna-form.model';

export class SearchSurveyRequest {
  public pagedInfo: { skipCount: number; maxResultCount: number };
  public searchFormTitle: string | undefined;
  public filterByStatus: SurveyStatus[];

  public includeFormForImportToCourse: boolean = false;
}

export interface ISearchSurveyResponse {
  totalCount: number;
  items: IStandaloneSurveyModel[];
}

export class SearchSurveyResponse {
  public totalCount: number;
  public items: StandaloneSurveyModel[];

  constructor(data?: ISearchSurveyResponse) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.items = data.items.map(p => new StandaloneSurveyModel(p));
    }
  }
}
