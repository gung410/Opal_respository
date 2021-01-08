import { Course, CourseStatus } from '@opal20/domain-api';

import { ContextMenuAction } from '@opal20/domain-components';

export class CourseDetailAction {
  public action: ContextMenuAction;
  public dataItem: Course;
  public status: CourseStatus;
}
