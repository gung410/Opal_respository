import { DigitalContentSortField } from './digital-content-sort-field.model';
import { SortDirection } from '../../share/dtos/sort-direction';

export interface SortDisplayItem {
  text: string;
  sortMode: SortMode;
}

export interface SortMode {
  sortField: DigitalContentSortField;
  sortDirect: SortDirection;
}
