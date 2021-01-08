import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { IDPMode } from 'app/individual-development/idp.constant';

@Component({
  selector: 'learning-needs-analysis-review-dialog',
  templateUrl: './learning-needs-analysis-review-dialog.component.html',
  styleUrls: ['./learning-needs-analysis-review-dialog.component.scss'],
})
export class LearningNeedsAnalysisReviewDialogComponent implements OnInit {
  @Input() user: Staff;
  @Input() learningNeeds: IdpDto;
  @Input() needsResults: IdpDto[];
  @Input() mode: IDPMode = IDPMode.Learner;
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();
  @Output() needToReloadLNA: EventEmitter<void> = new EventEmitter<void>();
  constructor() {}

  ngOnInit(): void {}

  onCancel(): void {
    this.cancel.emit();
  }

  onNeedToReloadLNA(): void {
    this.needToReloadLNA.emit();
  }
}
