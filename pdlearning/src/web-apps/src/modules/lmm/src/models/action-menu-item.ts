import { CourseContentItemModel } from '@opal20/domain-api';

export enum ContentItemAction {
  MoveUp = 'MoveUp',
  MoveDown = 'MoveDown',
  Edit = 'Edit',
  Delete = 'Delete'
}

export interface IActionMenuItem {
  actionType: ContentItemAction;
  item: CourseContentItemModel;
  parent?: CourseContentItemModel;
}
