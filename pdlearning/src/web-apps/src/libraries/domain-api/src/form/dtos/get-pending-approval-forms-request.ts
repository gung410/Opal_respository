import { FormModel, IFormModel } from '../models/form.model';

export interface IGetPendingApprovalFormsRequest {
  pagedInfo: { skipCount: number; maxResultCount: number };
}

export interface IGetPendingApprovalFormsResponse {
  totalCount: number;
  items: IFormModel[];
}

export class GetPendingApprovalFormsResponseResponse {
  public totalCount: number;
  public items: FormModel[];

  constructor(data?: IGetPendingApprovalFormsResponse) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.items = data.items.map(p => new FormModel(p));
    }
  }
}
