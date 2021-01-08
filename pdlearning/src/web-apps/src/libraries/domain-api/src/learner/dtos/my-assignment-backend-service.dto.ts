import { IMyAssignment, MyAssignment } from '../models/my-assignment.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';

export class PagedMyAssignmentResult implements IPagedResultDto<IMyAssignment> {
  public totalCount: number = 0;
  public items: MyAssignment[] = [];

  constructor(data?: IPagedResultDto<IMyAssignment>) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items !== undefined ? data.items.map(p => new MyAssignment(p)) : [];
  }
}
