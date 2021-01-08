import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { MatSelectChange } from '@angular/material/select';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { AppConstant } from 'app/shared/app.constant';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import * as moment from 'moment';

enum NeedsStatus {
  NotStarted = 'not-started',
  Started = 'not-started',
  PendingForApproval = 'pending-for-approval',
  Approved = 'approved',
  Completed = 'completed',
  Rejected = 'rejected',
}

const LNA_REVIEWABLE_STATUSES = [
  IdpStatusCodeEnum.PendingForApproval,
  IdpStatusCodeEnum.Approved,
  IdpStatusCodeEnum.Completed,
  IdpStatusCodeEnum.Rejected,
];

@Component({
  selector: 'learning-needs-header',
  templateUrl: './learning-needs-header.component.html',
  styleUrls: ['./learning-needs-header.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LearningNeedsHeaderComponent
  extends BaseSmartComponent
  implements OnChanges {
  @Input() needsResults: IdpDto[];
  @Input() defaultResult: IdpDto;
  @Input() allowReviewLNA: boolean;
  @Output() periodChange: EventEmitter<IdpDto> = new EventEmitter<IdpDto>();
  @Output() reviewLNA: EventEmitter<IdpDto> = new EventEmitter<IdpDto>();

  cutoffDate: string;
  currentStatus: string;
  completionRate: number;
  needsStatus: any = NeedsStatus;
  currentNeedsResult: IdpDto;

  showReviewButton: boolean;
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  ngOnChanges(): void {
    this.currentNeedsResult = this.defaultResult;
    this.updateDisplayValue();
  }

  onReviewLNAClicked(): void {
    this.reviewLNA.emit(this.currentNeedsResult);
  }

  onPeriodChanged({ value }: MatSelectChange): void {
    const result = this.needsResults.find(
      (rs) => rs.resultIdentity.extId === value
    );
    this.currentNeedsResult = result;
    this.updateDisplayValue();
  }

  private updateDisplayValue(): void {
    this.cutoffDate = this.currentNeedsResult.dueDate
      ? moment(this.currentNeedsResult.dueDate).format(
          AppConstant.backendDateFormat
        )
      : '--';
    this.currentStatus = this.currentNeedsResult.assessmentStatusInfo.assessmentStatusCode;
    this.completionRate =
      this.currentNeedsResult.additionalProperties.completionRate || 0;
    this.showReviewButton = LNA_REVIEWABLE_STATUSES.includes(
      this.currentStatus as IdpStatusCodeEnum
    );
    this.periodChange.emit(this.currentNeedsResult);
  }
}
