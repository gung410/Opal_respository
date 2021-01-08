import { BlockoutDateModel, IBlockoutDateModel } from './blockout-date.model';

export interface IGetBlockoutDateDependenciesModel {
  matchedBlockoutDates: IBlockoutDateModel[];
  matchedFromDateBlockoutDates: IBlockoutDateModel[];
  matchedToDateBlockoutDates: IBlockoutDateModel[];
}

export class GetBlockoutDateDependenciesModel implements IGetBlockoutDateDependenciesModel {
  public matchedBlockoutDates: BlockoutDateModel[] = [];
  public matchedFromDateBlockoutDates: BlockoutDateModel[] = [];
  public matchedToDateBlockoutDates: BlockoutDateModel[] = [];

  constructor(data?: IGetBlockoutDateDependenciesModel) {
    if (data) {
      this.matchedBlockoutDates = data.matchedBlockoutDates.map(p => new BlockoutDateModel(p));
      this.matchedFromDateBlockoutDates = data.matchedFromDateBlockoutDates.map(p => new BlockoutDateModel(p));
      this.matchedToDateBlockoutDates = data.matchedToDateBlockoutDates.map(p => new BlockoutDateModel(p));
    }
  }
}
