import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CxDialogTemplateComponent } from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { MassNominationResultDto } from 'app-models/pdcatalog/pdcatalog.dto';

@Component({
  selector: 'invalid-nomination-record-dialog',
  templateUrl: './invalid-nomination-record-dialog.component.html',
  styleUrls: ['./invalid-nomination-record-dialog.component.scss'],
})
export class InvalidNominationRecordDialogComponent extends CxDialogTemplateComponent {
  @Input() invalidNominatingResults: MassNominationResultDto[];

  @Output() cancel: EventEmitter<any> = new EventEmitter();
  cancelButtonText: string = this.translateService.instant(
    'Common.Action.Close'
  ) as string;

  constructor(
    private translateService: TranslateService
  ) {
    super();
  }

  ngOnInit(): void { }

  onCancel(): void {
    this.cancel.emit();
  }
}
