import {
  CxItemTableHeaderModel,
  CxColumnSortType,
} from '@conexus/cx-angular-common';

export const StaffTableHeaders: CxItemTableHeaderModel[] = [
  {
    text: 'NAME',
    fieldSort: 'firstName',
    sortType: CxColumnSortType.ASCENDING,
  },
  {
    text: 'APPROVING OFFICER',
    fieldSort: '',
    sortType: CxColumnSortType.UNSORTED,
  },
  {
    text: 'SERVICE SCHEME',
    fieldSort: '',
    sortType: CxColumnSortType.UNSORTED,
  },
  {
    text: 'LNA STATUS',
    fieldSort: '',
    sortType: CxColumnSortType.UNSORTED,
  },
  {
    text: 'COMPLETE BY',
    fieldSort: 'dueDate',
    sortType: CxColumnSortType.UNSORTED,
  },
];
