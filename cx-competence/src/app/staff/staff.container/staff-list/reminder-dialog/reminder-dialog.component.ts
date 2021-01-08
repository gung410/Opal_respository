import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { ToastrService } from 'ngx-toastr';
import {
  LearningNeedAnalysisRemindingList,
  LearningNeedAnalysisRemindingRequest,
} from '../models/reminder-request.model';

@Component({
  selector: 'reminder-dialog',
  templateUrl: './reminder-dialog.component.html',
  styleUrls: ['./reminder-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ReminderDialogComponent
  extends BaseSmartComponent
  implements OnInit {
  dateToSend: Date = new Date();
  currentDate: Date = new Date();
  remindNow: boolean = true;
  remindingLearners: LearningNeedAnalysisRemindingRequest[];
  @Input() selectedEmployees: any[];
  @Output()
  done: EventEmitter<LearningNeedAnalysisRemindingList> = new EventEmitter();
  @Output() cancel: EventEmitter<void> = new EventEmitter();
  constructor(
    private toastrService: ToastrService,
    private translateService: TranslateService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }
  ngOnInit(): void {
    this.remindingLearners = this.selectedEmployees.map((employee) => {
      return new LearningNeedAnalysisRemindingRequest({
        userExtId: employee.identity.extId,
        userFullName: employee.fullName,
        cutOffDate: employee.assessmentInfos.LearningNeed.dueDate,
        resultExtId: employee.assessmentInfos.LearningNeed.identity.extId,
      });
    });
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onSelectImmediateReminder(remindNow: boolean): void {
    this.remindNow = remindNow;
  }

  onDone(): void {
    if (this.isAllowToRemind) {
      this.done.emit(
        new LearningNeedAnalysisRemindingList({
          learningNeedCompletionRemindings: this.remindingLearners,
          dateToSend: this.remindNow ? new Date() : this.dateToSend,
        })
      );
    }
  }

  get isAllowToRemind(): boolean {
    const dateToSend = this.remindNow ? new Date() : this.dateToSend;
    const isAllowToRemind =
      this.remindingLearners.findIndex(
        (remindingLearner) => remindingLearner.cutOffDate < dateToSend
      ) > -1;
    if (isAllowToRemind) {
      this.toastrService.warning(
        this.translateService.instant(
          'Staff.ReminderDialog.Alert.Warning.GreaterThenCutOffDate'
        )
      );

      return false;
    }

    return true;
  }
}
