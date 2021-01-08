import { CourseContentItemModel, CourseContentItemType, LectureType } from '@opal20/domain-api';

export enum CreationPosition {
  Before = 'Before',
  After = 'After',
  Last = 'Last',
  Include = 'Include'
}

export interface ICreationMenuItem {
  itemType: CourseContentItemType | LectureType;
  order: number;
  item?: CourseContentItemModel;
  parent?: CourseContentItemModel;
}
