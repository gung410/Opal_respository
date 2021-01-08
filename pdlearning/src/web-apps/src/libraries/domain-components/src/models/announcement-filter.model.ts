import { DateUtils, IContainFilter, IFilter, IFromToFilter } from '@opal20/infrastructure';

export class AnnouncementFilterModel {
  public fromCreatedDate?: Date = undefined;
  public toCreatedDate?: Date = undefined;

  public convert(): IFilter {
    const containFilters: IContainFilter[] = [];
    const fromToFilters: IFromToFilter[] = [];

    fromToFilters.push({
      field: 'CreatedDate',
      fromValue: this.fromCreatedDate ? DateUtils.removeTime(this.fromCreatedDate).toLocaleString() : null,
      toValue: this.toCreatedDate ? DateUtils.setTimeToEndInDay(this.toCreatedDate).toLocaleString() : null,
      equalFrom: true,
      equalTo: true
    });

    return {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };
  }
}
