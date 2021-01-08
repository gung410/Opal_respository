import { ClassRun, ClassRunStatus } from '@opal20/domain-api';

import { ContextMenuAction } from '@opal20/domain-components';

export class ClassRunDetailAction {
  public action: ContextMenuAction;
  public dataItem: ClassRun;
  public status: ClassRunStatus;
}
