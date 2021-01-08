import { SortDirection } from '../../share/dtos/sort-direction';

export enum PersonalFileSortField {
  CreatedDate = 'CreatedDate',
  FileName = 'FileName',
  FileSize = 'FileSize'
}

export interface IPersonalFileSortMode {
  sortField: PersonalFileSortField;
  sortDirection: SortDirection;
}

export interface PersonalFileSortItem {
  id: number;
  text: string;
  sortMode: IPersonalFileSortMode;
}

export const PERSONAL_FILE_SORT_ITEMS: PersonalFileSortItem[] = [
  {
    id: 1,
    text: 'Recently upload',
    sortMode: {
      sortField: PersonalFileSortField.CreatedDate,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 2,
    text: 'Name (Descending)',
    sortMode: {
      sortField: PersonalFileSortField.FileName,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 3,
    text: 'Name (Ascending)',
    sortMode: {
      sortField: PersonalFileSortField.FileName,
      sortDirection: SortDirection.Ascending
    }
  },
  {
    id: 4,
    text: 'File Size (Descending)',
    sortMode: {
      sortField: PersonalFileSortField.FileSize,
      sortDirection: SortDirection.Descending
    }
  },
  {
    id: 5,
    text: 'File Size (Ascending)',
    sortMode: {
      sortField: PersonalFileSortField.FileSize,
      sortDirection: SortDirection.Ascending
    }
  }
];
