import { IContainFilter, IFilter, IFromToFilter } from '@opal20/infrastructure';

export class DigitalBadgesFilterModel {
  public convert(): IFilter {
    const containFilters: IContainFilter[] = [];
    const fromToFilters: IFromToFilter[] = [];

    return {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };
  }
}
