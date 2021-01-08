import { ActionItemResponse, IActionItemResponse } from './action-item-response.dto';

import { IPagedCxCompetenceModel } from '../models/cx-competence-paging-model';

export interface IIdpPlanPdos extends IPagedCxCompetenceModel<IActionItemResponse> {}

export class IdpPlanPdos implements IIdpPlanPdos {
  public totalItems: number;
  public pageIndex: number;
  public pageSize: number;
  public hasMoreData: boolean;
  public items: IActionItemResponse[];
  constructor(data?: IIdpPlanPdos) {
    if (data == null) {
      return;
    }
    this.totalItems = data.totalItems;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.hasMoreData = data.hasMoreData;
    this.items = data.items ? data.items.map(_ => new ActionItemResponse(_)) : [];
  }
}
