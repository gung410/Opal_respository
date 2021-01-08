import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { IApprovalUnitModel } from 'app/approval-page/models/class-registration.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';

@Component({
  selector: 'nomination-status-tabs',
  templateUrl: './nomination-status-tabs.component.html',
  styleUrls: ['./nomination-status-tabs.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NominationStatusTabs {
  @Input() approvalResultModel: IApprovalUnitModel;
  @Input() activeStatus: NominateStatusCodeEnum;
  @Input() isExternalPDO: boolean;
  @Output() onOpenTab: EventEmitter<
    NominateStatusCodeEnum
  > = new EventEmitter();

  public nominateStatus: typeof NominateStatusCodeEnum = NominateStatusCodeEnum;

  public openTab(status: NominateStatusCodeEnum): void {
    this.onOpenTab.emit(status);
  }
}
