import { CommunityResultModel, ICommunityResultModel } from '../models/csl-community.model';
import { ICslPagedResultDto, IPagedResultDto } from '../../share/dtos/paged-result.dto';

export class CSLCommunityResults implements IPagedResultDto<CommunityResultModel> {
  public totalCount: number = 0;
  public items: CommunityResultModel[] = [];

  public static createCSLCommunityResults(resCommunity: ICslPagedResultDto<CommunityResultModel>): CSLCommunityResults {
    return new CSLCommunityResults({
      totalCount: resCommunity.total,
      items: resCommunity.results
    });
  }

  constructor(data?: IPagedResultDto<ICommunityResultModel>) {
    if (data == null) {
      return;
    }

    this.items = this.items ? data.items.map(item => new CommunityResultModel(item)) : [];
    this.totalCount = data.totalCount;
  }
}
