import { DateUtils, IContainFilter, IFilter, IFromToFilter } from '@opal20/infrastructure';

export class BlockoutDateFilterModel {
  public createdDateFrom?: Date;
  public createdDateTo?: Date;
  public serviceSchemes?: string[];

  public convert(): IFilter {
    const containFilters: IContainFilter[] = [];
    const fromToFilters: IFromToFilter[] = [];

    containFilters.push({
      field: 'ServiceSchemes',
      values: this.serviceSchemes,
      notContain: false
    });

    fromToFilters.push({
      field: 'StartDate',
      fromValue: this.createdDateFrom ? DateUtils.removeTime(this.createdDateFrom).toLocaleString() : null,
      toValue: null,
      equalFrom: true,
      equalTo: true
    });

    fromToFilters.push({
      field: 'EndDate',
      fromValue: null,
      toValue: this.createdDateTo ? DateUtils.setTimeToEndInDay(this.createdDateTo).toLocaleString() : null,
      equalFrom: true,
      equalTo: true
    });

    return {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };
  }
}
