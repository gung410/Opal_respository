import { IOutstandingTask, OutstandingTask } from '../models/my-outstanding-task.model';

import { IPagedResultDto } from './../../share/dtos/paged-result.dto';

export interface IOutstandingTaskResult extends IPagedResultDto<IOutstandingTask> {}

export class OutstandingTaskResult implements IOutstandingTaskResult {
  public totalCount: number = 0;
  public items: OutstandingTask[] = [];
  constructor(data?: IOutstandingTaskResult) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items.map(item => new OutstandingTask(item));
  }
}
