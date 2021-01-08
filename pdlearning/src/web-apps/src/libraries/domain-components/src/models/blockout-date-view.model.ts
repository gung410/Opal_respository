import { BlockoutDateModel, IBlockoutDateModel, MetadataTagModel } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';
export interface IBlockoutDateViewModel extends IBlockoutDateModel {
  selected: boolean;
  startDateTime?: Date;
  endDateTime?: Date;
  serviceSchemesDisplayText: string[];
}

export class BlockoutDateViewModel extends BlockoutDateModel implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public serviceSchemesDisplayText: string[] = [];
  public startDateTime?: Date;
  public endDateTime?: Date;

  public static createFromModel(
    blockoutDate: BlockoutDateModel,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean>,
    metadataTagDict: Dictionary<MetadataTagModel>
  ): BlockoutDateViewModel {
    return new BlockoutDateViewModel({
      ...blockoutDate,
      selected: checkAll || selecteds[blockoutDate.id],
      serviceSchemesDisplayText: blockoutDate.serviceSchemes.map(p => metadataTagDict[p].displayText)
    });
  }

  constructor(data?: IBlockoutDateViewModel) {
    super(data);
    if (data != null) {
      this.id = data.id;
      this.selected = data.selected;
      this.startDateTime = new Date(data.validYear, data.startMonth - 1, data.startDay, 0, 0, 1, 0);
      this.endDateTime = new Date(data.validYear, data.endMonth - 1, data.endDay, 23, 59, 59, 0);
      this.serviceSchemesDisplayText = data.serviceSchemesDisplayText;
      this.title = data.title;
      this.description = data.description;
      this.createdBy = data.createdBy;
      this.startDay = data.startDay;
      this.startMonth = data.startMonth;
      this.endDay = data.endDay;
      this.endMonth = data.endMonth;
      this.validYear = data.validYear;
      this.planningCycleId = data.planningCycleId;
      this.serviceSchemes = data.serviceSchemes;
      this.status = data.status;
      this.isConfirmed = data.isConfirmed;
    }
  }
}
