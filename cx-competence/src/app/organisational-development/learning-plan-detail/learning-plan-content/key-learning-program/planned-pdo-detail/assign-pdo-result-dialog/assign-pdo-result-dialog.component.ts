import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { AssignModeEnum } from '../planned-pdo-detail.model';
import { AssignResultModel } from './assign-pdo-result-dialog.model';

@Component({
  selector: 'assign-pdo-result-dialog',
  templateUrl: './assign-pdo-result-dialog.component.html',
  styleUrls: ['./assign-pdo-result-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AssignPDOResultDialogComponent implements OnInit {
  @Input() invalidResults: AssignResultModel[] = [];
  @Input() totalLearner: number = 0;
  @Input() isSuccess: boolean;
  @Input() assignMode:
    | AssignModeEnum.Nominate
    | AssignModeEnum.Recommend
    | AssignModeEnum.AdhocNominate;
  @Input() isConfirmMode: boolean = false;
  @Input() approvalMode: boolean = false;

  @Output() close: EventEmitter<void> = new EventEmitter();
  @Output() confirm: EventEmitter<void> = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  get titleName(): string {
    return this.approvalMode
      ? 'Approve'
      : this.isNominate
      ? 'Nominate'
      : 'Recommend';
  }

  get subHeaderActionName(): string {
    return this.approvalMode
      ? 'approved'
      : this.isNominate
      ? 'nominated'
      : 'recommended';
  }

  get headerContentTranslatePath(): string {
    if (this.approvalMode) {
      return this.isSuccess
        ? 'Odp.LearningPlan.PlannedPDODetail.ApproveSuccess'
        : 'Odp.LearningPlan.PlannedPDODetail.ApproveUnSuccess';
    }

    switch (this.assignMode) {
      case AssignModeEnum.Nominate:
      case AssignModeEnum.AdhocNominate:
        return this.isSuccess
          ? 'Odp.LearningPlan.PlannedPDODetail.NominateSuccess'
          : 'Odp.LearningPlan.PlannedPDODetail.NominateUnSuccess';
      case AssignModeEnum.Recommend:
        return this.isSuccess
          ? 'Odp.LearningPlan.PlannedPDODetail.RecommendSuccess'
          : 'Odp.LearningPlan.PlannedPDODetail.RecommendUnSuccess';
      default:
        return 'N/A';
    }
  }

  objectiveName(isPlural: boolean = true): string {
    return this.approvalMode
      ? isPlural
        ? 'nomination requests'
        : 'nomination request'
      : isPlural
      ? 'learners'
      : 'learner';
  }

  get isNominate(): boolean {
    return (
      this.assignMode === AssignModeEnum.Nominate ||
      this.assignMode === AssignModeEnum.AdhocNominate
    );
  }

  get validLearnersCount(): number {
    const validLearners = this.totalLearner - this.invalidResults.length;

    return validLearners > 0 ? validLearners : 0;
  }

  onClose(): void {
    this.close.emit();
  }

  onConfirm(): void {
    this.confirm.emit();
  }
}
