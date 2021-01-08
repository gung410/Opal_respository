import { Component, Input } from '@angular/core';
import { CxDialogTemplateComponent } from '@conexus/cx-angular-common';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { Constant } from 'app/shared/app.constant';
import { isEmpty, trim } from 'lodash';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'pd-evaluation-dialog',
  templateUrl: './pd-evaluation-dialog.component.html',
  styleUrls: ['./pd-evaluation-dialog.component.scss'],
})
export class PdEvaluationDialogComponent extends CxDialogTemplateComponent {
  @Input() header: string;
  @Input() subHeader: string;
  commentText: string = '';
  maxLength: number = Constant.LONG_TEXT_MAX_LENGTH;

  constructor(
    private toastrService: ToastrService,
    private translateAdapterService: TranslateAdapterService
  ) {
    super();
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onDone(): void {
    const trimmedText = trim(this.commentText);

    if (isEmpty(trimmedText)) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.ApproveLearningNeedsResultDialog.ValidationEmptyText'
        )
      );
    } else {
      this.done.emit(trimmedText);
    }
  }
}
