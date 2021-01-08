import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { LearnerAssignPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { ImageHelpers } from 'app-utilities/image-helpers';

@Component({
  selector: 'nomination-member-list',
  templateUrl: './nomination-member-list.component.html',
  styleUrls: ['./nomination-member-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NominationMemberList {
  @Input() pageSize: number = 10;
  @Input() pageIndex: number = 1;

  @Input() pagingLearnerResultModel: PagingResponseModel<
    LearnerAssignPDOResultModel
  >;

  @Output() pagingChanged: EventEmitter<{
    pageIndex: number;
    pageSize: number;
  }> = new EventEmitter();

  public onPagingChange(pageIndex: number, pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = pageIndex;
    this.pagingChanged.emit({
      pageIndex,
      pageSize,
    });
  }

  getAvatar(email: string): string {
    return ImageHelpers.getAvatarFromEmail(email);
  }
}
