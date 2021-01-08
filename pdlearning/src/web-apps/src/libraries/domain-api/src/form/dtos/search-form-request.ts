import { FormModel, FormStatus, FormSurveyType, FormType, IFormModel } from '../models/form.model';

export class SearchFormRequest {
  public pagedInfo: { skipCount: number; maxResultCount: number };
  public searchFormTitle: string | undefined;
  public filterByStatus: FormStatus[];

  public includeFormForImportToCourse: boolean = false;
  public filterByType?: FormType = null;
  public filterBySurveyTypes: FormSurveyType[] = [];
  public excludeBySurveyTypes?: FormSurveyType[] = [];
  public isSurveyTemplate?: boolean = false;
}

export interface ISearchFormResponse {
  totalCount: number;
  items: IFormModel[];
}

export class SearchFormResponse {
  public totalCount: number;
  public items: FormModel[];

  constructor(data?: ISearchFormResponse) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.items = data.items.map(p => new FormModel(p));
    }
  }
}
