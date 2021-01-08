import { DigitalContentSortField } from './digital-content-sort-field.model';
import { SortDirection } from '../../share/dtos/sort-direction';

export interface DigitalContentSortMode {
  sortField: DigitalContentSortField;
  sortDirection: SortDirection;
}

export interface DigitalContentSortItem {
  id: number;
  text: string;
  sortMode: DigitalContentSortMode;
}

export const DIGITAL_CONTENT_SORT_ITEMS: DigitalContentSortItem[] = [
  {
    id: 1,
    text: 'Recently',
    sortMode: {
      sortField: DigitalContentSortField.ChangedDate,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 2,
    text: 'Top rated',
    sortMode: {
      sortField: DigitalContentSortField.AverageRating,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 3,
    text: 'Lowest rated',
    sortMode: {
      sortField: DigitalContentSortField.AverageRating,
      sortDirection: SortDirection.Ascending
    }
  },
  {
    id: 4,
    text: 'Most reviewed',
    sortMode: {
      sortField: DigitalContentSortField.ReviewCount,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 5,
    text: 'Least reviewed',
    sortMode: {
      sortField: DigitalContentSortField.ReviewCount,
      sortDirection: SortDirection.Ascending
    }
  }
];
